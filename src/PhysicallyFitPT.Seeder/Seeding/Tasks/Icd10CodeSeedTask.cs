// <copyright file="Icd10CodeSeedTask.cs" company="PlaceholderCompany">
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
/// Seed task for ICD-10 codes (002.icd10.codes).
/// </summary>
public class Icd10CodeSeedTask : BaseSeedTask
{
  private const string TaskId = "002.icd10.codes";
  private const string TaskName = "ICD-10 Codes";
  private const string DataFileName = "icd10.json";

  private readonly JsonDataLoader dataLoader;

  /// <summary>
  /// Initializes a new instance of the <see cref="Icd10CodeSeedTask"/> class.
  /// </summary>
  /// <param name="dbContext">Database context.</param>
  /// <param name="hashCalculator">Hash calculator.</param>
  /// <param name="dataLoader">JSON data loader.</param>
  /// <param name="logger">Logger instance.</param>
  public Icd10CodeSeedTask(
    ApplicationDbContext dbContext,
    SeedHashCalculator hashCalculator,
    JsonDataLoader dataLoader,
    ILogger<Icd10CodeSeedTask> logger)
    : base(dbContext, hashCalculator, logger)
  {
    this.dataLoader = dataLoader;
  }

  /// <inheritdoc/>
  public override string Id => TaskId;

  /// <inheritdoc/>
  public override string Name => TaskName;

  /// <inheritdoc/>
  public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
  {
    var filePath = JsonDataLoader.GetDataFilePath(DataFileName);
    var fallbackData = GetFallbackData();
    var seedData = await dataLoader.LoadDataAsync(filePath, fallbackData);

    Logger.LogInformation("Processing {Count} ICD-10 codes", seedData.Length);

    foreach (var data in seedData)
    {
      var existing = await DbContext.Icd10Codes
        .FirstOrDefaultAsync(c => c.Code == data.Code, cancellationToken);

      if (existing != null)
      {
        // Update existing record if description changed
        if (existing.Description != data.Description)
        {
          existing.Description = data.Description;
          Logger.LogDebug("Updated ICD-10 code {Code}", data.Code);
        }
      }
      else
      {
        // Add new record
        DbContext.Icd10Codes.Add(new Icd10Code
        {
          Code = data.Code,
          Description = data.Description,
        });
        Logger.LogDebug("Added ICD-10 code {Code}", data.Code);
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
      return HashCalculator.ComputeHash(Id, fileHash);
    }

    // Use fallback signature
    var fallbackSignature = GetFallbackSignature();
    return HashCalculator.ComputeFallbackHash(Id, fallbackSignature);
  }

  private static Icd10CodeSeedData[] GetFallbackData()
  {
    return new[]
    {
      new Icd10CodeSeedData { Code = "M25.561", Description = "Pain in right knee" },
      new Icd10CodeSeedData { Code = "M25.562", Description = "Pain in left knee" },
      new Icd10CodeSeedData { Code = "M54.50", Description = "Low back pain, unspecified" },
    };
  }

  private static string GetFallbackSignature()
  {
    // Create a signature of the fallback data for hashing
    return "M25.561:Pain in right knee|M25.562:Pain in left knee|M54.50:Low back pain, unspecified";
  }
}