// <copyright file="EnvDetector.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Seeder.Utils;

/// <summary>
/// Utility for detecting and managing environment contexts.
/// </summary>
public static class EnvDetector
{
  /// <summary>
  /// Gets the current environment name from various sources.
  /// Priority: --env CLI argument > PFP_ENV environment variable > ASPNETCORE_ENVIRONMENT > "Development".
  /// </summary>
  /// <returns>The current environment name.</returns>
  public static string GetCurrentEnvironment()
  {
    // Check PFP_ENV first (seeder-specific)
    var pfpEnv = Environment.GetEnvironmentVariable("PFP_ENV");
    if (!string.IsNullOrWhiteSpace(pfpEnv))
    {
      return pfpEnv;
    }

    // Fallback to standard ASP.NET Core environment
    var aspNetEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    if (!string.IsNullOrWhiteSpace(aspNetEnv))
    {
      return aspNetEnv;
    }

    // Default to Development
    return "Development";
  }

  /// <summary>
  /// Checks if the current environment matches any of the allowed environments.
  /// </summary>
  /// <param name="allowedEnvironments">List of allowed environments.</param>
  /// <param name="currentEnvironment">Current environment (optional, will detect if not provided).</param>
  /// <returns>True if the environment is allowed or if no restrictions are specified.</returns>
  public static bool IsEnvironmentAllowed(IReadOnlyList<string> allowedEnvironments, string? currentEnvironment = null)
  {
    if (allowedEnvironments == null || allowedEnvironments.Count == 0)
    {
      return true; // No restrictions
    }

    currentEnvironment ??= GetCurrentEnvironment();
    return allowedEnvironments.Contains(currentEnvironment, StringComparer.OrdinalIgnoreCase);
  }

  /// <summary>
  /// Standard environment names used in the system.
  /// </summary>
  public static class Environments
  {
    /// <summary>
    /// Development environment name.
    /// </summary>
    public const string Development = "Development";

    /// <summary>
    /// Staging environment name.
    /// </summary>
    public const string Staging = "Staging";

    /// <summary>
    /// Production environment name.
    /// </summary>
    public const string Production = "Production";
  }
}