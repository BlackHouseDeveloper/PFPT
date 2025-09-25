// <copyright file="SeedRunner.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using PhysicallyFitPT.Core;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Seeder.Utils;

namespace PhysicallyFitPT.Seeder.Seeding;

/// <summary>
/// Core service for running seed tasks with locking, transaction management, and change detection.
/// </summary>
public class SeedRunner
{
  private readonly ApplicationDbContext dbContext;
  private readonly IEnumerable<ISeedTask> seedTasks;
  private readonly ILogger<SeedRunner> logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="SeedRunner"/> class.
  /// </summary>
  /// <param name="dbContext">Database context.</param>
  /// <param name="seedTasks">Collection of registered seed tasks.</param>
  /// <param name="logger">Logger instance.</param>
  public SeedRunner(
    ApplicationDbContext dbContext,
    IEnumerable<ISeedTask> seedTasks,
    ILogger<SeedRunner> logger)
  {
    this.dbContext = dbContext;
    this.seedTasks = seedTasks;
    this.logger = logger;
  }

  /// <summary>
  /// Runs seed tasks based on the provided options.
  /// </summary>
  /// <param name="options">Seeding options.</param>
  /// <param name="cancellationToken">Cancellation token.</param>
  /// <returns>True if all tasks completed successfully.</returns>
  public async Task<bool> RunAsync(SeedRunOptions options, CancellationToken cancellationToken = default)
  {
    logger.LogInformation("Starting seed run with environment: {Environment}", options.Environment);

    if (!await AcquireLockAsync())
    {
      logger.LogError("Failed to acquire seeder lock. Another seeding process may be running.");
      return false;
    }

    try
    {
      var tasks = GetFilteredTasks(options);
      var success = true;

      foreach (var task in tasks)
      {
        if (!await RunSingleTaskAsync(task, options, cancellationToken))
        {
          success = false;
          if (!options.ContinueOnError)
          {
            break;
          }
        }
      }

      logger.LogInformation("Seed run completed. Success: {Success}", success);
      return success;
    }
    finally
    {
      await ReleaseLockAsync();
    }
  }

  /// <summary>
  /// Lists all tasks with their current status.
  /// </summary>
  /// <param name="environment">Current environment.</param>
  /// <returns>Task status information.</returns>
  public async Task<IList<SeedTaskStatus>> ListTasksAsync(string environment)
  {
    var allTasks = seedTasks.OrderBy(t => t.Id).ToList();
    var appliedTasks = await dbContext.SeedHistory.ToDictionaryAsync(h => h.TaskId, h => h);

    var result = new List<SeedTaskStatus>();

    foreach (var task in allTasks)
    {
      var currentHash = await task.ComputeContentDescriptorAsync();
      var applied = appliedTasks.TryGetValue(task.Id, out var history);
      var hashChanged = applied && history!.Hash != currentHash;
      var environmentAllowed = EnvDetector.IsEnvironmentAllowed(task.AllowedEnvironments, environment);

      string? pendingReason = null;
      if (!applied)
      {
        if (!environmentAllowed)
        {
          pendingReason = $"Environment mismatch (allowed: {string.Join(", ", task.AllowedEnvironments)})";
        }
        else if (hashChanged)
        {
          pendingReason = "Hash changed since last run";
        }
      }

      result.Add(new SeedTaskStatus
      {
        Id = task.Id,
        Name = task.Name,
        Applied = applied,
        CurrentHash = currentHash,
        StoredHash = history?.Hash,
        AllowedEnvironments = task.AllowedEnvironments.ToList(),
        PendingReason = pendingReason,
      });
    }

    return result;
  }

  private async Task<bool> AcquireLockAsync()
  {
    try
    {
      var lockEntity = new SeederLock
      {
        AcquiredAtUtc = DateTimeOffset.UtcNow,
        ProcessInfo = $"{Environment.MachineName}:{Environment.ProcessId}",
      };

      dbContext.SeederLocks.Add(lockEntity);
      await dbContext.SaveChangesAsync();
      return true;
    }
    catch (DbUpdateException)
    {
      // Lock already exists
      return false;
    }
  }

  private async Task ReleaseLockAsync()
  {
    try
    {
      var lockEntity = await dbContext.SeederLocks.FirstOrDefaultAsync(l => l.Id == 1);
      if (lockEntity != null)
      {
        dbContext.SeederLocks.Remove(lockEntity);
        await dbContext.SaveChangesAsync();
      }
    }
    catch (Exception ex)
    {
      logger.LogWarning(ex, "Failed to release seeder lock");
    }
  }

