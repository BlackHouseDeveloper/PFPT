using System;
using System.Globalization;
using FluentAssertions;
using PhysicallyFitPT.Infrastructure.Services;
using Xunit;

namespace PhysicallyFitPT.Tests;

public class PocCalculationServiceTests
{
    private readonly PocCalculationService _service = new();

    [Fact]
    public void CalculateProgress_AdjustsEndDate_WhenEndPrecedesStart()
    {
        var start = new DateOnly(2024, 1, 10);
        var input = new PocCalculationInput(
            StartDate: start,
            EndDate: new DateOnly(2024, 1, 5),
            Frequency: 3,
            TotalVisitsAuthorized: 12,
            VisitsCompleted: 0);

        var result = _service.CalculateProgress(input, todayOverride: start);

        result.TotalDays.Should().Be(0);
        result.TotalWeeks.Should().Be(0);
        result.CurrentWeek.Should().Be(0);
        result.WeeksRemaining.Should().Be(0);
        result.ProjectedCompletionDate.Should().Be(start.AddDays(28));
    }

    [Fact]
    public void CalculateProgress_HandlesZeroFrequencyWithoutDivisionByZero()
    {
        var today = new DateOnly(2024, 1, 15);
        var input = new PocCalculationInput(
            StartDate: new DateOnly(2024, 1, 1),
            EndDate: new DateOnly(2024, 2, 1),
            Frequency: 0,
            TotalVisitsAuthorized: 10,
            VisitsCompleted: 2);

        var result = _service.CalculateProgress(input, todayOverride: today);

        result.VisitsPerWeek.Should().Be(0);
        result.UtilizationRate.Should().Be(20);
        result.ProjectedCompletionDate.Should().Be(today);
    }

    [Fact]
    public void CalculateGoalProgress_ReturnsProgress_ForHigherIsBetter()
    {
        var outcome = new OutcomeMeasureData(
            MeasureName: "ROM",
            InitialScore: 10,
            CurrentScore: 25,
            TargetScore: 30,
            HigherIsBetter: true);

        var progress = _service.CalculateGoalProgress(outcome);

        progress.Should().Be(75);
    }

    [Fact]
    public void CalculateGoalProgress_ClampsToHundred_ForLowerIsBetter()
    {
        var outcome = new OutcomeMeasureData(
            MeasureName: "Pain",
            InitialScore: 8,
            CurrentScore: 1,
            TargetScore: 2,
            HigherIsBetter: false);

        var progress = _service.CalculateGoalProgress(outcome);

        progress.Should().Be(100);
    }

    [Fact]
    public void ValidatePocForNoteSignOff_ReturnsMissingAndWarnings()
    {
        var icdCodes = new[]
        {
            new IcdCodeEntry("A12"),
            new IcdCodeEntry("invalid")
        };

        var goals = new[] { new PocGoal("Strengthen quad", TargetDate: null) };

        var result = _service.ValidatePocForNoteSignOff(icdCodes, goals, startDate: null, endDate: null);

        result.IsValid.Should().BeFalse();
        result.MissingFields.Should().Contain("POC start and end dates required");
        result.Warnings.Should().Contain("Some ICD-10 codes may have invalid format");
        result.Warnings.Should().Contain("No primary ICD-10 code designated");
        result.Warnings.Should().Contain("Some goals missing target dates");
    }

    [Fact]
    public void CalculatePocStatus_ReturnsExpiringSoon_WhenWeeksLow()
    {
        var status = _service.CalculatePocStatus(weeksRemaining: 2, visitsRemaining: 5, totalVisitsAuthorized: 20);
        status.Should().Be(PocStatus.ExpiringSoon);
    }

    [Fact]
    public void GeneratePocSummaryForNote_FormatsWithCulture()
    {
        var calculation = new PocCalculationResult(
            TotalWeeks: 8,
            CurrentWeek: 3,
            WeeksRemaining: 5,
            WeeksElapsed: 2,
            DaysRemaining: 20,
            TotalDays: 56,
            VisitsPerWeek: 2,
            ExpectedVisitsByNow: 4,
            VisitsPaceAhead: 1,
            ProjectedCompletionDate: new DateOnly(2024, 3, 1),
            IsOnTrack: true,
            UtilizationRate: 50,
            ProjectedVisitsAtCompletion: 12,
            WillExceedAuthorization: false);

        var text = _service.GeneratePocSummaryForNote(calculation, CultureInfo.GetCultureInfo("en-US"));

        text.Should().Contain("POC Week 3/8");
        text.Should().Contain("Visit utilization: 50%");
        text.Should().Contain("Projected completion: 3/1/2024");
    }
}
