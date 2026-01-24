using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PhysicallyFitPT.Infrastructure.Services;

public sealed class PocCalculationService
{
  private static readonly Regex Icd10Regex = new("^[A-Z]\\d{2}\\.?[A-Z0-9]{0,4}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

  public PocCalculationResult CalculateProgress(PocCalculationInput input, DateOnly? todayOverride = null)
  {
    var today = todayOverride ?? DateOnly.FromDateTime(DateTime.UtcNow);
    var start = input.StartDate;
    var end = input.EndDate < input.StartDate ? input.StartDate : input.EndDate;

    var totalDays = Math.Max(0, (int)Math.Floor((end.ToDateTime(TimeOnly.MinValue) - start.ToDateTime(TimeOnly.MinValue)).TotalDays));
    var totalWeeks = Math.Max(0, (int)Math.Ceiling(totalDays / 7.0));

    var elapsedDaysRaw = (int)Math.Floor((today.ToDateTime(TimeOnly.MinValue) - start.ToDateTime(TimeOnly.MinValue)).TotalDays);
    var daysElapsed = Math.Max(0, elapsedDaysRaw);
    var weeksElapsed = Math.Max(0, (int)Math.Floor((double)daysElapsed / 7.0));
    var currentWeek = totalWeeks == 0 ? 0 : Math.Min(weeksElapsed + 1, totalWeeks);

    var daysRemaining = Math.Max(0, (int)Math.Floor((end.ToDateTime(TimeOnly.MinValue) - today.ToDateTime(TimeOnly.MinValue)).TotalDays));
    var weeksRemaining = Math.Max(0, (int)Math.Ceiling(daysRemaining / 7.0));

    var expectedVisitsByNow = (int)Math.Floor((double)(weeksElapsed * input.Frequency));
    var visitsPaceAhead = input.VisitsCompleted - expectedVisitsByNow;
    var isOnTrack = visitsPaceAhead >= -1;

    var utilizationRate = input.TotalVisitsAuthorized == 0
        ? 0d
        : (double)input.VisitsCompleted / input.TotalVisitsAuthorized * 100d;

    var averageVisitsPerWeek = input.Frequency <= 0
        ? 0
        : weeksElapsed > 0
            ? input.VisitsCompleted / (double)weeksElapsed
            : input.Frequency;

    var remainingVisits = Math.Max(0, input.TotalVisitsAuthorized - input.VisitsCompleted);
    var weeksToComplete = averageVisitsPerWeek <= 0
        ? 0
        : (int)Math.Ceiling(remainingVisits / averageVisitsPerWeek);

    var projectedCompletionDate = today.AddDays(weeksToComplete * 7);
    var projectedVisitsAtCompletion = (int)Math.Round(input.VisitsCompleted + (weeksRemaining * averageVisitsPerWeek));
    var willExceedAuthorization = projectedVisitsAtCompletion > input.TotalVisitsAuthorized;

    return new PocCalculationResult(
        TotalWeeks: totalWeeks,
        CurrentWeek: currentWeek,
        WeeksRemaining: weeksRemaining,
        WeeksElapsed: weeksElapsed,
        DaysRemaining: daysRemaining,
        TotalDays: totalDays,
        VisitsPerWeek: input.Frequency,
        ExpectedVisitsByNow: expectedVisitsByNow,
        VisitsPaceAhead: visitsPaceAhead,
        ProjectedCompletionDate: projectedCompletionDate,
        IsOnTrack: isOnTrack,
        UtilizationRate: Math.Round(utilizationRate, 1, MidpointRounding.AwayFromZero),
        ProjectedVisitsAtCompletion: projectedVisitsAtCompletion,
        WillExceedAuthorization: willExceedAuthorization);
  }

  public int CalculateWeeksRemainingByVisits(int visitsRemaining, int frequency)
  {
    if (frequency <= 0)
    {
      return 0;
    }

    return (int)Math.Ceiling(Math.Max(0, visitsRemaining) / (double)frequency);
  }

  public bool ValidateIcd10Code(string code)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      return false;
    }

