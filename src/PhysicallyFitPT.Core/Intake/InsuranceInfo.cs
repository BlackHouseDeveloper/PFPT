// <copyright file="InsuranceInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core.Intake;

/// <summary>
/// Insurance coverage details provided during intake.
/// </summary>
public record InsuranceInfo
{
    /// <summary>
    /// Gets or sets a value indicating whether the patient has insurance. Null indicates not answered yet.
    /// </summary>
    public bool? HasInsurance { get; set; }

    /// <summary>
    /// Gets or sets the name of the insurance provider.
    /// </summary>
    public string? InsuranceProvider { get; set; }

    /// <summary>
    /// Gets or sets the insurance policy number.
    /// </summary>
    public string? PolicyNumber { get; set; }

    /// <summary>
    /// Gets or sets the insurance group number.
    /// </summary>
    public string? GroupNumber { get; set; }

    /// <summary>
    /// Gets or sets the insurance member ID.
    /// </summary>
    public string? MemberId { get; set; }

    /// <summary>
    /// Gets or sets the copay amount.
    /// </summary>
    public string? CopayAmount { get; set; }

    /// <summary>
    /// Gets or sets the insurance deductible amount.
    /// </summary>
    public string? Deductible { get; set; }
}
