namespace PhysicallyFitPT.Domain;

public enum GoalStatus
{
    NotStarted = 0,
    InProgress = 1,
    Met = 2,
    PartiallyMet = 3,
    NotMet = 4
}

public sealed class Goal
{
    public Guid Id { get; set; }

    public bool IsLongTerm { get; set; }

    // 500-char max enforced in DB; Domain is annotation-free by design.
    public string Description { get; set; } = string.Empty;

    // Optional measurement metadata
    public string? MeasureType { get; set; }

    public string? BaselineValue { get; set; }

    public string? TargetValue { get; set; }

    public DateTime? TargetDate { get; set; }

    public GoalStatus Status { get; set; } = GoalStatus.NotStarted;
}
