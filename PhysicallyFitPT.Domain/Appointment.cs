namespace PhysicallyFitPT.Domain;

public class Appointment : Entity
{
  public Guid PatientId { get; set; }
  public Patient Patient { get; set; } = null!;
  public VisitType VisitType { get; set; }
  public DateTimeOffset ScheduledStart { get; set; }
  public DateTimeOffset? ScheduledEnd { get; set; }
  public string? Location { get; set; }
  public string? ClinicianNpi { get; set; }
  public string? ClinicianName { get; set; }
  public DateTimeOffset? QuestionnaireSentAt { get; set; }
  public DateTimeOffset? QuestionnaireCompletedAt { get; set; }
  public bool IsCheckedIn { get; set; }
  public Note? Note { get; set; }
}
