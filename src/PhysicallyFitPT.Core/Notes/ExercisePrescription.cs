// <copyright file="ExercisePrescription.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core.Notes;

/// <summary>
/// Represents an exercise prescription in the home exercise program.
/// </summary>
public sealed class ExercisePrescription
{
  /// <summary>
  /// Gets or sets the unique identifier for this exercise prescription.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the name of the prescribed exercise.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the dosage or prescription details (e.g., "3 sets of 10 reps").
  /// </summary>
  public string? Dosage { get; set; }

  /// <summary>
  /// Gets or sets additional notes or instructions for the exercise.
  /// </summary>
  public string? Notes { get; set; }
}