    return Icd10Regex.IsMatch(code.Trim().ToUpperInvariant());
  }

  public bool HasRequiredIcd10Codes(IEnumerable<IcdCodeEntry> codes)
  {
    var codeList = codes?.ToList() ?? new List<IcdCodeEntry>();
    return codeList.Count >= 2 && codeList.All(c => ValidateIcd10Code(c.Code));
  }

  public PocStatus CalculatePocStatus(int weeksRemaining, int visitsRemaining, int totalVisitsAuthorized)
  {
    if (visitsRemaining <= 0)
    {
      return PocStatus.Completed;
    }

    if (weeksRemaining <= 0)
    {
      return PocStatus.Expired;
    }

    if (weeksRemaining <= 2 || visitsRemaining <= 3)
    {
      return PocStatus.ExpiringSoon;
    }

    if (visitsRemaining > totalVisitsAuthorized)
    {
      return PocStatus.ExpiringSoon;
    }

    return PocStatus.Active;
  }

  public string GeneratePocSummaryForNote(PocCalculationResult calculation, CultureInfo? culture = null)
  {
    var status = calculation.IsOnTrack ? "on track" : "behind pace";
    var cultureInfo = culture ?? CultureInfo.CurrentCulture;
    var projectedDate = calculation.ProjectedCompletionDate.ToDateTime(TimeOnly.MinValue).ToString("d", cultureInfo);

    return $"POC Week {calculation.CurrentWeek}/{calculation.TotalWeeks} ({calculation.WeeksRemaining} weeks remaining). " +
           $"Visit utilization: {calculation.UtilizationRate}% ({status}). " +
           $"Projected completion: {projectedDate}.";
  }

  public double CalculateGoalProgress(OutcomeMeasureData outcome)
  {
    var totalChange = outcome.HigherIsBetter
        ? outcome.TargetScore - outcome.InitialScore
        : outcome.InitialScore - outcome.TargetScore;

    if (totalChange <= 0)
    {
      return 0;
    }

    var currentChange = outcome.HigherIsBetter
        ? outcome.CurrentScore - outcome.InitialScore
        : outcome.InitialScore - outcome.CurrentScore;

    var progress = (currentChange / totalChange) * 100d;
    return Math.Clamp(progress, 0, 100);
  }

  public string GenerateSmartGoalFromOutcome(OutcomeMeasureData outcome, string patientName, int targetWeeks)
  {
    var verb = outcome.HigherIsBetter ? "increase" : "decrease";
    var change = Math.Abs(outcome.TargetScore - outcome.InitialScore);
    var direction = outcome.HigherIsBetter ? "point increase" : "point decrease";

    return $"{patientName} will {verb} {outcome.MeasureName} from {outcome.InitialScore} to {outcome.TargetScore} ({change} {direction}) within {targetWeeks} weeks, demonstrating improved functional capacity.";
  }

  public bool ShouldExtendPoc(int weeksRemaining, int visitsRemaining, double goalsAchievedPercentage)
  {
    return (weeksRemaining < 2 && visitsRemaining > 6) || goalsAchievedPercentage < 70;
  }

  public DischargeReadinessResult CalculateDischargeReadiness(double goalsAchievedPercentage, double utilizationRate, int weeksRemaining)
  {
    if (goalsAchievedPercentage >= 85 && utilizationRate >= 70)
    {
      return new DischargeReadinessResult(
          IsReady: true,
          Recommendation: "Patient has achieved treatment goals. Consider discharge planning.",
          SuggestedWeeksUntilDischarge: Math.Min(2, Math.Max(weeksRemaining, 0)));
    }

    if (goalsAchievedPercentage >= 70 && weeksRemaining <= 3)
    {
      return new DischargeReadinessResult(
          IsReady: false,
          Recommendation: "Approaching POC end. Evaluate if extension is needed for remaining goals.",
          SuggestedWeeksUntilDischarge: Math.Max(weeksRemaining, 0));
    }

    return new DischargeReadinessResult(
        IsReady: false,
        Recommendation: "Continue current treatment plan. Monitor progress towards goals.",
        SuggestedWeeksUntilDischarge: Math.Max(4, weeksRemaining));
  }

  public PocValidationResult ValidatePocForNoteSignOff(
      IEnumerable<IcdCodeEntry> icd10Codes,
      IEnumerable<PocGoal> goals,
      DateOnly? startDate,
      DateOnly? endDate)
  {
    var missingFields = new List<string>();
    var warnings = new List<string>();

    var icdList = icd10Codes?.ToList() ?? new List<IcdCodeEntry>();
    var goalList = goals?.ToList() ?? new List<PocGoal>();

    if (icdList.Count < 2)
    {
      missingFields.Add("At least 2 ICD-10 codes required");
    }

    if (goalList.Count == 0)
    {
      missingFields.Add("At least 1 treatment goal required");
    }

    if (startDate is null || endDate is null)
    {
      missingFields.Add("POC start and end dates required");
    }

    if (icdList.Count > 0)
    {
      var invalidCodes = icdList.Where(c => !ValidateIcd10Code(c.Code)).ToList();
      if (invalidCodes.Count > 0)
      {
        warnings.Add("Some ICD-10 codes may have invalid format");
      }

      if (!icdList.Any(c => c.IsPrimary))
      {
        warnings.Add("No primary ICD-10 code designated");
      }
    }

    if (goalList.Count > 0)
    {
      var goalsMissingDates = goalList.Count(g => g.TargetDate is null);
      if (goalsMissingDates > 0)
      {
        warnings.Add("Some goals missing target dates");
      }
    }

    return new PocValidationResult(
        IsValid: missingFields.Count == 0,
        MissingFields: missingFields,
        Warnings: warnings);
  }
}

public sealed record PocCalculationInput(
    DateOnly StartDate,
    DateOnly EndDate,
    int Frequency,
    int TotalVisitsAuthorized,
    int VisitsCompleted,
    int? VisitsScheduled = null);

public sealed record PocCalculationResult(
    int TotalWeeks,
    int CurrentWeek,
    int WeeksRemaining,
    int WeeksElapsed,
    int DaysRemaining,
    int TotalDays,
    double VisitsPerWeek,
    int ExpectedVisitsByNow,
    int VisitsPaceAhead,
    DateOnly ProjectedCompletionDate,
    bool IsOnTrack,
    double UtilizationRate,
    int ProjectedVisitsAtCompletion,
    bool WillExceedAuthorization);

public sealed record OutcomeMeasureData(
    string MeasureName,
    double InitialScore,
    double CurrentScore,
    double TargetScore,
    bool HigherIsBetter);

public sealed record DischargeReadinessResult(
    bool IsReady,
    string Recommendation,
    int SuggestedWeeksUntilDischarge);

public sealed record PocValidationResult(
    bool IsValid,
    IReadOnlyList<string> MissingFields,
    IReadOnlyList<string> Warnings);

public sealed record IcdCodeEntry(
    string Code,
    bool IsPrimary = false);

public sealed record PocGoal(
    string? Description,
    DateOnly? TargetDate);

public enum PocStatus
{
  Active,
  ExpiringSoon,
  Expired,
  Completed
}
