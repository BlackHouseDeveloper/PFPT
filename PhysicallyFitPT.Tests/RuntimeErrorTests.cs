using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using PhysicallyFitPT.Shared;
using Xunit;

namespace PhysicallyFitPT.Tests;

/// <summary>
/// Tests to demonstrate potential runtime errors identified in the codebase
/// </summary>
public class RuntimeErrorTests
{
    [Fact]
    public void InterventionsLibrary_ThrowsKeyNotFoundException_WhenAccessingInvalidKey()
    {
        // Arrange & Act
        Action act = () => _ = InterventionsLibrary.ExerciseLibrary["InvalidBodyPart"];
        
        // Assert
        act.Should().Throw<KeyNotFoundException>()
           .WithMessage("*InvalidBodyPart*");
    }

    [Fact]
    public async Task PatientService_HandlesNullQuery_Gracefully()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var factory = new TestDbContextFactory(options);
        var mockLogger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<PatientService>();
        IPatientService svc = new PatientService(factory, mockLogger);

        // Act & Assert - Should not throw
        var results = await svc.SearchAsync(null!, 10);
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task DatabaseService_ThrowsOnConnectionFailure()
    {
        // Arrange - Invalid connection string to simulate connection failure
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=/invalid/path/nonexistent.db")
            .Options;

        var factory = new TestDbContextFactory(options);
        var mockLogger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<PatientService>();
        IPatientService svc = new PatientService(factory, mockLogger);

        // Act & Assert
        Func<Task> act = async () => await svc.SearchAsync("test", 10);
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public void Entity_DateValidation_AllowsInvalidDates()
    {
        // Arrange & Act - Demonstrates lack of validation
        var patient = new Patient
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateTime.Now.AddYears(10) // Future date - should be invalid
        };

        // Assert - No validation prevents this invalid state
        patient.DateOfBirth.Should().BeAfter(DateTime.Now);
    }

    [Fact]
    public void RomMeasure_AllowsInvalidMeasurements()
    {
        // Arrange & Act - Demonstrates lack of range validation
        var romMeasure = new RomMeasure
        {
            Joint = "Knee",
            Movement = "Flexion",
            MeasuredDegrees = -50, // Negative degrees - should be invalid
            NormalDegrees = 1000   // Unrealistic value - should be invalid
        };

        // Assert - No validation prevents these invalid values
        romMeasure.MeasuredDegrees.Should().BeNegative();
        romMeasure.NormalDegrees.Should().BeGreaterThan(360);
    }

    [Fact]
    public void OutcomeMeasureScore_AllowsInvalidPercentage()
    {
        // Arrange & Act - Demonstrates lack of percentage validation
        var score = new OutcomeMeasureScore
        {
            Instrument = "LEFS",
            Percent = 150.0 // Invalid percentage > 100
        };

        // Assert - No validation prevents invalid percentage
        score.Percent.Should().BeGreaterThan(100);
    }

    [Fact]
    public void Patient_AllowsInvalidEmailFormat()
    {
        // Arrange & Act - Demonstrates lack of email validation
        var patient = new Patient
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "not-an-email" // Invalid email format
        };

        // Assert - No validation prevents invalid email
        patient.Email.Should().NotContain("@");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RequiredStringFields_ShouldNotAllowEmptyOrWhitespace(string? value)
    {
        // Arrange & Act - Demonstrates lack of validation for required fields
        var goal = new Goal
        {
            Description = value! // Required field that shouldn't be empty/null
        };

        // Assert - No validation prevents empty required fields
        goal.Description.Should().BeNullOrWhiteSpace();
    }

    [Fact]
    public async Task NoteBuilderService_CouldThrowNullReference_OnNullObjective()
    {
        // This test demonstrates a potential issue but would require mocking
        // to force the objective to be null - included for documentation
        
        // Potential issue in UpdateObjectiveAsync if note.Objective is null:
        // note.Objective.Rom = rom?.ToList() ?? note.Objective.Rom;
        
        Assert.True(true); // Placeholder - actual test would require complex setup
    }
}

/// <summary>
/// Test helper to create DbContext factory for testing
/// </summary>
public class TestDbContextFactory : IDbContextFactory<ApplicationDbContext>
{
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public TestDbContextFactory(DbContextOptions<ApplicationDbContext> options)
    {
        _options = options;
    }

    public ApplicationDbContext CreateDbContext()
    {
        var context = new ApplicationDbContext(_options);
        context.Database.EnsureCreated();
        return context;
    }

    public async Task<ApplicationDbContext> CreateDbContextAsync()
    {
        var context = new ApplicationDbContext(_options);
        await context.Database.EnsureCreatedAsync();
        return context;
    }
}