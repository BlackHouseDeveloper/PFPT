// <copyright file="AssessmentDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

using System.Collections.Generic;

/// <summary>
/// Data transfer object representing the assessment portion of a clinical note.
/// </summary>
public class AssessmentDto
{
    /// <summary>
    /// Gets or sets the clinician's clinical impression.
    /// </summary>
    public string? ClinicalImpression { get; set; }

    /// <summary>
    /// Gets or sets the patient's rehabilitation potential.
    /// </summary>
    public string? RehabPotential { get; set; }

    /// <summary>
    /// Gets or sets the ICD-10 diagnostic codes.
    /// </summary>
    public List<Icd10LinkDto> Icd10Codes { get; set; } = new();

    /// <summary>
    /// Gets or sets the treatment goals.
    /// </summary>
    public List<GoalDto> Goals { get; set; } = new();
}
