// <copyright file="AppStatsEventIds.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services;

using Microsoft.Extensions.Logging;

/// <summary>
/// Central registry for logging identifiers used by the statistics services.
/// </summary>
public static class AppStatsEventIds
{
  /// <summary>
  /// Event identifier emitted when contention on the statistics cache semaphore exceeds the configured threshold.
  /// </summary>
  public static readonly EventId CacheGateContention = new(7201, nameof(CacheGateContention));
}
