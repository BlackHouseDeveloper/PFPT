// <copyright file="MmtMeasure.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core.Notes;

/// <summary>
/// Represents a manual muscle test measurement for a specific muscle group.
/// </summary>
public sealed class MmtMeasure
{
  /// <summary>
  /// Gets or sets the unique identifier for this MMT measurement.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the muscle group being tested (e.g., "Quadriceps").
  /// </summary>
  public string MuscleGroup { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the side of the body being tested.
  /// </summary>
  public Side Side { get; set; } = Side.NA;

  /// <summary>
  /// Gets or sets the muscle strength grade using the standard 0-5 scale.
  /// </summary>
  public string Grade { get; set; } = "3/5";

  /// <summary>
  /// Gets or sets a value indicating whether pain was present during the test.
  /// </summary>
  public bool WithPain { get; set; }

  /// <summary>
  /// Gets or sets additional notes or observations about the test.
  /// </summary>
  public string? Notes { get; set; }
}
