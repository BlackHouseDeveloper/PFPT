// <copyright file="DeliveryMethod.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

/// <summary>
/// Specifies the method for delivering messages to patients.
/// </summary>
public enum DeliveryMethod
{
    /// <summary>
    /// Short Message Service (SMS) text message.
    /// </summary>
    SMS = 0,

    /// <summary>
    /// Electronic mail (email) message.
    /// </summary>
    Email = 1,
}
