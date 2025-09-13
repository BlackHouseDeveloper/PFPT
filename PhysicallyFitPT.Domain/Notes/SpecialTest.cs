// <copyright file="SpecialTest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain.Notes;

/// <summary>
/// Represents a special test performed during the physical examination.
/// </summary>
public sealed class SpecialTest
{
    /// <summary>
    /// Gets or sets the unique identifier for this special test.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the special test (e.g., "Lachman").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the side of the body being tested.
    /// </summary>
    public Side Side { get; set; } = Side.NA;

    /// <summary>
    /// Gets or sets the result of the special test.
    /// </summary>
    public SpecialTestResult Result { get; set; } = SpecialTestResult.NotPerformed;

    /// <summary>
    /// Gets or sets additional notes or observations about the test.
    /// </summary>
    public string? Notes { get; set; }
}
