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

/// <summary>
/// Represents the subjective section of a clinical note containing patient-reported information.
/// </summary>
public sealed class SubjectiveSection
{
  /// <summary>
  /// Gets or sets the primary complaint or reason for the patient's visit.
  /// </summary>
  public string? ChiefComplaint { get; set; }

  /// <summary>
  /// Gets or sets the detailed history of the patient's current condition.
  /// </summary>
  public string? HistoryOfPresentIllness { get; set; }

  /// <summary>
  /// Gets or sets the pain locations as comma-separated values.
  /// Keep as CSV to match current migration and DTOs.
  /// </summary>
  public string? PainLocationsCsv { get; set; }

  // We use a string for flexibility (single or composite “X/10” inputs)
  /// <summary>
  /// Gets or sets the pain severity rating on a scale of 0 to 10.
  /// We use a string for flexibility (single or composite "X/10" inputs).
  /// </summary>
  public string? PainSeverity0to10 { get; set; }

  public string? AggravatingFactors { get; set; }

  /// <summary>
  /// Gets or sets factors that worsen or aggravate the patient's symptoms.
  /// </summary>
  public string? EasingFactors { get; set; }

  /// <summary>
  /// Gets or sets factors that relieve or ease the patient's symptoms.
  /// </summary>
  public string? FunctionalLimitations { get; set; }

  /// <summary>
  /// Gets or sets the functional limitations experienced by the patient.
  /// </summary>
  public string? PatientGoalsNarrative { get; set; }
}

/// <summary>
/// Represents the objective section of a clinical note containing measurable physical findings.
/// </summary>
public sealed class ObjectiveSection
{
  /// <summary>
  /// Gets or sets the range of motion measurements for the patient.
  /// </summary>
  public List<RomMeasure> Rom { get; set; } = new();

  /// <summary>
  /// Gets or sets the manual muscle test measurements for the patient.
  /// </summary>
  public List<MmtMeasure> Mmt { get; set; } = new();

  /// <summary>
  /// Gets or sets the special tests performed during the evaluation.
  /// </summary>
  public List<SpecialTest> SpecialTests { get; set; } = new();

  /// <summary>
  /// Gets or sets the outcome measures and scores collected during the visit.
  /// </summary>
  public List<OutcomeMeasureScore> OutcomeMeasures { get; set; } = new();

  /// <summary>
  /// Gets or sets the interventions provided during the treatment session.
  /// </summary>
  public List<ProvidedIntervention> ProvidedInterventions { get; set; } = new();
}

/// <summary>
/// Represents a range of motion measurement for a specific joint and movement.
/// </summary>
public sealed class RomMeasure
{
  /// <summary>
  /// Gets or sets the unique identifier for this ROM measurement.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the joint being measured (e.g., "Knee").
  /// </summary>
  public string Joint { get; set; } = string.Empty;     // e.g., “Knee”

  /// <summary>
  /// Gets or sets the movement being measured (e.g., "Flexion").
  /// </summary>
  public string Movement { get; set; } = string.Empty;  // e.g., “Flexion”

  public Side Side { get; set; } = Side.NA;
  /// <summary>
  /// Gets or sets the side of the body being measured.
  /// </summary>

  public int? MeasuredDegrees { get; set; }
  /// <summary>
  /// Gets or sets the measured range of motion in degrees.
  /// </summary>

  public int? NormalDegrees { get; set; }
  /// <summary>
  /// Gets or sets the normal range of motion in degrees for comparison.
  /// </summary>

  public bool WithPain { get; set; }
  /// <summary>
  /// Gets or sets a value indicating whether pain was present during the measurement.
  /// </summary>

  public string? Notes { get; set; }
  /// <summary>
  /// Gets or sets additional notes or observations about the measurement.
  /// </summary>
}

/// <summary>
/// Represents a manual muscle test measurement for a specific muscle group.
/// </summary>
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
