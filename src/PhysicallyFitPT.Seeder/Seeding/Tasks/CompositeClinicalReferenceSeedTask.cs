// <copyright file="CompositeClinicalReferenceSeedTask.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhysicallyFitPT.Core;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Seeder.Seeding.Hashing;

namespace PhysicallyFitPT.Seeder.Seeding.Tasks;

/// <summary>
/// Composite seed task that summarizes reference data (004.composite.reference).
/// This task runs after CPT and ICD-10 codes are seeded.
/// </summary>
public class CompositeClinicalReferenceSeedTask : BaseSeedTask
{
  private const string TaskId = "004.composite.reference";
  private const string TaskName = "Composite Clinical Reference Summary";
  private const string VersionToken = "v1.0"; // Change this to force re-run

  /// <summary>
  /// Initializes a new instance of the <see cref="CompositeClinicalReferenceSeedTask"/> class.
  /// </summary>
  /// <param name="dbContext">Database context.</param>
  /// <param name="hashCalculator">Hash calculator.</param>
  /// <param name="logger">Logger instance.</param>
  public CompositeClinicalReferenceSeedTask(
    ApplicationDbContext dbContext,
    SeedHashCalculator hashCalculator,
    ILogger<CompositeClinicalReferenceSeedTask> logger)
    : base(dbContext, hashCalculator, logger)
  {
  }

  /// <inheritdoc/>
  public override string Id => TaskId;

  /// <inheritdoc/>
  public override string Name => TaskName;

  /// <inheritdoc/>
  public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
  {
    var cptCount = await DbContext.CptCodes.CountAsync(cancellationToken);
    var icd10Count = await DbContext.Icd10Codes.CountAsync(cancellationToken);

    Logger.LogInformation("Reference data summary: {CptCount} CPT codes, {Icd10Count} ICD-10 codes", cptCount, icd10Count);

    // Update or create the reference summary record
    var existing = await DbContext.ReferenceSummaries.FirstOrDefaultAsync(r => r.Id == 1, cancellationToken);

    if (existing != null)
    {
      existing.CptCount = cptCount;
      existing.Icd10Count = icd10Count;
      existing.UpdatedAtUtc = DateTimeOffset.UtcNow;
      Logger.LogDebug("Updated reference summary");
    }
    else
    {
      DbContext.ReferenceSummaries.Add(new ReferenceSummary
      {
        Id = 1,
        CptCount = cptCount,
        Icd10Count = icd10Count,
        UpdatedAtUtc = DateTimeOffset.UtcNow,
      });
      Logger.LogDebug("Created reference summary");
    }

    await DbContext.SaveChangesAsync(cancellationToken);
  }

  /// <inheritdoc/>
  public override Task<string> ComputeContentDescriptorAsync()
  {
    // This task's hash is based on the logic version, not external data
    var hash = HashCalculator.ComputeFallbackHash(Id, "reference-summary-logic", VersionToken);
    return Task.FromResult(hash);
  }
}
