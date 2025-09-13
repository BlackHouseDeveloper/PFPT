// <copyright file="PlanSection.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain.Notes;

/// <summary>
/// Represents the plan section of a clinical note containing treatment plans and recommendations.
/// </summary>
public sealed class PlanSection
{
    /// <summary>
    /// Gets or sets the frequency of treatment sessions (e.g., "3x/week").
    /// </summary>
    public string? Frequency { get; set; }

    /// <summary>
    /// Gets or sets the expected duration of treatment (e.g., "4-6 weeks").
    /// </summary>
    public string? Duration { get; set; }

    /// <summary>
    /// Gets or sets the planned interventions as comma-separated values.
    /// </summary>
    public string? PlannedInterventionsCsv { get; set; }

    /// <summary>
    /// Gets or sets the focus areas for the next visit.
    /// </summary>
    public string? NextVisitFocus { get; set; }

    /// <summary>
    /// Gets or sets the home exercise program prescriptions.
    /// </summary>
    public List<ExercisePrescription> Hep { get; set; } = new();
}
