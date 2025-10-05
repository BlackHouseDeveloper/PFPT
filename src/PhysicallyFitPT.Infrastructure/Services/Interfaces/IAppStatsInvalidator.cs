namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

/// <summary>
/// Exposes cache invalidation for application statistics, allowing write operations (appointments, patients, notes, etc.) to keep dashboards fresh.
/// </summary>
public interface IAppStatsInvalidator
{
  /// <summary>
  /// Invalidates any cached statistics snapshot.
  /// </summary>
  void InvalidateCache();
}
