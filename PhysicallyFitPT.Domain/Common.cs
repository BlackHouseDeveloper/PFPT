namespace PhysicallyFitPT.Domain;

public abstract class Entity
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public string? CreatedBy { get; set; }
  public DateTimeOffset? UpdatedAt { get; set; }
  public string? UpdatedBy { get; set; }
  public bool IsDeleted { get; set; }
}

public enum VisitType { Eval = 0, Daily = 1, Progress = 2, Discharge = 3 }
public enum Side { NA = 0, Left = 1, Right = 2, Bilateral = 3 }
public enum SpecialTestResult { NotPerformed = 0, Negative = 1, Positive = 2, Inconclusive = 3 }
public enum DeliveryMethod { SMS = 0, Email = 1 }
public enum QuestionnaireType { Eval = 0, Daily = 1, Progress = 2, Discharge = 3, BodyPartSpecific = 4 }
public enum GoalStatus { Active = 0, Met = 1, PartiallyMet = 2, NotMet = 3, Deferred = 4 }
