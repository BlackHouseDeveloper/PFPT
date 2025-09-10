// <copyright file="ProvidedInterventionDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

using System;

/// <summary>
/// Data transfer object for provided interventions.
/// </summary>
public class ProvidedInterventionDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the provided intervention.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the CPT code for the intervention.
    /// </summary>
    public string CptCode { get; set; } = null!;

    /// <summary>
    /// Gets or sets the description of the intervention.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the number of units provided.
    /// </summary>
    public int Units { get; set; }

    /// <summary>
    /// Gets or sets the duration of the intervention in minutes.
    /// </summary>
    public int? Minutes { get; set; }
}
