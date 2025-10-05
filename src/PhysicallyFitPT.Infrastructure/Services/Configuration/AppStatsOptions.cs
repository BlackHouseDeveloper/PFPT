// <copyright file="AppStatsOptions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services.Configuration;

/// <summary>
/// Configuration options controlling how aggregated statistics are cached.
/// </summary>
public class AppStatsOptions
{
  /// <summary>
  /// Gets or sets the cache duration (in seconds) for application statistics. Defaults to 15 seconds.
  /// </summary>
  public int CacheTtlSeconds { get; set; } = 15;
}
