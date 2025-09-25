// <copyright file="SeedRunIntegrationTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PhysicallyFitPT.Core;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Seeder.Seeding;
using Xunit;

namespace PhysicallyFitPT.Seeder.Tests;

/// <summary>
/// Integration tests for the seeding framework.
/// </summary>
public class SeedRunIntegrationTests : IDisposable
{
  private readonly ApplicationDbContext dbContext;
  private readonly SeedRunner seedRunner;
  private readonly IServiceProvider serviceProvider;

  /// <summary>
  /// Initializes a new instance of the <see cref="SeedRunIntegrationTests"/> class.
  /// </summary>
  public SeedRunIntegrationTests()
  {
    // Create in-memory database for testing
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
      .Options;

    this.dbContext = new ApplicationDbContext(options);

    // Ensure database is created
    this.dbContext.Database.EnsureCreated();

    // Build service provider with seeding services
    var services = new ServiceCollection();
    services.AddSingleton(this.dbContext);
    services.AddSingleton<ILogger<SeedRunner>>(_ => NullLogger<SeedRunner>.Instance);
    services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
    PhysicallyFitPT.Seeder.Seeding.SeedTaskRegistry.AddSeedingServices(services);

    this.serviceProvider = services.BuildServiceProvider();
    this.seedRunner = this.serviceProvider.GetRequiredService<SeedRunner>();
  }

  /// <summary>
  /// Tests that the first run seeds all applicable tasks.
  /// </summary>
  /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
  [Fact]
  public async Task FirstRun_SeedsAllApplicableTasks_InDevelopmentEnvironment()
  {
    // Arrange
    var options = new SeedRunOptions
    {
      Environment = "Development",
      Force = false,
      ReplayChanged = false,
      DryRun = false,
    };

    // Act
    var result = await this.seedRunner.RunAsync(options);

    // Assert
    result.Should().BeTrue();

    // Verify data was seeded
    var cptCodes = await this.dbContext.CptCodes.ToListAsync();
    var icd10Codes = await this.dbContext.Icd10Codes.ToListAsync();
    var patients = await this.dbContext.Patients.ToListAsync();
    var referenceSummary = await this.dbContext.ReferenceSummaries.FirstOrDefaultAsync(r => r.Id == 1);

    cptCodes.Should().HaveCountGreaterThan(0);
    icd10Codes.Should().HaveCountGreaterThan(0);
    patients.Should().HaveCountGreaterThan(0); // Should include sample patients in Development
    referenceSummary.Should().NotBeNull();
    referenceSummary!.CptCount.Should().Be(cptCodes.Count);
    referenceSummary.Icd10Count.Should().Be(icd10Codes.Count);

    // Verify seed history
    var seedHistory = await this.dbContext.SeedHistory.ToListAsync();
    seedHistory.Should().HaveCount(4); // All 4 tasks should be applied
    seedHistory.Should().Contain(h => h.TaskId == "001.cpt.codes");
    seedHistory.Should().Contain(h => h.TaskId == "002.icd10.codes");
    seedHistory.Should().Contain(h => h.TaskId == "003.patients.sample");
    seedHistory.Should().Contain(h => h.TaskId == "004.composite.reference");
  }

  /// <summary>
  /// Tests that the second run is idempotent (no changes).
  /// </summary>
  /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
  [Fact]
  public async Task SecondRun_IsIdempotent_WithoutReplayChangedFlag()
  {
    // Arrange - First run
    var options = new SeedRunOptions
    {
      Environment = "Development",
      Force = false,
      ReplayChanged = false,
      DryRun = false,
    };

    await this.seedRunner.RunAsync(options);

    // Get counts after first run
    var initialCptCount = await this.dbContext.CptCodes.CountAsync();
    var initialIcd10Count = await this.dbContext.Icd10Codes.CountAsync();
    var initialPatientCount = await this.dbContext.Patients.CountAsync();

    // Act - Second run
    var result = await this.seedRunner.RunAsync(options);

    // Assert
    result.Should().BeTrue();

    // Verify no additional data was created
    var finalCptCount = await this.dbContext.CptCodes.CountAsync();
    var finalIcd10Count = await this.dbContext.Icd10Codes.CountAsync();
    var finalPatientCount = await this.dbContext.Patients.CountAsync();

    finalCptCount.Should().Be(initialCptCount);
    finalIcd10Count.Should().Be(initialIcd10Count);
    finalPatientCount.Should().Be(initialPatientCount);

    // Seed history should still have the same records
    var seedHistory = await this.dbContext.SeedHistory.ToListAsync();
    seedHistory.Should().HaveCount(4);
  }

