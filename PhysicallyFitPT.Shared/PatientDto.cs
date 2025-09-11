// <copyright file="PatientDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  /// <summary>
  /// Represents a patient data transfer object.
  /// </summary>
  public class PatientDto
  {
    /// <summary>
    /// Gets or sets the unique identifier for the patient.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the Medical Record Number (MRN) for the patient.
    /// </summary>
    public string? MRN { get; set; }

    /// <summary>
    /// Gets or sets the patient's first name.
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the patient's last name.
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the patient's date of birth.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the patient's sex/gender.
    /// </summary>
    public string? Sex { get; set; }

    /// <summary>
    /// Gets or sets the patient's email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the patient's mobile phone number.
    /// </summary>
    public string? MobilePhone { get; set; }

    /// <summary>
    /// Gets or sets the patient's medications as comma-separated values.
    /// </summary>
    public string? MedicationsCsv { get; set; }

    /// <summary>
    /// Gets or sets the patient's comorbidities as comma-separated values.
    /// </summary>
    public string? ComorbiditiesCsv { get; set; }

    /// <summary>
    /// Gets or sets the patient's assistive devices as comma-separated values.
    /// </summary>
    public string? AssistiveDevicesCsv { get; set; }

    /// <summary>
    /// Gets or sets the patient's living situation description.
    /// </summary>
    public string? LivingSituation { get; set; }
  }
}
