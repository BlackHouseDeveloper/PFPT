using System.ComponentModel.DataAnnotations;
using PhysicallyFitPT.Domain.Validation;

namespace PhysicallyFitPT.Domain;

public class Patient : Entity
{
  [StringLength(40)]
  public string? MRN { get; set; }
  
  [Required]
  [StringLength(60)]
  public string FirstName { get; set; } = null!;
  
  [Required]
  [StringLength(60)]
  public string LastName { get; set; } = null!;
  
  [DateRangeValidation("1900-01-01", "2024-12-31")]
  public DateTime? DateOfBirth { get; set; }
  
  public string? Sex { get; set; }
  
  [EmailAddressValidation]
  [StringLength(120)]
  public string? Email { get; set; }
  
  [PhoneNumberValidation]
  [StringLength(30)]
  public string? MobilePhone { get; set; }
  
  public string? MedicationsCsv { get; set; }
  public string? ComorbiditiesCsv { get; set; }
  public string? AssistiveDevicesCsv { get; set; }
  public string? LivingSituation { get; set; }
  
  public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
