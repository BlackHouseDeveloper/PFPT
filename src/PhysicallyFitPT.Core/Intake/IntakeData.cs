// <copyright file="IntakeData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core.Intake;

/// <summary>
/// Aggregate of patient intake information spanning demographics, medical history, insurance, and consents.
/// </summary>
public record IntakeData
{
  /// <summary>
  /// Gets or sets the unique identifier for the patient.
  /// </summary>
  public string? PatientId { get; set; }

  /// <summary>
  /// Gets or sets the patient demographic information.
  /// </summary>
  public PatientDemographics PatientInfo { get; set; } = new();

  /// <summary>
  /// Gets or sets the patient's medical history.
  /// </summary>
  public MedicalHistoryData MedicalHistory { get; set; } = new();

  /// <summary>
  /// Gets or sets the patient's insurance information.
  /// </summary>
  public InsuranceInfo Insurance { get; set; } = new();

  /// <summary>
  /// Gets or sets the patient's consent acknowledgements.
  /// </summary>
  public ConsentsData Consents { get; set; } = new();

  /// <summary>
  /// Gets or sets the timestamp when this intake was created.
  /// </summary>
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
