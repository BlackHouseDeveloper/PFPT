// <copyright file="Note.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

using PhysicallyFitPT.Domain.Notes;

// Note + Sections (annotation-free POCOs)

/// <summary>
/// Represents a clinical note in the physical therapy system.
/// </summary>
public sealed class Note
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
  /// Gets or sets the patient associated with this note.
  /// </summary>
  public Patient? Patient { get; set; }

  /// <summary>
  /// Gets or sets the appointment identifier associated with this note.
  /// </summary>
  public Guid AppointmentId { get; set; }

  /// <summary>
  /// Gets or sets the appointment associated with this note.
  /// </summary>
  public Appointment? Appointment { get; set; }

  /// <summary>
  /// Gets or sets the type of visit for this note.
  /// </summary>
  public VisitType VisitType { get; set; }

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

  // Owned sections (configured in DbContext)

  /// <summary>
  /// Gets or sets the subjective section of the note containing patient-reported information.
  /// </summary>
  public SubjectiveSection Subjective { get; set; } = new();

  /// <summary>
  /// Gets or sets the objective section of the note containing measurable findings.
  /// </summary>
  public ObjectiveSection Objective { get; set; } = new();

  /// <summary>
  /// Gets or sets the assessment section of the note containing clinical judgment.
  /// </summary>
  public AssessmentSection Assessment { get; set; } = new();

  /// <summary>
  /// Gets or sets the plan section of the note containing treatment plans and goals.
  /// </summary>
  public PlanSection Plan { get; set; } = new();

  // Auditing / soft-delete

  /// <summary>
  /// Gets or sets the date and time when the note was created.
  /// </summary>
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

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

  /// <summary>
  /// Gets or sets a value indicating whether the note is marked as deleted.
  /// </summary>
  public bool IsDeleted { get; set; }
}
