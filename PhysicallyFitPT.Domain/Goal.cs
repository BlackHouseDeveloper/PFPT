using System.ComponentModel.DataAnnotations;
using PhysicallyFitPT.Domain.Validation;

namespace PhysicallyFitPT.Domain;

public class Goal
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public bool IsLongTerm { get; set; }
  
  [Required]
  [StringLength(500)]
  public string Description { get; set; } = null!;
  
  public string? MeasureType { get; set; }
  public string? BaselineValue { get; set; }
  public string? TargetValue { get; set; }
  public DateTime? TargetDate { get; set; }
  public GoalStatus Status { get; set; } = GoalStatus.Active;
}
