// <copyright file="MedicalHistoryData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core.Intake;

/// <summary>
/// Medical history information captured for a patient during intake.
/// </summary>
public record MedicalHistoryData
{
    /// <summary>
    /// Gets or sets the list of chief complaints.
    /// </summary>
    public List<string> ChiefComplaints { get; set; } = new();

    /// <summary>
    /// Gets or sets the date when symptoms began.
    /// </summary>
    public string? OnsetDate { get; set; }

    /// <summary>
    /// Gets or sets the mechanism of injury.
    /// </summary>
    public string? Mechanism { get; set; }

    /// <summary>
    /// Gets or sets the list of past medical conditions.
    /// </summary>
    public List<string> PastMedicalHistory { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of current medications.
    /// </summary>
    public List<string> Medications { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the patient has surgical history.
    /// </summary>
    public bool HasSurgicalHistory { get; set; }

    /// <summary>
    /// Gets or sets the details of past surgeries.
    /// </summary>
    public string? SurgicalHistory { get; set; }

    /// <summary>
    /// Gets or sets the patient's known allergies.
    /// </summary>
    public string? Allergies { get; set; }
}
