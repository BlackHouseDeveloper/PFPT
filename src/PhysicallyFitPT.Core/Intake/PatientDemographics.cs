// <copyright file="PatientDemographics.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core.Intake;

/// <summary>
/// Patient demographic details collected during intake.
/// </summary>
public record PatientDemographics
{
  /// <summary>
  /// Gets or sets the patient's first name.
  /// </summary>
  public string? FirstName { get; set; }

  /// <summary>
  /// Gets or sets the patient's last name.
  /// </summary>
  public string? LastName { get; set; }

  /// <summary>
  /// Gets or sets the patient's date of birth.
  /// </summary>
  public DateOnly? DateOfBirth { get; set; }

  /// <summary>
  /// Gets or sets the patient's gender.
  /// </summary>
  public string? Gender { get; set; }

  /// <summary>
  /// Gets or sets the patient's phone number.
  /// </summary>
  public string? Phone { get; set; }

  /// <summary>
  /// Gets or sets the patient's email address.
  /// </summary>
  public string? Email { get; set; }

  /// <summary>
  /// Gets or sets the patient's street address.
  /// </summary>
  public string? Address { get; set; }

  /// <summary>
  /// Gets or sets the patient's city.
  /// </summary>
  public string? City { get; set; }

  /// <summary>
  /// Gets or sets the patient's state or province.
  /// </summary>
  public string? State { get; set; }

  /// <summary>
  /// Gets or sets the patient's postal code.
  /// </summary>
  public string? ZipCode { get; set; }

  /// <summary>
  /// Gets or sets the emergency contact's name.
  /// </summary>
  public string? EmergencyContactName { get; set; }

  /// <summary>
  /// Gets or sets the emergency contact's phone number.
  /// </summary>
  public string? EmergencyContactPhone { get; set; }
}
