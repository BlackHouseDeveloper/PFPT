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
  /// <returns></returns>
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
  /// <returns></returns>
  public static IEnumerable<string> GetAvailableBodyRegions()
  {
    return InterventionsLibrary.ExerciseLibrary.Keys;
  }

  /// <summary>
  /// Checks if a body region has exercises available.
  /// </summary>
  /// <returns></returns>
  public static bool HasExercises(string bodyRegion)
  {
    return !string.IsNullOrWhiteSpace(bodyRegion) &&
           InterventionsLibrary.ExerciseLibrary.ContainsKey(bodyRegion);
  }
}
