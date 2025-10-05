namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

using System.Threading;
using System.Threading.Tasks;
using PhysicallyFitPT.Shared;

/// <summary>
/// Provides aggregated statistics for application health dashboards.
/// </summary>
public interface IAppStatsService
{
  /// <summary>
  /// Computes the latest application statistics snapshot.
  /// </summary>
  /// <param name="cancellationToken">Cancellation token for the async operation.</param>
  /// <returns>The aggregated statistics for the app.</returns>
  Task<AppStatsDto> GetAppStatsAsync(CancellationToken cancellationToken = default);

  /// <summary>
  /// Invalidates any cached statistics so subsequent calls recompute fresh values.
  /// </summary>
  void InvalidateCache();
}
