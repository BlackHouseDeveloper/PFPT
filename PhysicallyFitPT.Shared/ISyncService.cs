// <copyright file="ISyncService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Synchronization status enumeration.
/// </summary>
public enum SyncStatus
{
  /// <summary>
  /// No sync operation is currently running.
  /// </summary>
  Idle,

  /// <summary>
  /// Sync operation is currently in progress.
  /// </summary>
  Syncing,

  /// <summary>
  /// Last sync operation completed successfully.
  /// </summary>
  Success,

  /// <summary>
  /// Last sync operation failed.
  /// </summary>
  Failed,

  /// <summary>
  /// Sync is disabled or not available.
  /// </summary>
  Disabled,
}

/// <summary>
/// Interface for background synchronization services (primarily for MAUI offline-first implementation).
/// </summary>
public interface ISyncService
{
  /// <summary>
  /// Event raised when sync status changes.
  /// </summary>
  event EventHandler<SyncStatus>? StatusChanged;

  /// <summary>
  /// Gets the current synchronization status.
  /// </summary>
  SyncStatus Status { get; }

  /// <summary>
  /// Gets the timestamp of the last successful synchronization.
  /// </summary>
  DateTimeOffset? LastSyncTime { get; }

  /// <summary>
  /// Gets the last sync error message if any.
  /// </summary>
  string? LastError { get; }

  /// <summary>
  /// Performs a manual synchronization operation.
  /// </summary>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>True if sync was successful, false otherwise.</returns>
  Task<bool> SyncAsync(CancellationToken cancellationToken = default);

  /// <summary>
  /// Starts periodic background synchronization.
  /// </summary>
  /// <param name="intervalMinutes">Interval between sync operations in minutes.</param>
  void StartPeriodicSync(int intervalMinutes = 30);

  /// <summary>
  /// Stops periodic background synchronization.
  /// </summary>
  void StopPeriodicSync();

  /// <summary>
  /// Forces a sync on connectivity restored (if supported by platform).
  /// </summary>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>A task representing the sync operation.</returns>
  Task SyncOnConnectivityRestoredAsync(CancellationToken cancellationToken = default);
}
