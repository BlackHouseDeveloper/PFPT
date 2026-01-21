// <copyright file="UIContexts.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may contain multiple public types (context classes).
#pragma warning disable SA1649 // File name matches first type (multi-context container).

namespace PhysicallyFitPT.Shared.Components.UI;

/// <summary>
/// Context for Tabs component providing state management for tab selection.
/// </summary>
public class TabsContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TabsContext"/> class.
    /// </summary>
    /// <param name="defaultValue">Initial tab value.</param>
    public TabsContext(string defaultValue)
    {
        this.ActiveValue = defaultValue;
    }

    /// <summary>
    /// Event raised when the tab value changes.
    /// </summary>
    public event Action? OnValueChanged;

    /// <summary>
    /// Gets or sets the current active tab value.
    /// </summary>
    public string ActiveValue { get; set; }

    /// <summary>
    /// Update the active tab value and notify listeners when it changes.
    /// </summary>
    /// <param name="value">New active tab value.</param>
    public void SetValue(string value)
    {
        if (this.ActiveValue != value)
        {
            this.ActiveValue = value;
            this.OnValueChanged?.Invoke();
        }
    }

    /// <summary>
    /// Compatibility method for legacy callers expecting SetActive.
    /// Forwards to SetValue.
    /// </summary>
    /// <param name="value">New active tab value.</param>
    public void SetActive(string value) => this.SetValue(value);
}

/// <summary>
/// Context for ToggleGroup component providing variant and size settings.
/// </summary>
public class ToggleGroupContext
{
    /// <summary>
    /// Event raised when a toggle value changes.
    /// </summary>
    public event Action? OnValueChanged;

    /// <summary>
    /// Gets or sets the visual variant applied to toggle group items.
    /// </summary>
    public string Variant { get; set; } = "default";

    /// <summary>
    /// Gets or sets the size applied to toggle group items.
    /// </summary>
    public string Size { get; set; } = "default";

    /// <summary>
    /// Notify listeners that the toggle value has changed.
    /// </summary>
    public void NotifyValueChanged()
    {
        this.OnValueChanged?.Invoke();
    }
}

/// <summary>
/// Context for Sidebar component providing state management for sidebar visibility.
/// </summary>
public class SidebarContextValue
{
    /// <summary>
    /// Event raised when sidebar state changes.
    /// </summary>
    public event Action? OnStateChanged;

    /// <summary>
    /// Gets or sets a value indicating whether the sidebar is open.
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// Update the open state and notify listeners when it changes.
    /// </summary>
    /// <param name="open">New open state.</param>
    public void SetOpen(bool open)
    {
        if (this.IsOpen != open)
        {
            this.IsOpen = open;
            this.OnStateChanged?.Invoke();
        }
    }
}

/// <summary>
/// Context for Toast/Toaster component providing notification state.
/// </summary>
public class ToastContext
{
    /// <summary>
    /// Event raised when a toast is added.
    /// </summary>
    public event Action? OnToastAdded;

    /// <summary>
    /// Event raised when a toast is removed.
    /// </summary>
    public event Action? OnToastRemoved;

    /// <summary>
    /// Notify listeners that a toast has been added.
    /// </summary>
    public void NotifyToastAdded()
    {
        this.OnToastAdded?.Invoke();
    }

    /// <summary>
    /// Notify listeners that a toast has been removed.
    /// </summary>
    public void NotifyToastRemoved()
    {
        this.OnToastRemoved?.Invoke();
    }
}

/// <summary>
/// Context for Dialog component providing modal state and controls.
/// </summary>
public class DialogContext
{
    /// <summary>
    /// Event raised when dialog state changes.
    /// </summary>
    public event Action? OnStateChanged;

    /// <summary>
    /// Gets or sets a value indicating whether the dialog is open.
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// Update the open state and notify listeners when it changes.
    /// </summary>
    /// <param name="open">New open state.</param>
    public void SetOpen(bool open)
    {
        if (this.IsOpen != open)
        {
            this.IsOpen = open;
            this.OnStateChanged?.Invoke();
        }
    }
}

#pragma warning restore SA1402, SA1649
