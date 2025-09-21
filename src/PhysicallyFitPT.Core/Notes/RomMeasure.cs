// <copyright file="RomMeasure.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core.Notes;

/// <summary>
/// Represents a range of motion measurement for a specific joint and movement.
/// </summary>
public sealed class RomMeasure
{
  /// <summary>
  /// Gets or sets the unique identifier for this ROM measurement.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the joint being measured (e.g., "Knee").
  /// </summary>
  public string Joint { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the movement being measured (e.g., "Flexion").
  /// </summary>
  public string Movement { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the side of the body being measured.
  /// </summary>
  public Side Side { get; set; } = Side.NA;

  /// <summary>
  /// Gets or sets the measured range of motion in degrees.
  /// </summary>
  public int? MeasuredDegrees { get; set; }

  /// <summary>
  /// Gets or sets the normal range of motion in degrees for comparison.
  /// </summary>
  public int? NormalDegrees { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether pain was present during the measurement.
  /// </summary>
  public bool WithPain { get; set; }

  /// <summary>
  /// Gets or sets additional notes or observations about the measurement.
  /// </summary>
  public string? Notes { get; set; }
}
