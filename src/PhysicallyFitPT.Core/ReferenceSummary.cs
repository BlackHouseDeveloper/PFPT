// <copyright file="ReferenceSummary.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core;

/// <summary>
/// Represents a summary of reference data counts for the composite reference seed task.
/// </summary>
public class ReferenceSummary
{
  /// <summary>
  /// Gets or sets the summary identifier (always 1 for singleton summary).
  /// </summary>
  public int Id { get; set; } = 1;

  /// <summary>
  /// Gets or sets the count of CPT codes in the system.
  /// </summary>
  public int CptCount { get; set; }

  /// <summary>
  /// Gets or sets the count of ICD-10 codes in the system.
  /// </summary>
  public int Icd10Count { get; set; }

  /// <summary>
  /// Gets or sets the timestamp when the summary was last updated.
  /// </summary>
  public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
