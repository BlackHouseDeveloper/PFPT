namespace PhysicallyFitPT.Shared;

/// <summary>
/// Safe wrapper for InterventionsLibrary to prevent KeyNotFoundException
/// </summary>
public static class SafeInterventionsLibrary
{
    /// <summary>
    /// Safely gets exercises for a body region, returning empty list if not found
    /// </summary>
    public static List<string> GetExercises(string bodyRegion)
    {
        if (string.IsNullOrWhiteSpace(bodyRegion))
            return new List<string>();
            
        return InterventionsLibrary.ExerciseLibrary.TryGetValue(bodyRegion, out var exercises) 
            ? exercises 
            : new List<string>();
    }

    /// <summary>
    /// Gets all available body regions
    /// </summary>
    public static IEnumerable<string> GetAvailableBodyRegions()
    {
        return InterventionsLibrary.ExerciseLibrary.Keys;
    }

    /// <summary>
    /// Checks if a body region has exercises available
    /// </summary>
    public static bool HasExercises(string bodyRegion)
    {
        return !string.IsNullOrWhiteSpace(bodyRegion) && 
               InterventionsLibrary.ExerciseLibrary.ContainsKey(bodyRegion);
    }
}