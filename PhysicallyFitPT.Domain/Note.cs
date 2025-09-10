// <copyright file="Note.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

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

// S
public sealed class SubjectiveSection
{
  public string? ChiefComplaint { get; set; }

  public string? HistoryOfPresentIllness { get; set; }

  // Keep as CSV to match current migration and DTOs
  public string? PainLocationsCsv { get; set; }

  // We use a string for flexibility (single or composite “X/10” inputs)
  public string? PainSeverity0to10 { get; set; }

  public string? AggravatingFactors { get; set; }

  public string? EasingFactors { get; set; }

  public string? FunctionalLimitations { get; set; }

  public string? PatientGoalsNarrative { get; set; }
}

// O
public sealed class ObjectiveSection
{
  public List<RomMeasure> Rom { get; set; } = new();

  public List<MmtMeasure> Mmt { get; set; } = new();

  public List<SpecialTest> SpecialTests { get; set; } = new();

  public List<OutcomeMeasureScore> OutcomeMeasures { get; set; } = new();

  public List<ProvidedIntervention> ProvidedInterventions { get; set; } = new();
}

public sealed class RomMeasure
{
  public Guid Id { get; set; }

  public string Joint { get; set; } = string.Empty;     // e.g., “Knee”

  public string Movement { get; set; } = string.Empty;  // e.g., “Flexion”

  public Side Side { get; set; } = Side.NA;

  public int? MeasuredDegrees { get; set; }

  public int? NormalDegrees { get; set; }

  public bool WithPain { get; set; }

  public string? Notes { get; set; }
}

public sealed class MmtMeasure
{
  public Guid Id { get; set; }

  public string MuscleGroup { get; set; } = string.Empty; // e.g., “Quadriceps”

  public Side Side { get; set; } = Side.NA;

  public string Grade { get; set; } = "3/5";

  public bool WithPain { get; set; }

  public string? Notes { get; set; }
}

public sealed class SpecialTest
{
  public Guid Id { get; set; }

  public string Name { get; set; } = string.Empty; // e.g., “Lachman”

  public Side Side { get; set; } = Side.NA;

  public SpecialTestResult Result { get; set; } = SpecialTestResult.NotPerformed;

  public string? Notes { get; set; }
}

public sealed class OutcomeMeasureScore
{
  public Guid Id { get; set; }

  public string Instrument { get; set; } = string.Empty; // e.g., “LEFS”

  public int? RawScore { get; set; }

  public double? Percent { get; set; }

  public DateTime? CollectedOn { get; set; }
}

public sealed class ProvidedIntervention
{
  public Guid Id { get; set; }

  public string CptCode { get; set; } = string.Empty;

  public string? Description { get; set; }

  public int Units { get; set; }

  public int? Minutes { get; set; }
}

// A
public sealed class AssessmentSection
{
  public string? ClinicalImpression { get; set; }

  public string? RehabPotential { get; set; }

  public List<Icd10Link> Icd10Codes { get; set; } = new();

  public List<Goal> Goals { get; set; } = new();
}

public sealed class Icd10Link
{
  public Guid Id { get; set; }

  public string Code { get; set; } = string.Empty;

  public string? Description { get; set; }
}

// P
public sealed class PlanSection
{
  public string? Frequency { get; set; }

  public string? Duration { get; set; }

  public string? PlannedInterventionsCsv { get; set; }

  public string? NextVisitFocus { get; set; }

  public List<ExercisePrescription> Hep { get; set; } = new();
}

public sealed class ExercisePrescription
{
  public Guid Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public string? Dosage { get; set; }

  public string? Notes { get; set; }
}
