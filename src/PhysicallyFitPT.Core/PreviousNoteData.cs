// <copyright file="PreviousNoteData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core;

/// <summary>
/// Mock data representing a previous patient note for carry-forward functionality.
/// In production, this would be retrieved from the database.
/// </summary>
public class PreviousNoteData
{
    /// <summary>
    /// Gets or sets the patient's chief complaint.
    /// </summary>
    public required string ChiefComplaint { get; set; }

    /// <summary>
    /// Gets or sets the pain level (0-10 scale).
    /// </summary>
    public required List<int> PainLevel { get; set; }

    /// <summary>
    /// Gets or sets the primary pain location.
    /// </summary>
    public required string PainLocation { get; set; }

    /// <summary>
    /// Gets or sets the pain description.
    /// </summary>
    public required string PainDescription { get; set; }

    /// <summary>
    /// Gets or sets functional limitations (comma-separated).
    /// </summary>
    public required string Limitations { get; set; }

    /// <summary>
    /// Gets or sets range of motion measurements.
    /// </summary>
    public required string Rom { get; set; }

    /// <summary>
    /// Gets or sets manual muscle testing results.
    /// </summary>
    public required string Mmt { get; set; }

    /// <summary>
    /// Gets or sets the clinical diagnosis.
    /// </summary>
    public required string Diagnosis { get; set; }

    /// <summary>
    /// Gets or sets patient goals.
    /// </summary>
    public required List<string> Goals { get; set; }
}
