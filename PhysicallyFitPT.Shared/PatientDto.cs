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

    public DateTime? DateOfBirth { get; set; }

    public string? Sex { get; set; }

    public string? Email { get; set; }

    public string? MobilePhone { get; set; }

    public string? MedicationsCsv { get; set; }

    public string? ComorbiditiesCsv { get; set; }

    public string? AssistiveDevicesCsv { get; set; }

    public string? LivingSituation { get; set; }
  }
}
