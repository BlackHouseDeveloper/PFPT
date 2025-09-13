// <copyright file="ObjectiveDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

using System.Collections.Generic;

/// <summary>
/// Data transfer object representing the objective portion of a clinical note.
/// </summary>
public class ObjectiveDto
{
    /// <summary>
    /// Gets or sets the range of motion measurements.
    /// </summary>
    public List<RomMeasureDto> Rom { get; set; } = new();

    /// <summary>
    /// Gets or sets the manual muscle test measurements.
    /// </summary>
    public List<MmtMeasureDto> Mmt { get; set; } = new();

    /// <summary>
    /// Gets or sets the special test results.
    /// </summary>
    public List<SpecialTestDto> SpecialTests { get; set; } = new();

    /// <summary>
    /// Gets or sets the outcome measure scores.
    /// </summary>
    public List<OutcomeMeasureScoreDto> OutcomeMeasures { get; set; } = new();

    /// <summary>
    /// Gets or sets the provided interventions.
    /// </summary>
    public List<ProvidedInterventionDto> ProvidedInterventions { get; set; } = new();
}
