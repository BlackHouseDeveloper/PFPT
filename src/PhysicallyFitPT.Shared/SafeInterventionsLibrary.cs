// <copyright file="SafeInterventionsLibrary.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

/// <summary>
/// Safe wrapper for InterventionsLibrary to prevent KeyNotFoundException.
/// </summary>
public static class SafeInterventionsLibrary
{
  /// <summary>
  /// Safely gets exercises for a body region, returning empty list if not found.
  /// </summary>
  /// <param name="bodyRegion">The body region to get exercises for.</param>
  /// <returns>A list of exercise names for the specified body region, or empty list if not found.</returns>
  public static List<string> GetExercises(string bodyRegion)
  {
    if (string.IsNullOrWhiteSpace(bodyRegion))
    {
      return new List<string>();
    }

    return InterventionsLibrary.ExerciseLibrary.TryGetValue(bodyRegion, out var exercises)
        ? exercises
        : new List<string>();
  }

  /// <summary>
  /// Gets all available body regions.
  /// </summary>
  /// <returns>An enumerable collection of available body region names.</returns>
  public static IEnumerable<string> GetAvailableBodyRegions()
  {
    return InterventionsLibrary.ExerciseLibrary.Keys;
  }

  /// <summary>
  /// Checks if a body region has exercises available.
  /// </summary>
  /// <param name="bodyRegion">The body region to check for available exercises.</param>
  /// <returns>True if the body region has exercises available; otherwise, false.</returns>
  public static bool HasExercises(string bodyRegion)
  {
    return !string.IsNullOrWhiteSpace(bodyRegion) &&
           InterventionsLibrary.ExerciseLibrary.ContainsKey(bodyRegion);
  }
}
