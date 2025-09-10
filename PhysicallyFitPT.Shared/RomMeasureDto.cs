// <copyright file="RomMeasureDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

using System;

/// <summary>
/// Data transfer object for range of motion measurements.
/// </summary>
public class RomMeasureDto
{
  /// <summary>
  /// Gets or sets the unique identifier of the ROM measurement.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the joint being measured.
  /// </summary>
  public string Joint { get; set; } = null!;

  /// <summary>
  /// Gets or sets the movement being measured.
  /// </summary>
  public string Movement { get; set; } = null!;

  /// <summary>
  /// Gets or sets the side of the body (left/right).
  /// </summary>
  public int Side { get; set; }

  /// <summary>
  /// Gets or sets the measured degrees of motion.
  /// </summary>
  public int? MeasuredDegrees { get; set; }

  /// <summary>
  /// Gets or sets the normal degrees of motion for comparison.
  /// </summary>
  public int? NormalDegrees { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether the movement causes pain.
  /// </summary>
  public bool WithPain { get; set; }

  /// <summary>
  /// Gets or sets additional notes about the measurement.
  /// </summary>
  public string? Notes { get; set; }
}
