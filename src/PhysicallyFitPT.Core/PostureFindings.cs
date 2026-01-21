// <copyright file="PostureFindings.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core;

/// <summary>
/// Common postural assessment findings.
/// </summary>
public static class PostureFindings
{
    /// <summary>
    /// Gets all standard posture finding descriptions.
    /// </summary>
    public static readonly List<string> All = new()
    {
        "Forward head posture",
        "Rounded shoulders",
        "Increased thoracic kyphosis",
        "Increased lumbar lordosis",
        "Decreased lumbar lordosis",
        "Lateral shift (left/right)",
        "Pelvic tilt (anterior/posterior)",
        "Leg length discrepancy",
        "Scoliosis",
        "Normal postural alignment",
    };
}
