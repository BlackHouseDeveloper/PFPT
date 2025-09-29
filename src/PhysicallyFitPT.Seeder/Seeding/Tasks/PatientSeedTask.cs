// <copyright file="PatientSeedTask.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhysicallyFitPT.Core;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Seeder.Seeding.Hashing;
using PhysicallyFitPT.Seeder.Utils;

namespace PhysicallyFitPT.Seeder.Seeding.Tasks;

/// <summary>
/// Seed task for sample patients (003.patients.sample).
/// This task only runs in Development environment unless forced.
/// </summary>
public class PatientSeedTask : BaseSeedTask
{
  private const string TaskId = "003.patients.sample";
  private const string TaskName = "Sample Patients";
  private const string DataFileName = "patients.dev.json";

  private readonly JsonDataLoader dataLoader;

  /// <summary>
  /// Initializes a new instance of the <see cref="PatientSeedTask"/> class.
  /// </summary>
  /// <param name="dbContext">Database context.</param>
  /// <param name="hashCalculator">Hash calculator.</param>
  /// <param name="dataLoader">JSON data loader.</param>
  /// <param name="logger">Logger instance.</param>
  public PatientSeedTask(
    ApplicationDbContext dbContext,
    SeedHashCalculator hashCalculator,
    JsonDataLoader dataLoader,
    ILogger<PatientSeedTask> logger)
    : base(dbContext, hashCalculator, logger)
  {
    this.dataLoader = dataLoader;
  }

  /// <inheritdoc/>
  public override string Id => TaskId;

  /// <inheritdoc/>
  public override string Name => TaskName;

  /// <inheritdoc/>
  public override IReadOnlyList<string> AllowedEnvironments { get; } = [EnvDetector.Environments.Development];

  /// <inheritdoc/>
  public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
  {
    var filePath = JsonDataLoader.GetDataFilePath(DataFileName);
    var fallbackData = GetFallbackData();
    var seedData = await dataLoader.LoadDataAsync(filePath, fallbackData);

    Logger.LogInformation("Processing {Count} sample patients", seedData.Length);

    foreach (var data in seedData)
    {
      var existing = await DbContext.Patients
        .FirstOrDefaultAsync(p => p.MRN == data.MRN, cancellationToken);

      if (existing != null)
      {
        // Update existing record if data changed
        var updated = false;
        if (existing.FirstName != data.FirstName)
        {
          existing.FirstName = data.FirstName;
          updated = true;
        }

        if (existing.LastName != data.LastName)
        {
          existing.LastName = data.LastName;
          updated = true;
        }

        if (existing.Email != data.Email)
        {
          existing.Email = data.Email;
          updated = true;
        }

        if (updated)
        {
          Logger.LogDebug("Updated patient {MRN}", data.MRN);
        }
      }
      else
      {
        // Add new record
        DbContext.Patients.Add(new Patient
        {
          MRN = data.MRN,
          FirstName = data.FirstName,
          LastName = data.LastName,
          Email = data.Email,
        });
        Logger.LogDebug("Added patient {MRN}", data.MRN);
      }
    }

    await DbContext.SaveChangesAsync(cancellationToken);
  }

  /// <inheritdoc/>
  public override async Task<string> ComputeContentDescriptorAsync()
  {
    var filePath = JsonDataLoader.GetDataFilePath(DataFileName);
    var fileHash = await HashCalculator.ComputeFileHashAsync(filePath);

    if (fileHash != null)
    {
      return SeedHashCalculator.ComputeHash(Id, fileHash);
    }

    // Use fallback signature
    var fallbackSignature = GetFallbackSignature();
    return HashCalculator.ComputeFallbackHash(Id, fallbackSignature);
  }

  private static PatientSeedData[] GetFallbackData()
  {
    return new[]
    {
      new PatientSeedData { MRN = "A1001", FirstName = "Jane", LastName = "Doe", Email = "jane@example.com" },
      new PatientSeedData { MRN = "A1002", FirstName = "John", LastName = "Smith", Email = "john@example.com" },
      new PatientSeedData { MRN = "A1003", FirstName = "Sam", LastName = "Sample", Email = "sam@example.com" },
    };
  }

  private static string GetFallbackSignature()
  {
    // Create a signature of the fallback data for hashing
    return "A1001:Jane:Doe:jane@example.com|A1002:John:Smith:john@example.com|A1003:Sam:Sample:sam@example.com";
  }
}
