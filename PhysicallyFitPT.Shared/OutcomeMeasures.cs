// <copyright file="OutcomeMeasures.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

/// <summary>
/// Provides reference data for outcome measures organized by body region.
/// </summary>
public static class OutcomeMeasures
{
  /// <summary>
  /// Gets a dictionary of outcome measures organized by body region.
  /// </summary>
  public static readonly Dictionary<string, List<string>> RegionMeasures = new()
  {
    ["Neck"] = new() { "NDI", "PSFS", "VAS" },
    ["Shoulder"] = new() { "DASH", "SPADI", "QuickDASH" },
    ["LowBack"] = new() { "ODI", "Roland-Morris", "PSFS" },
    ["Hip"] = new() { "LEFS", "HOOS" },
    ["Knee"] = new() { "LEFS", "KOOS", "Lysholm" },
    ["Ankle"] = new() { "FAAM", "LEFS" },
    ["General Balance"] = new() { "TUG", "5xSTS", "BBS", "ABC" },
    ["Whole Body"] = new() { "PSFS", "SF-36", "NPRS" },
  };
}
