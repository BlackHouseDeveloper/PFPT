// <copyright file="SpecialTestResult.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

/// <summary>
/// Specifies the result of a special clinical test.
/// </summary>
public enum SpecialTestResult
{
    /// <summary>
    /// The test was not performed.
    /// </summary>
    NotPerformed = 0,

    /// <summary>
    /// The test result was negative (normal).
    /// </summary>
    Negative = 1,

    /// <summary>
    /// The test result was positive (abnormal).
    /// </summary>
    Positive = 2,

    /// <summary>
    /// The test result was inconclusive or unclear.
    /// </summary>
    Inconclusive = 3,
}