  /// <summary>
  /// Tests that Production environment blocks sample patients.
  /// </summary>
  /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
  [Fact]
  public async Task ProductionEnvironment_BlocksSamplePatients_WithoutForceFlag()
  {
    // Arrange
    var options = new SeedRunOptions
    {
      Environment = "Production",
      Force = false,
      ReplayChanged = false,
      DryRun = false,
    };

    // Act
    var result = await this.seedRunner.RunAsync(options);

    // Assert
    result.Should().BeTrue();

    // Verify no patients were seeded
    var patients = await this.dbContext.Patients.ToListAsync();
    patients.Should().BeEmpty();

    // But CPT and ICD-10 codes should still be seeded
    var cptCodes = await this.dbContext.CptCodes.ToListAsync();
    var icd10Codes = await this.dbContext.Icd10Codes.ToListAsync();
    cptCodes.Should().HaveCountGreaterThan(0);
    icd10Codes.Should().HaveCountGreaterThan(0);

    // Verify seed history - should have 3 tasks (excluding patients)
    var seedHistory = await this.dbContext.SeedHistory.ToListAsync();
    seedHistory.Should().HaveCount(3);
    seedHistory.Should().Contain(h => h.TaskId == "001.cpt.codes");
    seedHistory.Should().Contain(h => h.TaskId == "002.icd10.codes");
    seedHistory.Should().Contain(h => h.TaskId == "004.composite.reference");
    seedHistory.Should().NotContain(h => h.TaskId == "003.patients.sample");
  }

  /// <summary>
  /// Tests that force flag overrides environment restrictions.
  /// </summary>
  /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
  [Fact]
  public async Task ForceFlag_OverridesEnvironmentRestrictions_InProductionEnvironment()
  {
    // Arrange
    var options = new SeedRunOptions
    {
      Environment = "Production",
      Force = true, // This should allow patients to be seeded
      ReplayChanged = false,
      DryRun = false,
    };

    // Act
    var result = await this.seedRunner.RunAsync(options);

    // Assert
    result.Should().BeTrue();

    // Verify patients were seeded despite Production environment
    var patients = await this.dbContext.Patients.ToListAsync();
    patients.Should().HaveCountGreaterThan(0);

    // Verify all 4 tasks were applied
    var seedHistory = await this.dbContext.SeedHistory.ToListAsync();
    seedHistory.Should().HaveCount(4);
    seedHistory.Should().Contain(h => h.TaskId == "003.patients.sample");
  }

  /// <summary>
  /// Tests that task listing returns correct status information.
  /// </summary>
  /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
  [Fact]
  public async Task ListTasks_ReturnsCorrectStatus_BeforeAndAfterSeeding()
  {
    // Act - List tasks before seeding
    var tasksBefore = await this.seedRunner.ListTasksAsync("Development");

    // Assert - All tasks should be pending
    tasksBefore.Should().HaveCount(4);
    tasksBefore.Should().OnlyContain(t => !t.Applied);

    // Arrange - Run seeding
    var options = new SeedRunOptions
    {
      Environment = "Development",
      Force = false,
      ReplayChanged = false,
      DryRun = false,
    };

    await this.seedRunner.RunAsync(options);

    // Act - List tasks after seeding
    var tasksAfter = await this.seedRunner.ListTasksAsync("Development");

    // Assert - All tasks should be applied
    tasksAfter.Should().HaveCount(4);
    tasksAfter.Should().OnlyContain(t => t.Applied);

    // Verify specific task information
    var cptTask = tasksAfter.First(t => t.Id == "001.cpt.codes");
    cptTask.Name.Should().Be("CPT Codes");
    cptTask.PendingReason.Should().BeNull();
  }

  /// <summary>
  /// Tests dry run mode (no actual changes).
  /// </summary>
  /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
  [Fact]
  public async Task DryRun_DoesNotMakeChanges_ButReturnsSuccess()
  {
    // Arrange
    var options = new SeedRunOptions
    {
      Environment = "Development",
      Force = false,
      ReplayChanged = false,
      DryRun = true, // Dry run mode
    };

    // Act
    var result = await this.seedRunner.RunAsync(options);

    // Assert
    result.Should().BeTrue();

    // Verify no data was actually seeded
    var cptCodes = await this.dbContext.CptCodes.ToListAsync();
    var icd10Codes = await this.dbContext.Icd10Codes.ToListAsync();
    var patients = await this.dbContext.Patients.ToListAsync();
    var seedHistory = await this.dbContext.SeedHistory.ToListAsync();

    cptCodes.Should().BeEmpty();
    icd10Codes.Should().BeEmpty();
    patients.Should().BeEmpty();
    seedHistory.Should().BeEmpty();
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    this.dbContext?.Dispose();
    if (this.serviceProvider is IDisposable disposable) disposable.Dispose();
  }
}