// <copyright file="GaitDeviations.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core;

/// <summary>
/// Gait deviation patterns commonly observed in physical therapy.
/// </summary>
public static class GaitDeviations
{
    /// <summary>
    /// Gets all standard gait deviation descriptions.
    /// </summary>
    public static readonly List<string> All = new()
    {
        "Antalgic gait",
        "Trendelenburg gait",
        "Steppage gait",
        "Ataxic gait",
        "Decreased stride length",
        "Decreased step width",
        "Reduced arm swing",
        "Forward trunk lean",
        "Lateral trunk lean",
        "Knee hyperextension",
        "Foot drop",
        "Hip hiking",
        "Circumduction",
        "Normal gait pattern",
    };
}
