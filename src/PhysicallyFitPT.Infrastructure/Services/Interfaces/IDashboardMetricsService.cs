// <copyright file="IDashboardMetricsService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

using System.Threading;
using System.Threading.Tasks;
using PhysicallyFitPT.Shared;

/// <summary>
/// Provides aggregated metrics for the dashboard experience.
/// </summary>
public interface IDashboardMetricsService
{
  /// <summary>
  /// Gets the latest dashboard statistics from the persistence store.
  /// </summary>
  /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
  /// <returns>The computed dashboard statistics.</returns>
  Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default);
}
