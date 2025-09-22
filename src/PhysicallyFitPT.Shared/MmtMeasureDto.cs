// <copyright file="MmtMeasureDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  /// <summary>
  /// Represents a manual muscle test measurement data transfer object.
  /// </summary>
  public class MmtMeasureDto
  {
    /// <summary>
    /// Gets or sets the unique identifier for this MMT measurement.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the muscle group being tested.
    /// </summary>
    public string MuscleGroup { get; set; } = null!;

    /// <summary>
    /// Gets or sets the side of the body being tested.
    /// </summary>
    public int Side { get; set; }

    /// <summary>
    /// Gets or sets the muscle strength grade using the standard 0-5 scale.
    /// </summary>
    public string Grade { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether pain was present during the test.
    /// </summary>
    public bool WithPain { get; set; }

    /// <summary>
    /// Gets or sets additional notes or observations about the test.
    /// </summary>
    public string? Notes { get; set; }
  }
}