  private IEnumerable<ISeedTask> GetFilteredTasks(SeedRunOptions options)
  {
    var tasks = seedTasks.OrderBy(t => t.Id);

    if (!string.IsNullOrEmpty(options.TaskFilter))
    {
      return tasks.Where(t => t.Id.Equals(options.TaskFilter, StringComparison.OrdinalIgnoreCase) ||
                              t.Name.Contains(options.TaskFilter, StringComparison.OrdinalIgnoreCase));
    }

    return tasks;
  }

  private async Task<bool> RunSingleTaskAsync(ISeedTask task, SeedRunOptions options, CancellationToken cancellationToken)
  {
    logger.LogDebug("Evaluating task {TaskId}: {TaskName}", task.Id, task.Name);

    // Check environment restrictions
    if (!options.Force && !EnvDetector.IsEnvironmentAllowed(task.AllowedEnvironments, options.Environment))
    {
      logger.LogDebug("Skipping task {TaskId} due to environment restriction", task.Id);
      return true;
    }

    // Check if task needs to run
    var currentHash = await task.ComputeContentDescriptorAsync();
    var existingHistory = await dbContext.SeedHistory.FirstOrDefaultAsync(h => h.TaskId == task.Id, cancellationToken);

    var hashChanged = existingHistory != null && existingHistory.Hash != currentHash;
    var shouldRun = existingHistory == null ||
                    hashChanged ||
                    options.ReplayChanged ||
                    options.Force;

    if (!shouldRun)
    {
      logger.LogDebug("Skipping task {TaskId} - already applied with current hash", task.Id);
      return true;
    }

    if (options.DryRun)
    {
      logger.LogInformation("DRY RUN: Would execute task {TaskId}: {TaskName}", task.Id, task.Name);
      return true;
    }

    var canUseTransactions = dbContext.Database.IsRelational();
    IDbContextTransaction? transaction = null;

    try
    {
      if (canUseTransactions)
      {
        transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
      }

      logger.LogInformation("Executing task {TaskId}: {TaskName}", task.Id, task.Name);
      await task.ExecuteAsync(cancellationToken);

      // Update or insert history
      if (existingHistory != null)
      {
        existingHistory.Hash = currentHash;
        existingHistory.AppliedAtUtc = DateTimeOffset.UtcNow;
      }
      else
      {
        dbContext.SeedHistory.Add(new SeedHistory
        {
          TaskId = task.Id,
          Name = task.Name,
          Hash = currentHash,
          AppliedAtUtc = DateTimeOffset.UtcNow,
        });
      }

      await dbContext.SaveChangesAsync(cancellationToken);

      if (transaction != null)
      {
        await transaction.CommitAsync(cancellationToken);
      }

      logger.LogInformation("Task {TaskId} completed successfully", task.Id);
      return true;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Task {TaskId} failed", task.Id);
      if (transaction != null)
      {
        await transaction.RollbackAsync(cancellationToken);
      }
      return false;
    }
    finally
    {
      if (transaction != null)
      {
        await transaction.DisposeAsync();
      }
    }
  }
}

/// <summary>
/// Options for controlling seed task execution.
/// </summary>
public class SeedRunOptions
{
  /// <summary>
  /// Gets or sets the target environment.
  /// </summary>
  public string Environment { get; set; } = "Development";

  /// <summary>
  /// Gets or sets a value indicating whether to override environment restrictions.
  /// </summary>
  public bool Force { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether to re-run tasks with changed hashes.
  /// </summary>
  public bool ReplayChanged { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether to perform a dry run (no actual changes).
  /// </summary>
  public bool DryRun { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether to continue executing tasks after a failure.
  /// </summary>
  public bool ContinueOnError { get; set; }

  /// <summary>
  /// Gets or sets an optional filter to run only specific tasks.
  /// </summary>
  public string? TaskFilter { get; set; }
}

/// <summary>
/// Status information for a seed task.
/// </summary>
public class SeedTaskStatus
{
  /// <summary>
  /// Gets or sets the task identifier.
  /// </summary>
  public string Id { get; set; } = null!;

  /// <summary>
  /// Gets or sets the task name.
  /// </summary>
  public string Name { get; set; } = null!;

  /// <summary>
  /// Gets or sets a value indicating whether the task has been applied.
  /// </summary>
  public bool Applied { get; set; }

  /// <summary>
  /// Gets or sets the current hash of the task content.
  /// </summary>
  public string CurrentHash { get; set; } = null!;

  /// <summary>
  /// Gets or sets the stored hash from the database.
  /// </summary>
  public string? StoredHash { get; set; }

  /// <summary>
  /// Gets or sets the allowed environments for this task.
  /// </summary>
  public List<string> AllowedEnvironments { get; set; } = new();

  /// <summary>
  /// Gets or sets the reason why the task is pending (if applicable).
  /// </summary>
  public string? PendingReason { get; set; }
}
