// <copyright file="AssessmentSection.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain.Notes;

/// <summary>
/// Represents the assessment section of a clinical note containing clinical judgments and diagnoses.
/// </summary>
public sealed class AssessmentSection
{
    /// <summary>
    /// Gets or sets the clinician's clinical impression of the patient's condition.
    /// </summary>
    public string? ClinicalImpression { get; set; }

    /// <summary>
    /// Gets or sets the assessment of the patient's rehabilitation potential.
    /// </summary>
    public string? RehabPotential { get; set; }

    /// <summary>
    /// Gets or sets the ICD-10 diagnosis codes associated with this assessment.
    /// </summary>
    public List<Icd10Link> Icd10Codes { get; set; } = new();

    /// <summary>
    /// Gets or sets the treatment goals established for the patient.
    /// </summary>
    public List<Goal> Goals { get; set; } = new();
}
