// <copyright file="Side.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

/// <summary>
/// Specifies the anatomical side or laterality for measurements and tests.
/// </summary>
public enum Side
{
    /// <summary>
    /// Not applicable or not specified.
    /// </summary>
    NA = 0,

    /// <summary>
    /// Left side.
    /// </summary>
    Left = 1,

    /// <summary>
    /// Right side.
    /// </summary>
    Right = 2,

    /// <summary>
    /// Both sides (bilateral).
    /// </summary>
    Bilateral = 3,
}
