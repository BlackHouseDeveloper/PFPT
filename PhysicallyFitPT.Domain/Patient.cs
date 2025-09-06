namespace PhysicallyFitPT.Domain;

public class Patient : Entity
{
  public string? MRN { get; set; }
  public string FirstName { get; set; } = null!;
  public string LastName { get; set; } = null!;
  public DateTime? DateOfBirth { get; set; }
  public string? Sex { get; set; }
  public string? Email { get; set; }
  public string? MobilePhone { get; set; }
  public string? MedicationsCsv { get; set; }
  public string? ComorbiditiesCsv { get; set; }
  public string? AssistiveDevicesCsv { get; set; }
  public string? LivingSituation { get; set; }
  public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
