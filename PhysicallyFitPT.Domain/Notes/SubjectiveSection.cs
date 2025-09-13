// <copyright file="SubjectiveSection.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain.Notes;

/// <summary>
/// Represents the subjective section of a clinical note containing patient-reported information.
/// </summary>
public sealed class SubjectiveSection
{
    /// <summary>
    /// Gets or sets the primary complaint or reason for the patient's visit.
    /// </summary>
    public string? ChiefComplaint { get; set; }

    /// <summary>
    /// Gets or sets the detailed history of the patient's current condition.
    /// </summary>
    public string? HistoryOfPresentIllness { get; set; }

    /// <summary>
    /// Gets or sets the pain locations as comma-separated values.
    /// Keep as CSV to match current migration and DTOs.
    /// </summary>
    public string? PainLocationsCsv { get; set; }

    /// <summary>
    /// Gets or sets the pain severity rating on a scale of 0 to 10.
    /// We use a string for flexibility (single or composite "X/10" inputs).
    /// </summary>
    public string? PainSeverity0to10 { get; set; }

    /// <summary>
    /// Gets or sets factors that worsen or aggravate the patient's symptoms.
    /// </summary>
    public string? AggravatingFactors { get; set; }

    /// <summary>
    /// Gets or sets factors that relieve or ease the patient's symptoms.
    /// </summary>
    public string? EasingFactors { get; set; }

    /// <summary>
    /// Gets or sets the functional limitations experienced by the patient.
    /// </summary>
    public string? FunctionalLimitations { get; set; }

    /// <summary>
    /// Gets or sets the narrative description of the patient's goals for treatment.
    /// </summary>
    public string? PatientGoalsNarrative { get; set; }
}
