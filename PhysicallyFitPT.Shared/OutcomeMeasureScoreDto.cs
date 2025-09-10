// <copyright file="OutcomeMeasureScoreDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

using System;

/// <summary>
/// Data transfer object for outcome measure scores.
/// </summary>
public class OutcomeMeasureScoreDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the outcome measure score.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the outcome measurement instrument.
    /// </summary>
    public string Instrument { get; set; } = null!;

    /// <summary>
    /// Gets or sets the raw score value.
    /// </summary>
    public int? RawScore { get; set; }

    /// <summary>
    /// Gets or sets the percentage score.
    /// </summary>
    public double? Percent { get; set; }

    /// <summary>
    /// Gets or sets the date when the score was collected.
    /// </summary>
    public DateTime? CollectedOn { get; set; }
}
