// <copyright file="PlanDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

using System.Collections.Generic;

/// <summary>
/// Data transfer object representing the plan portion of a clinical note.
/// </summary>
public class PlanDto
{
  /// <summary>
  /// Gets or sets the treatment frequency.
  /// </summary>
  public string? Frequency { get; set; }

  /// <summary>
  /// Gets or sets the treatment duration.
  /// </summary>
  public string? Duration { get; set; }

  /// <summary>
  /// Gets or sets the planned interventions as a CSV string.
  /// </summary>
  public string? PlannedInterventionsCsv { get; set; }

  /// <summary>
  /// Gets or sets the focus for the next visit.
  /// </summary>
  public string? NextVisitFocus { get; set; }

  /// <summary>
  /// Gets or sets the home exercise program prescriptions.
  /// </summary>
  public List<ExercisePrescriptionDto> Hep { get; set; } = new();
}
