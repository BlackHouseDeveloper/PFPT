// <copyright file="BaseSeedTask.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.Extensions.Logging;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Seeder.Seeding.Hashing;

namespace PhysicallyFitPT.Seeder.Seeding;

/// <summary>
/// Base class for seed tasks providing common functionality.
/// </summary>
public abstract class BaseSeedTask : ISeedTask
{
  /// <summary>
  /// Initializes a new instance of the <see cref="BaseSeedTask"/> class.
  /// </summary>
  /// <param name="dbContext">Database context.</param>
  /// <param name="hashCalculator">Hash calculator for content descriptors.</param>
  /// <param name="logger">Logger instance.</param>
  protected BaseSeedTask(
    ApplicationDbContext dbContext,
    SeedHashCalculator hashCalculator,
    ILogger logger)
  {
    DbContext = dbContext;
    HashCalculator = hashCalculator;
    Logger = logger;
  }

  /// <inheritdoc/>
  public abstract string Id { get; }

  /// <inheritdoc/>
  public abstract string Name { get; }

  /// <inheritdoc/>
  public virtual IReadOnlyList<string> AllowedEnvironments { get; } = Array.Empty<string>();

  /// <summary>
  /// Gets the database context.
  /// </summary>
  protected ApplicationDbContext DbContext { get; }

  /// <summary>
  /// Gets the hash calculator.
  /// </summary>
  protected SeedHashCalculator HashCalculator { get; }

  /// <summary>
  /// Gets the logger.
  /// </summary>
  protected ILogger Logger { get; }

  /// <inheritdoc/>
  public abstract Task ExecuteAsync(CancellationToken cancellationToken = default);

  /// <inheritdoc/>
  public abstract Task<string> ComputeContentDescriptorAsync();
}