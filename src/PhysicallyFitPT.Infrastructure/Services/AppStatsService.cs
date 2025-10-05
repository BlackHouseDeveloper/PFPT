// <copyright file="AppStatsService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services;

using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services.Configuration;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using PhysicallyFitPT.Shared;

/// <summary>
/// Computes aggregated statistics for the Physically Fit PT application while applying in-memory caching to reduce database load.
/// </summary>
/// <remarks>Cache duration defaults to 15 seconds and can be tuned via <c>AppStats:CacheTtlSeconds</c>. Mutating services invalidate the cache after completing persistence.</remarks>
public sealed class AppStatsService : IAppStatsService, IAppStatsInvalidator, IDisposable
{
  private const string CacheKey = "app-stats";
  private static readonly object MeterSync = new();
  private static Meter? metricsMeter;
  private static Counter<long>? cacheWaitCounter;
  private readonly IDbContextFactory<ApplicationDbContext> dbContextFactory;
  private readonly ILogger<AppStatsService> logger;
  private readonly IMemoryCache memoryCache;
  private readonly IOptionsMonitor<AppStatsOptions> optionsMonitor;
  private readonly SemaphoreSlim cacheGate = new(1, 1);
  private bool disposed;

  static AppStatsService()
  {
    InitializeMetrics();
    AppDomain.CurrentDomain.ProcessExit += OnAppDomainShutdown;
    AppDomain.CurrentDomain.DomainUnload += OnAppDomainShutdown;
  }
  // Note: swap IMemoryCache with IDistributedCache when multiple API nodes are deployed.

  /// <summary>
  /// Initializes a new instance of the <see cref="AppStatsService"/> class.
  /// </summary>
  /// <param name="dbContextFactory">Factory for creating database contexts.</param>
  /// <param name="logger">Logger used for diagnostics.</param>
  /// <param name="memoryCache">Process-local cache used to throttle stat recalculation.</param>
  /// <param name="optionsMonitor">Provides configuration for cache behaviour.</param>
  public AppStatsService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    ILogger<AppStatsService> logger,
    IMemoryCache memoryCache,
    IOptionsMonitor<AppStatsOptions> optionsMonitor)
  {
    InitializeMetrics();
    this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    this.optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
  }

  /// <inheritdoc />
  public async Task<AppStatsDto> GetAppStatsAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      if (!this.memoryCache.TryGetValue(CacheKey, out AppStatsDto? snapshot) || snapshot is null)
      {
        var waitStart = Stopwatch.GetTimestamp();
        await this.cacheGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
          if (!this.memoryCache.TryGetValue(CacheKey, out snapshot) || snapshot is null)
          {
            snapshot = await ComputeSnapshotAsync(cancellationToken).ConfigureAwait(false);
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
              AbsoluteExpirationRelativeToNow = GetCacheDuration(),
              Priority = CacheItemPriority.Normal,
            };
            this.memoryCache.Set(CacheKey, snapshot, cacheEntryOptions);
          }
        }
        finally
        {
          this.cacheGate.Release();
          var waitDuration = Stopwatch.GetElapsedTime(waitStart);
          if (waitDuration > TimeSpan.FromMilliseconds(250))
          {
            cacheWaitCounter?.Add(1);
            if (this.logger.IsEnabled(LogLevel.Debug))
            {
              this.logger.LogDebug(AppStatsEventIds.CacheGateContention, "App stats cache gate lock was held for {Duration}ms", waitDuration.TotalMilliseconds);
            }
          }
        }
      }

      if (snapshot is null)
      {
        throw new InvalidOperationException("App stats snapshot could not be loaded.");
      }

      return Clone(snapshot);
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Failed to compute application statistics");
      throw;
    }
  }

  /// <inheritdoc />
  public void InvalidateCache()
  {
    this.memoryCache.Remove(CacheKey);
  }

  /// <inheritdoc />
  public void Dispose()
  {
    if (this.disposed)
    {
      return;
    }

    this.cacheGate.Dispose();
    this.disposed = true;
    GC.SuppressFinalize(this);
  }

  private TimeSpan GetCacheDuration()
  {
    var seconds = Math.Clamp(this.optionsMonitor.CurrentValue.CacheTtlSeconds, 1, 300);
    return TimeSpan.FromSeconds(seconds);
  }

  /// <summary>
  /// Materializes the current application statistics directly from the database. Relies on SQLite's
  /// support for ordering nullable <see cref="DateTimeOffset"/> columns in EF Core 8.
  /// </summary>
  private async Task<AppStatsDto> ComputeSnapshotAsync(CancellationToken cancellationToken)
  {
    await using var db = await this.dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

    var patientCountTask = db.Patients.AsNoTracking().CountAsync(cancellationToken);
    var appointmentCountTask = db.Appointments.AsNoTracking().CountAsync(cancellationToken);
    var latestTimestampsTask = db.Patients.AsNoTracking()
      .Select(p => p.UpdatedAt ?? p.CreatedAt)
      .ToListAsync(cancellationToken);

    await Task.WhenAll(patientCountTask, appointmentCountTask, latestTimestampsTask).ConfigureAwait(false);

    var timestampCandidates = await latestTimestampsTask.ConfigureAwait(false);
    DateTimeOffset? latestTimestamp = timestampCandidates.Count > 0
      ? timestampCandidates.Max()
      : null;

    return new AppStatsDto
    {
      Patients = await patientCountTask.ConfigureAwait(false),
      Appointments = await appointmentCountTask.ConfigureAwait(false),
      LastPatientUpdated = latestTimestamp,
      ApiHealthy = true,
    };
  }

  private static AppStatsDto Clone(AppStatsDto source)
  {
    return source with { };
  }

  private static void InitializeMetrics()
  {
    if (metricsMeter is not null)
    {
      return;
    }

    lock (MeterSync)
    {
      if (metricsMeter is null)
      {
        var meter = new Meter("PhysicallyFitPT.AppStats", "1.0");
        metricsMeter = meter;
        cacheWaitCounter = meter.CreateCounter<long>("appstats.cache_wait_exceeded.count");
      }
    }
  }

  private static void OnAppDomainShutdown(object? sender, EventArgs e)
  {
    lock (MeterSync)
    {
      var meter = metricsMeter;
      if (meter is null)
      {
        return;
      }

      metricsMeter = null;
      cacheWaitCounter = null;
      meter.Dispose();
    }
  }
}
