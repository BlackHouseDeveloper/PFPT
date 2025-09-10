// <copyright file="Patient.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

/// <summary>
/// Represents a patient in the physical therapy system.
/// </summary>
public sealed class Patient
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
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the patient's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the patient's date of birth.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the patient's biological sex.
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
    /// Gets or sets the patient's medications as a comma-separated string.
    /// </summary>
    public string? MedicationsCsv { get; set; }

    /// <summary>
    /// Gets or sets the patient's comorbidities as a comma-separated string.
    /// </summary>
    public string? ComorbiditiesCsv { get; set; }

    /// <summary>
    /// Gets or sets the patient's assistive devices as a comma-separated string.
    /// </summary>
    public string? AssistiveDevicesCsv { get; set; }

    /// <summary>
    /// Gets or sets the patient's living situation description.
    /// </summary>
    public string? LivingSituation { get; set; }

    // Auditing / soft-delete (kept annotation-free)
    /// <summary>
    /// Gets or sets the timestamp when the patient record was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the identifier of the user who created the patient record.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the patient record was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last updated the patient record.
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the patient record is soft-deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the list of appointments associated with this patient.
    /// </summary>
    public List<Appointment> Appointments { get; set; } = new();
}
