// <copyright file="OutcomeMeasureScore.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain.Notes;

/// <summary>
/// Represents a scored outcome measure assessment.
/// </summary>
public sealed class OutcomeMeasureScore
{
    /// <summary>
    /// Gets or sets the unique identifier for this outcome measure score.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the outcome measure instrument (e.g., "LEFS").
    /// </summary>
    public string Instrument { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the raw score obtained on the instrument.
    /// </summary>
    public int? RawScore { get; set; }

    /// <summary>
    /// Gets or sets the percentage score or normalized score.
    /// </summary>
    public double? Percent { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the outcome measure was collected.
    /// </summary>
    public DateTime? CollectedOn { get; set; }
}
