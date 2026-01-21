// <copyright file="ConsentsData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core.Intake;

/// <summary>
/// Consent acknowledgements recorded during intake.
/// </summary>
public record ConsentsData
{
    /// <summary>
    /// Gets or sets a value indicating whether the patient consents to treatment.
    /// </summary>
    public bool ConsentTreatment { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the patient acknowledges privacy policies.
    /// </summary>
    public bool ConsentPrivacy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the patient consents to communication.
    /// </summary>
    public bool ConsentCommunication { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the patient accepts financial responsibility.
    /// </summary>
    public bool ConsentFinancialResponsibility { get; set; }

    /// <summary>
    /// Gets or sets the date and time when consents were signed.
    /// </summary>
    public DateTime ConsentDate { get; set; } = DateTime.UtcNow;
}
