namespace PhysicallyFitPT.Domain;

public class Note : Entity
{
  public Guid PatientId { get; set; }
  public Patient Patient { get; set; } = null!;
  public Guid AppointmentId { get; set; }
  public Appointment Appointment { get; set; } = null!;
  public VisitType VisitType { get; set; }

  public SubjectiveSection Subjective { get; set; } = new();
  public ObjectiveSection Objective { get; set; } = new();
  public AssessmentSection Assessment { get; set; } = new();
  public PlanSection Plan { get; set; } = new();

  public bool IsSigned { get; set; }
  public DateTimeOffset? SignedAt { get; set; }
  public string? SignedBy { get; set; }
}

public class SubjectiveSection
{
  public string? ChiefComplaint { get; set; }
  public string? HistoryOfPresentIllness { get; set; }
  public string? PainLocationsCsv { get; set; }
  public string? PainSeverity0to10 { get; set; }
  public string? AggravatingFactors { get; set; }
  public string? EasingFactors { get; set; }
  public string? FunctionalLimitations { get; set; }
  public string? PatientGoalsNarrative { get; set; }
}

public class ObjectiveSection
{
  public List<RomMeasure> Rom { get; set; } = new();
  public List<MmtMeasure> Mmt { get; set; } = new();
  public List<SpecialTest> SpecialTests { get; set; } = new();
  public List<OutcomeMeasureScore> OutcomeMeasures { get; set; } = new();
  public List<ProvidedIntervention> ProvidedInterventions { get; set; } = new();
}

public class RomMeasure
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Joint { get; set; } = "Knee";
  public string Movement { get; set; } = "Flexion";
  public Side Side { get; set; } = Side.NA;
  public int? MeasuredDegrees { get; set; }
  public int? NormalDegrees { get; set; }
  public bool WithPain { get; set; }
  public string? Notes { get; set; }
}

public class MmtMeasure
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string MuscleGroup { get; set; } = "Quadriceps";
  public Side Side { get; set; } = Side.NA;
  public string Grade { get; set; } = "4+";
  public bool WithPain { get; set; }
  public string? Notes { get; set; }
}

public class SpecialTest
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Name { get; set; } = "Lachman";
  public Side Side { get; set; } = Side.NA;
  public SpecialTestResult Result { get; set; } = SpecialTestResult.NotPerformed;
  public string? Notes { get; set; }
}

public class OutcomeMeasureScore
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Instrument { get; set; } = "LEFS";
  public int? RawScore { get; set; }
  public double? Percent { get; set; }
  public DateTime? CollectedOn { get; set; }
}

public class ProvidedIntervention
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string CptCode { get; set; } = null!;
  public string? Description { get; set; }
  public int Units { get; set; }
  public int? Minutes { get; set; }
}

public class AssessmentSection
{
  public string? ClinicalImpression { get; set; }
  public string? RehabPotential { get; set; }
  public List<Icd10Link> Icd10Codes { get; set; } = new();
  public List<Goal> Goals { get; set; } = new();
}

public class Icd10Link
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Code { get; set; } = null!;
  public string? Description { get; set; }
}

public class PlanSection
{
  public string? Frequency { get; set; }
  public string? Duration { get; set; }
  public string? PlannedInterventionsCsv { get; set; }
  public List<ExercisePrescription> Hep { get; set; } = new();
  public string? NextVisitFocus { get; set; }
}

public class ExercisePrescription
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Name { get; set; } = null!;
  public string? Dosage { get; set; }
  public string? Notes { get; set; }
}
