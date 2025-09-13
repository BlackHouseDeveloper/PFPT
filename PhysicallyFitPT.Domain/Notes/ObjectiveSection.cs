// <copyright file="ObjectiveSection.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain.Notes;

/// <summary>
/// Represents the objective section of a clinical note containing measurable physical findings.
/// </summary>
public sealed class ObjectiveSection
{
    /// <summary>
    /// Gets or sets the range of motion measurements for the patient.
    /// </summary>
    public List<RomMeasure> Rom { get; set; } = new();

    /// <summary>
    /// Gets or sets the manual muscle test measurements for the patient.
    /// </summary>
    public List<MmtMeasure> Mmt { get; set; } = new();

    /// <summary>
    /// Gets or sets the special tests performed during the evaluation.
    /// </summary>
    public List<SpecialTest> SpecialTests { get; set; } = new();

    /// <summary>
    /// Gets or sets the outcome measures and scores collected during the visit.
    /// </summary>
    public List<OutcomeMeasureScore> OutcomeMeasures { get; set; } = new();

    /// <summary>
    /// Gets or sets the interventions provided during the treatment session.
    /// </summary>
    public List<ProvidedIntervention> ProvidedInterventions { get; set; } = new();
}
