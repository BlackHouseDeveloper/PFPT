// <copyright file="MobileDetectionService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared.Components.UI.Sidebar;

/// <summary>
/// Service to manage mobile viewport detection.
/// </summary>
public class MobileDetectionService
{
    private bool isMobile = false;

    /// <summary>
    /// Event raised when the mobile status changes.
    /// </summary>
    public event Action? OnMobileStatusChanged;

    /// <summary>
    /// Gets a value indicating whether the viewport is mobile sized.
    /// </summary>
    public bool IsMobile => this.isMobile;

    /// <summary>
    /// Set the mobile status and notify listeners when it changes.
    /// </summary>
    /// <param name="isMobile">New mobile status.</param>
    public void SetMobileStatus(bool isMobile)
    {
        if (this.isMobile != isMobile)
        {
            this.isMobile = isMobile;
            this.OnMobileStatusChanged?.Invoke();
        }
    }
}
