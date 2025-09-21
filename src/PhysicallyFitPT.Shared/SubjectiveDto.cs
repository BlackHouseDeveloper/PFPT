// <copyright file="SubjectiveDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

/// <summary>
/// Data transfer object representing the subjective portion of a clinical note.
/// </summary>
public class SubjectiveDto
{
  /// <summary>
  /// Gets or sets the patient's chief complaint.
  /// </summary>
  public string? ChiefComplaint { get; set; }

  /// <summary>
  /// Gets or sets the history of present illness.
  /// </summary>
  public string? HistoryOfPresentIllness { get; set; }

  /// <summary>
  /// Gets or sets the pain locations as a CSV string.
  /// </summary>
  public string? PainLocationsCsv { get; set; }

  /// <summary>
  /// Gets or sets the pain severity rating (0-10 scale).
  /// </summary>
  public string? PainSeverity0to10 { get; set; }

  /// <summary>
  /// Gets or sets factors that aggravate symptoms.
  /// </summary>
  public string? AggravatingFactors { get; set; }

  /// <summary>
  /// Gets or sets factors that ease symptoms.
  /// </summary>
  public string? EasingFactors { get; set; }

  /// <summary>
  /// Gets or sets functional limitations experienced by the patient.
  /// </summary>
  public string? FunctionalLimitations { get; set; }

  /// <summary>
  /// Gets or sets the patient's narrative goals for treatment.
  /// </summary>
  public string? PatientGoalsNarrative { get; set; }
}
