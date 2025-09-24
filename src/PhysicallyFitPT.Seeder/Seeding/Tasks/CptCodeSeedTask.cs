// <copyright file="CptCodeSeedTask.cs" company="PlaceholderCompany">
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
/// Seed task for CPT codes (001.cpt.codes).
/// </summary>
public class CptCodeSeedTask : BaseSeedTask
{
  private const string TaskId = "001.cpt.codes";
  private const string TaskName = "CPT Codes";
  private const string DataFileName = "cpt.json";

  private readonly JsonDataLoader dataLoader;

  /// <summary>
  /// Initializes a new instance of the <see cref="CptCodeSeedTask"/> class.
  /// </summary>
  /// <param name="dbContext">Database context.</param>
  /// <param name="hashCalculator">Hash calculator.</param>
  /// <param name="dataLoader">JSON data loader.</param>
  /// <param name="logger">Logger instance.</param>
  public CptCodeSeedTask(
    ApplicationDbContext dbContext,
    SeedHashCalculator hashCalculator,
    JsonDataLoader dataLoader,
    ILogger<CptCodeSeedTask> logger)
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

    Logger.LogInformation("Processing {Count} CPT codes", seedData.Length);

    foreach (var data in seedData)
    {
      var existing = await DbContext.CptCodes
        .FirstOrDefaultAsync(c => c.Code == data.Code, cancellationToken);

      if (existing != null)
      {
        // Update existing record if description changed
        if (existing.Description != data.Description)
        {
          existing.Description = data.Description;
          Logger.LogDebug("Updated CPT code {Code}", data.Code);
        }
      }
      else
      {
        // Add new record
        DbContext.CptCodes.Add(new CptCode
        {
          Code = data.Code,
          Description = data.Description,
        });
        Logger.LogDebug("Added CPT code {Code}", data.Code);
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

  private static CptCodeSeedData[] GetFallbackData()
  {
    return new[]
    {
      new CptCodeSeedData { Code = "97110", Description = "Therapeutic exercise" },
      new CptCodeSeedData { Code = "97140", Description = "Manual therapy" },
      new CptCodeSeedData { Code = "97530", Description = "Therapeutic activities" },
    };
  }

  private static string GetFallbackSignature()
  {
    // Create a signature of the fallback data for hashing
    return "97110:Therapeutic exercise|97140:Manual therapy|97530:Therapeutic activities";
  }
}