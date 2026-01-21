// <copyright file="CptCodeDetail.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core;

/// <summary>
/// Represents a CPT (Current Procedural Terminology) code detail for billing and documentation.
/// </summary>
public class CptCodeDetail
{
    /// <summary>
    /// Gets or sets the CPT code (e.g., 97110).
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Gets or sets the description of the procedure.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Gets or sets the minimum time in minutes for time-based codes.
    /// </summary>
    public int? TimeMin { get; set; }

    /// <summary>
    /// Gets or sets the category (e.g., Exercise, Manual, Modality).
    /// </summary>
    public required string Category { get; set; }

    /// <summary>
    /// Gets or sets search keywords for finding this code.
    /// </summary>
    public required List<string> Keywords { get; set; }
}
