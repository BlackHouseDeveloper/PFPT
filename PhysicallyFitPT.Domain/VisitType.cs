// <copyright file="VisitType.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

/// <summary>
/// Specifies the type of physical therapy visit.
/// </summary>
public enum VisitType
{
    /// <summary>
    /// Initial evaluation visit.
    /// </summary>
    Eval = 0,

    /// <summary>
    /// Daily treatment visit.
    /// </summary>
    Daily = 1,

    /// <summary>
    /// Progress evaluation visit.
    /// </summary>
    Progress = 2,

    /// <summary>
    /// Discharge visit.
    /// </summary>
    Discharge = 3,
}
