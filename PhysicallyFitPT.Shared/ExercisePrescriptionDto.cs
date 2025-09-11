// <copyright file="ExercisePrescriptionDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  /// <summary>
  /// Data transfer object representing an exercise prescription in a home exercise program.
  /// </summary>
  public class ExercisePrescriptionDto
  {
    /// <summary>
    /// Gets or sets the unique identifier of the exercise prescription.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the prescribed exercise.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the dosage or repetition instructions for the exercise.
    /// </summary>
    public string? Dosage { get; set; }

    /// <summary>
    /// Gets or sets additional notes or instructions for the exercise.
    /// </summary>
    public string? Notes { get; set; }
  }
}
