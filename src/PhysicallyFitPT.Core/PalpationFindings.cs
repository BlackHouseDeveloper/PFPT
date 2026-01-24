// <copyright file="PalpationFindings.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core;

/// <summary>
/// Common palpation findings during physical examination.
/// </summary>
public static class PalpationFindings
{
  /// <summary>
  /// Gets all standard palpation finding descriptions.
  /// </summary>
  public static readonly List<string> All = new()
    {
        "Tenderness",
        "Muscle spasm",
        "Trigger points",
        "Swelling/edema",
        "Warmth",
        "Crepitus",
        "Muscle guarding",
        "Decreased tissue mobility",
        "No abnormalities noted",
    };
}
