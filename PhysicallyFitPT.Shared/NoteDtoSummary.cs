// <copyright file="NoteDtoSummary.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  /// <summary>
  /// Represents a summary view of a clinical note for display purposes.
  /// </summary>
  public class NoteDtoSummary
  {
    /// <summary>
    /// Gets or sets the unique identifier for the note.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the type of visit for this note.
    /// </summary>
    public string VisitType { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether this note has been signed.
    /// </summary>
    public bool IsSigned { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the note was signed.
    /// </summary>
    public DateTimeOffset? SignedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of who signed the note.
    /// </summary>
    public string? SignedBy { get; set; }

    /// <summary>
    /// Gets or sets the date of the encounter (appointment scheduled start or note creation).
    /// </summary>
    public DateTimeOffset Date { get; set; }
  }
}
