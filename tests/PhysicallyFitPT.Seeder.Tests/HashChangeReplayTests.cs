// <copyright file="HashChangeReplayTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Seeder.Seeding;
using PhysicallyFitPT.Seeder.Seeding.Hashing;
using Xunit;

namespace PhysicallyFitPT.Seeder.Tests;

/// <summary>
/// Tests for hash-based change detection and replay functionality.
/// </summary>
public class HashChangeReplayTests : IDisposable
{
  private readonly ApplicationDbContext dbContext;
  private readonly SeedRunner seedRunner;
  private readonly IServiceProvider serviceProvider;

  /// <summary>
  /// Initializes a new instance of the <see cref="HashChangeReplayTests"/> class.
  /// </summary>
  public HashChangeReplayTests()
  {
    // Create in-memory database for testing
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
      .Options;

    this.dbContext = new ApplicationDbContext(options);
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
  /// Tests that hash calculation is consistent for same inputs.
  /// </summary>
  [Fact]
  public void HashCalculator_ProducesConsistentHashes_ForSameInputs()
  {
    // Arrange
    var taskId = "001.test.task";
    var content = "test content";

    // Act
    var hash1 = SeedHashCalculator.ComputeHash(taskId, content);
    var hash2 = SeedHashCalculator.ComputeHash(taskId, content);

    // Assert
    hash1.Should().Be(hash2);
    hash1.Should().HaveLength(64); // SHA256 produces 64-character hex string
  }

  /// <summary>
  /// Tests that different inputs produce different hashes.
  /// </summary>
  [Fact]
  public void HashCalculator_ProducesDifferentHashes_ForDifferentInputs()
  {
    // Arrange
    var taskId = "001.test.task";
    var content1 = "test content 1";
    var content2 = "test content 2";

    // Act
    var hash1 = SeedHashCalculator.ComputeHash(taskId, content1);
    var hash2 = SeedHashCalculator.ComputeHash(taskId, content2);

    // Assert
    hash1.Should().NotBe(hash2);
  }

  /// <summary>
  /// Tests that tasks are not re-run when hash hasn't changed.
  /// </summary>
  /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
  [Fact]
  public async Task UnchangedHash_SkipsTaskExecution_WithoutReplayFlag()
  {
    // Arrange - Run seeding first time
    var options = new SeedRunOptions
    {
      Environment = "Development",
      Force = false,
      ReplayChanged = false,
      DryRun = false,
    };

    await this.seedRunner.RunAsync(options);
    var initialCount = await this.dbContext.CptCodes.CountAsync();

    // Act - Run seeding second time without replay flag
    var result = await this.seedRunner.RunAsync(options);

    // Assert
    result.Should().BeTrue();
    var finalCount = await this.dbContext.CptCodes.CountAsync();
    finalCount.Should().Be(initialCount); // No new records should be added

    // Verify seed history timestamps haven't changed significantly
    var seedHistory = await this.dbContext.SeedHistory.ToListAsync();
    seedHistory.Should().HaveCount(4);

    // All tasks should have been applied around the same time (first run)
    var appliedTimes = seedHistory.Select(h => h.AppliedAtUtc).ToList();
    var timeSpan = appliedTimes.Max() - appliedTimes.Min();
    timeSpan.Should().BeLessThan(TimeSpan.FromMinutes(1)); // All applied in first run
  }

  /// <summary>
  /// Tests that replay-changed flag re-runs all tasks even with unchanged hashes.
  /// </summary>
  /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
  [Fact]
  public async Task ReplayChangedFlag_RerunsAllTasks_EvenWithUnchangedHashes()
  {
    // Arrange - Run seeding first time
    var initialOptions = new SeedRunOptions
    {
      Environment = "Development",
      Force = false,
      ReplayChanged = false,
      DryRun = false,
    };

    await this.seedRunner.RunAsync(initialOptions);
    var firstRunHistory = await this.dbContext.SeedHistory.ToListAsync();
    var firstRunTime = firstRunHistory.First().AppliedAtUtc;

    // Wait a moment to ensure timestamps will be different
    await Task.Delay(100);

    // Act - Run seeding with replay-changed flag
    var replayOptions = new SeedRunOptions
    {
      Environment = "Development",
      Force = false,
      ReplayChanged = true, // This should cause re-runs
      DryRun = false,
    };

    var result = await this.seedRunner.RunAsync(replayOptions);

    // Assert
    result.Should().BeTrue();

    // Verify seed history timestamps have been updated
    var updatedHistory = await this.dbContext.SeedHistory.ToListAsync();
    updatedHistory.Should().HaveCount(4);

    // All tasks should have newer timestamps
    foreach (var history in updatedHistory)
    {
      history.AppliedAtUtc.Should().BeAfter(firstRunTime);
    }
  }

  /// <summary>
  /// Tests fallback hash calculation for inline data.
  /// </summary>
  [Fact]
  public void FallbackHash_ProducesValidHash_ForInlineData()
  {
    // Arrange
    var taskId = "001.test.task";
    var fallbackSignature = "inline-data-signature";
    var versionToken = "v1.0";

    // Act
    var hash = SeedHashCalculator.ComputeFallbackHash(taskId, fallbackSignature, versionToken);

    // Assert
    hash.Should().NotBeNullOrEmpty();
    hash.Should().HaveLength(64);
    hash.Should().MatchRegex("^[a-f0-9]{64}$"); // Valid hex string
  }

  /// <summary>
  /// Tests that version token changes affect fallback hash.
  /// </summary>
  [Fact]
  public void FallbackHash_ChangesWith_VersionToken()
  {
    // Arrange
    var taskId = "001.test.task";
    var fallbackSignature = "inline-data-signature";

    // Act
    var hashV1 = SeedHashCalculator.ComputeFallbackHash(taskId, fallbackSignature, "v1.0");
    var hashV2 = SeedHashCalculator.ComputeFallbackHash(taskId, fallbackSignature, "v2.0");

    // Assert
    hashV1.Should().NotBe(hashV2);
  }

  /// <summary>
  /// Tests that force with replay-changed re-runs all tasks.
  /// </summary>
  /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
  [Fact]
  public async Task ForceWithReplayChanged_RerunsAllTasks_IgnoringEnvironmentAndHashes()
  {
    // Arrange - Run seeding first time in Development
    var initialOptions = new SeedRunOptions
    {
      Environment = "Development",
      Force = false,
      ReplayChanged = false,
      DryRun = false,
    };

    await this.seedRunner.RunAsync(initialOptions);

    // Act - Run with force and replay-changed in Production (should override restrictions)
    var forceOptions = new SeedRunOptions
    {
      Environment = "Production",
      Force = true,
      ReplayChanged = true,
      DryRun = false,
    };

    var result = await this.seedRunner.RunAsync(forceOptions);

    // Assert
    result.Should().BeTrue();

    // Even in Production with force, all tasks should run including patients
    var patients = await this.dbContext.Patients.ToListAsync();
    patients.Should().HaveCountGreaterThan(0);

    var seedHistory = await this.dbContext.SeedHistory.ToListAsync();
    seedHistory.Should().HaveCount(4);
    seedHistory.Should().Contain(h => h.TaskId == "003.patients.sample");
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    this.dbContext?.Dispose();
    if (this.serviceProvider is IDisposable disposable) disposable.Dispose();
  }
}
