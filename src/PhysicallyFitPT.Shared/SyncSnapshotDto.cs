// <copyright file="SyncSnapshotDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

using System;

/// <summary>
/// Represents a snapshot of remote aggregate data used to hydrate offline caches.
/// </summary>
public class SyncSnapshotDto
{
  /// <summary>
  /// Gets or sets the application statistics payload.
  /// </summary>
  public AppStatsDto AppStats { get; set; } = new();

  /// <summary>
  /// Gets or sets the dashboard statistics payload.
  /// </summary>
  public DashboardStatsDto DashboardStats { get; set; } = new();

  /// <summary>
  /// Gets or sets when the remote system generated the snapshot.
  /// </summary>
  public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;
}
