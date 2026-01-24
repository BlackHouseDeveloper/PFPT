// <copyright file="SidebarContextValue.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared.Components.UI.Sidebar;

/// <summary>
/// Context for sidebar state management across components.
/// </summary>
public class SidebarContextValue
{
    /// <summary>
    /// Gets or sets the current sidebar state.
    /// </summary>
    public SidebarState State { get; set; } = SidebarState.Expanded;

    /// <summary>
    /// Gets or sets a value indicating whether the sidebar is open.
    /// </summary>
    public bool Open { get; set; } = true;

    /// <summary>
    /// Gets or sets a setter for desktop open state.
    /// </summary>
    public Action<bool>? SetOpen { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the mobile sidebar is open.
    /// </summary>
    public bool OpenMobile { get; set; } = false;

    /// <summary>
    /// Gets or sets a setter for mobile open state.
    /// </summary>
    public Action<bool>? SetOpenMobile { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the viewport is mobile.
    /// </summary>
    public bool IsMobile { get; set; } = false;

    /// <summary>
    /// Gets or sets a toggle action for the sidebar.
    /// </summary>
    public Action? ToggleSidebar { get; set; }
}
