namespace PhysicallyFitPT.Shared;

public static class GoalTemplates
{
    public static readonly Dictionary<string, List<string>> BodyRegionGoals = new()
    {
        ["Neck"] = new()
        {
            "Patient will perform chin tucks 2x/day with correct form within 2 weeks.",
            "Patient will turn head ≥60° bilaterally without pain in 3 weeks.",
            "Patient will don overhead shirt without discomfort in 4 weeks."
        },
        ["LowBack"] = new()
        {
            "Patient will sit for 30 minutes without low back pain in 3 weeks.",
            "Patient will transition sit-to-stand with minimal discomfort within 2 weeks.",
            "Patient will sleep through the night without waking due to back pain within 4 weeks."
        },
        ["LE"] = new()
        {
            "Patient will ascend 1 flight of stairs without rail support in 4 weeks.",
            "Patient will perform 5 sit-to-stands in 30 seconds without assist.",
            "Patient will walk 500 feet pain-free in community setting within 6 weeks."
        },
        ["UE"] = new()
        {
            "Patient will reach overhead with <2/10 pain for 5 reps within 3 weeks.",
            "Patient will carry 10 lbs for 50 ft without upper extremity fatigue in 4 weeks.",
            "Patient will brush or style hair independently without compensation in 2 weeks."
        },
        ["Pelvic"] = new()
        {
            "Patient will sit-to-stand from toilet with minimal pelvic discomfort in 3 weeks.",
            "Patient will stand for 15 minutes without flare-up by week 4.",
            "Patient will report 50% reduction in urgency episodes with HEP adherence by week 6."
        }
    };
}
