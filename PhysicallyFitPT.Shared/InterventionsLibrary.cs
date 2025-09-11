// <copyright file="InterventionsLibrary.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

/// <summary>
/// Provides reference data for treatment interventions and exercises organized by category.
/// </summary>
public static class InterventionsLibrary
{
  /// <summary>
  /// Gets a list of treatment categories available for intervention planning.
  /// </summary>
  public static readonly List<string> TreatmentCategories = new()
    {
        "ROM – Active, Passive, AAROM",
        "Joint Mobilization – Grades I–V",
        "Soft Tissue Mobilization",
        "Muscle Strengthening (Isolated or Functional)",
        "Core Stabilization",
        "Neuromuscular Re-education",
        "Postural Training",
        "Gait & Balance Training",
        "Manual Therapy",
        "Pelvic Floor Therapy",
        "Pain Neuroscience Education",
        "HEP Instruction",
    };

  /// <summary>
  /// Gets a dictionary of exercises organized by body region.
  /// </summary>
  public static readonly Dictionary<string, List<string>> ExerciseLibrary = new()
  {
    ["Neck"] = new() { "Chin tucks", "Cervical isometrics", "Scapular retraction with band" },
    ["Shoulder"] = new() { "Wall slides", "Pendulum swings", "External rotation with band" },
    ["Lumbar"] = new() { "Bird dog", "Prone press-ups", "Bridges" },
    ["Hip"] = new() { "Clamshells", "Hip flexor stretch", "Glute bridges" },
    ["Knee"] = new() { "Step-ups", "Terminal knee extension", "Wall sits" },
    ["Ankle"] = new() { "Calf raises", "Single-leg balance", "Towel scrunches" },
    ["Pelvic"] = new() { "Pelvic tilts", "Core-lumbopelvic coordination", "Hip adduction squeeze" },
  };
}
