// <copyright file="HybridSyncService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Services;

using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using PhysicallyFitPT.Shared;

/// <summary>
/// Implements a hybrid sync layer that keeps the offline store hydrated with remote aggregate data.
/// </summary>
public sealed class HybridSyncService : ISyncService, IDisposable
{
  private const string ApiClientName = "api";

  private readonly IHttpClientFactory httpClientFactory;
  private readonly ILogger<HybridSyncService> logger;
  private readonly JsonSerializerOptions jsonOptions;
  private readonly string snapshotPath;
  private readonly object syncGate = new();

  private Timer? timer;
  private bool disposed;

  /// <summary>
  /// Initializes a new instance of the <see cref="HybridSyncService"/> class.
  /// </summary>
  /// <param name="httpClientFactory">Factory used to create HTTP clients for the remote API.</param>
  /// <param name="logger">Logger used to capture sync diagnostics.</param>
  public HybridSyncService(IHttpClientFactory httpClientFactory, ILogger<HybridSyncService> logger)
  {
    this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

    this.jsonOptions = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    this.snapshotPath = Path.Combine(FileSystem.AppDataDirectory, "sync-snapshot.json");
    this.LoadCachedSnapshot();
  }

  /// <inheritdoc />
  public event EventHandler<SyncStatus>? StatusChanged;

  /// <inheritdoc />
  public SyncStatus Status { get; private set; } = SyncStatus.Idle;

  /// <inheritdoc />
  public DateTimeOffset? LastSyncTime { get; private set; }

  /// <inheritdoc />
  public string? LastError { get; private set; }

  /// <inheritdoc />
  public SyncSnapshotDto? LatestSnapshot { get; private set; }

  /// <inheritdoc />
  public async Task<bool> SyncAsync(CancellationToken cancellationToken = default)
  {
    lock (this.syncGate)
    {
      if (this.Status == SyncStatus.Syncing)
      {
        this.logger.LogDebug("Sync already in progress; skipping overlapping execution");
        return false;
      }

      this.Status = SyncStatus.Syncing;
      this.LastError = null;
      this.OnStatusChanged();
    }

    try
    {
      var client = this.httpClientFactory.CreateClient(ApiClientName);
      using var response = await client.GetAsync(ApiRoutes.V1("sync", "snapshot"), cancellationToken).ConfigureAwait(false);

      if (!response.IsSuccessStatusCode)
      {
        this.LastError = $"Sync failed with status code {response.StatusCode}";
        this.Status = SyncStatus.Failed;
        this.OnStatusChanged();
        return false;
      }

      await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
      var snapshot = await JsonSerializer.DeserializeAsync<SyncSnapshotDto>(responseStream, this.jsonOptions, cancellationToken).ConfigureAwait(false);

      if (snapshot is null)
      {
        this.LastError = "Sync response payload was empty";
        this.Status = SyncStatus.Failed;
        this.OnStatusChanged();
        return false;
      }

      snapshot.AppStats = snapshot.AppStats with { ApiHealthy = true };
      this.LatestSnapshot = snapshot;
      this.LastSyncTime = DateTimeOffset.UtcNow;
      this.Status = SyncStatus.Success;

      await this.PersistSnapshotAsync(snapshot, cancellationToken).ConfigureAwait(false);
      this.OnStatusChanged();
      return true;
    }
    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
    {
      this.Status = SyncStatus.Idle;
      this.OnStatusChanged();
      throw;
    }
    catch (Exception ex)
    {
      this.logger.LogWarning(ex, "Hybrid sync failed");
      this.LastError = ex.Message;
      this.Status = SyncStatus.Failed;
      this.OnStatusChanged();
      return false;
    }
  }

  /// <inheritdoc />
  public void StartPeriodicSync(int intervalMinutes = 30)
  {
    var interval = TimeSpan.FromMinutes(Math.Max(intervalMinutes, 1));

    if (this.timer is null)
    {
      this.timer = new Timer(async _ => await this.SafeSyncAsync().ConfigureAwait(false), null, TimeSpan.Zero, interval);
    }
  }

  /// <inheritdoc />
  public void StopPeriodicSync()
  {
    this.timer?.Dispose();
    this.timer = null;
    this.Status = SyncStatus.Idle;
    this.OnStatusChanged();
  }

  /// <inheritdoc />
  public Task SyncOnConnectivityRestoredAsync(CancellationToken cancellationToken = default)
  {
    return this.SyncAsync(cancellationToken);
  }

  /// <inheritdoc />
  public void Dispose()
  {
    if (this.disposed)
    {
      return;
    }

    this.timer?.Dispose();
    this.disposed = true;
    GC.SuppressFinalize(this);
  }

  private async Task SafeSyncAsync()
  {
    try
    {
      await this.SyncAsync().ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      this.logger.LogDebug(ex, "Background sync threw an exception");
    }
  }

  private void LoadCachedSnapshot()
  {
    try
    {
      if (!File.Exists(this.snapshotPath))
      {
        return;
      }

      using var stream = File.OpenRead(this.snapshotPath);
      var snapshot = JsonSerializer.Deserialize<SyncSnapshotDto>(stream, this.jsonOptions);
      if (snapshot is not null)
      {
        this.LatestSnapshot = snapshot;
        this.LastSyncTime = snapshot.GeneratedAt;
        this.Status = SyncStatus.Success;
      }
    }
    catch (Exception ex)
    {
      this.logger.LogWarning(ex, "Failed to hydrate cached sync snapshot");
    }
  }

  private async Task PersistSnapshotAsync(SyncSnapshotDto snapshot, CancellationToken cancellationToken)
  {
    try
    {
      var directory = Path.GetDirectoryName(this.snapshotPath);
      if (!string.IsNullOrEmpty(directory))
      {
        Directory.CreateDirectory(directory);
      }

      await using var stream = new FileStream(this.snapshotPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
      await JsonSerializer.SerializeAsync(stream, snapshot, this.jsonOptions, cancellationToken).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      this.logger.LogWarning(ex, "Failed to persist sync snapshot to disk");
    }
  }

  private void OnStatusChanged()
  {
    this.StatusChanged?.Invoke(this, this.Status);
  }
}
