// <copyright file="NoteDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  /// <summary>
  /// Data transfer object representing a clinical note summary.
  /// </summary>
  public class NoteDto
  {
    /// <summary>
    /// Gets or sets the unique identifier for the note.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the patient identifier associated with this note.
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// Gets or sets the appointment identifier associated with this note.
    /// </summary>
    public Guid AppointmentId { get; set; }

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
    /// Gets or sets the date and time when the note was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of who created the note.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the note was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of who last updated the note.
    /// </summary>
    public string? UpdatedBy { get; set; }
  }
}
