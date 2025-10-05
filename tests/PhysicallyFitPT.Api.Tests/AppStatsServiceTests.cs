using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PhysicallyFitPT.Core;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Configuration;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using PhysicallyFitPT.Shared;
using Xunit;

namespace PhysicallyFitPT.Api.Tests;

public class AppStatsServiceTests
{
  [Fact]
  public async Task GetAppStatsAsync_ReturnsZeroWhenDatabaseIsEmpty()
  {
    // Arrange
    var (service, factory) = CreateService();

    // Act
    var stats = await service.GetAppStatsAsync();

    // Assert
    Assert.Equal(0, stats.Patients);
    Assert.Equal(0, stats.Appointments);
    Assert.Null(stats.LastPatientUpdated);
    Assert.True(stats.ApiHealthy);
    Assert.Equal(1, factory.CreateCount);
  }

  [Fact]
  public async Task GetAppStatsAsync_RespectsCacheUntilInvalidation()
  {
    var (service, factory) = CreateService();

    // Prime the cache (no data yet)
    await service.GetAppStatsAsync();

    await using (var ctx = await factory.CreateDbContextAsync())
    {
      var patient = new Patient
      {
        FirstName = "Pat",
        LastName = "Example",
        MRN = "MRN-1",
        Email = "pat@example.com",
      };

      ctx.Patients.Add(patient);
      ctx.Appointments.Add(new Appointment
      {
        PatientId = patient.Id,
        ScheduledStart = DateTimeOffset.UtcNow.AddHours(1),
        VisitType = VisitType.Daily,
      });

      await ctx.SaveChangesAsync();
    }

    // Cache still returns the old value
    var cachedStats = await service.GetAppStatsAsync();
    Assert.Equal(0, cachedStats.Patients);
    Assert.Equal(0, cachedStats.Appointments);

    // After invalidation we should observe the latest values
    service.InvalidateCache();
    var refreshedStats = await service.GetAppStatsAsync();

    Assert.Equal(1, refreshedStats.Patients);
    Assert.Equal(1, refreshedStats.Appointments);
    Assert.NotNull(refreshedStats.LastPatientUpdated);
  }

  [Fact]
  public async Task GetAppStatsAsync_DebouncesConcurrentRequests()
  {
    var (service, factory) = CreateService();

    var tasks = Enumerable.Range(0, 8)
      .Select(_ => service.GetAppStatsAsync())
      .ToArray();

    await Task.WhenAll(tasks);

    Assert.Equal(1, factory.CreateCount);
  }

  [Fact]
  public async Task GetAppStatsAsync_ComputesLatestTimestampEvenWhenOutOfOrder()
  {
    var (service, factory) = CreateService();
    DateTimeOffset? expected = null;

    await using (var ctx = await factory.CreateDbContextAsync())
    {
      var patientA = new Patient { FirstName = "A", LastName = "One", MRN = "MRN-1" };
      var patientB = new Patient { FirstName = "B", LastName = "Two", MRN = "MRN-2" };
      ctx.Patients.AddRange(patientA, patientB);
      await ctx.SaveChangesAsync();

      patientA.LastName = "One-Updated";
      await ctx.SaveChangesAsync();

      patientB.LastName = "Two-Updated";
      await ctx.SaveChangesAsync();
      expected = patientB.UpdatedAt ?? patientB.CreatedAt;
    }

    service.InvalidateCache();
    var stats = await service.GetAppStatsAsync();

    Assert.Equal(expected?.UtcDateTime, stats.LastPatientUpdated?.UtcDateTime);
  }

  private static (AppStatsService Service, CountingDbContextFactory Factory) CreateService(int cacheTtlSeconds = 15)
  {
    var dbName = CreateUniqueDatabaseName();
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase(dbName)
      .Options;

    var factory = new CountingDbContextFactory(options);
    var cache = new MemoryCache(new MemoryCacheOptions());
    var optionsMonitor = new TestOptionsMonitor<AppStatsOptions>(new AppStatsOptions { CacheTtlSeconds = cacheTtlSeconds });

    var service = new AppStatsService(
      factory,
      NullLogger<AppStatsService>.Instance,
      cache,
      optionsMonitor);

    return (service, factory);
  }

  private static string CreateUniqueDatabaseName()
  {
    var sequence = Interlocked.Increment(ref databaseCounter);
    return $"AppStatsServiceTests_{sequence}";
  }

  private static int databaseCounter;

  private sealed class CountingDbContextFactory : IDbContextFactory<ApplicationDbContext>
  {
    private readonly DbContextOptions<ApplicationDbContext> options;
    private int createCount;

    public CountingDbContextFactory(DbContextOptions<ApplicationDbContext> options)
    {
      this.options = options;
    }

    public int CreateCount => this.createCount;

    public ApplicationDbContext CreateDbContext()
    {
      Interlocked.Increment(ref this.createCount);
      return new ApplicationDbContext(this.options);
    }

    public Task<ApplicationDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
      Interlocked.Increment(ref this.createCount);
      return Task.FromResult(new ApplicationDbContext(this.options));
    }
  }

  private sealed class TestOptionsMonitor<T> : IOptionsMonitor<T>
    where T : class, new()
  {
    private T currentValue;

    public TestOptionsMonitor(T value) => this.currentValue = value ?? new T();

    public T CurrentValue => this.currentValue;

    public T Get(string? name) => this.currentValue;

    public IDisposable OnChange(Action<T, string> listener) => NullDisposable.Instance;

    private sealed class NullDisposable : IDisposable
    {
      public static readonly NullDisposable Instance = new();

      public void Dispose()
      {
      }
    }
  }
}
