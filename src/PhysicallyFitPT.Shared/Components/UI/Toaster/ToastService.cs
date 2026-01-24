// <copyright file="ToastService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File contains multiple types.
#pragma warning disable SA1649 // File name matches first type.

namespace PhysicallyFitPT.Shared.Components.UI.Toaster;

/// <summary>
/// Toast notification type.
/// </summary>
public enum ToastType
{
    /// <summary>
    /// Neutral toast without special styling.
    /// </summary>
    Default,

    /// <summary>
    /// Success toast style.
    /// </summary>
    Success,

    /// <summary>
    /// Error toast style.
    /// </summary>
    Error,

    /// <summary>
    /// Informational toast style.
    /// </summary>
    Info,

    /// <summary>
    /// Warning toast style.
    /// </summary>
    Warning,

    /// <summary>
    /// Loading toast style.
    /// </summary>
    Loading,
}

/// <summary>
/// Toast notification data.
/// </summary>
public class Toast
{
    /// <summary>
    /// Gets or sets the unique toast identifier.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the toast body message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the toast visual type.
    /// </summary>
    public ToastType Type { get; set; } = ToastType.Default;

    /// <summary>
    /// Gets or sets the toast title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets an optional description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the duration in milliseconds before auto-close.
    /// </summary>
    public double Duration { get; set; } = 4000; // milliseconds

    /// <summary>
    /// Gets or sets a value indicating whether the toast auto closes.
    /// </summary>
    public bool AutoClose { get; set; } = true;

    /// <summary>
    /// Gets or sets the created-at timestamp in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Service for managing toast notifications globally.
/// </summary>
public class ToastService
{
    private readonly List<Toast> toasts = new();

    /// <summary>
    /// Event raised when a toast is added.
    /// </summary>
    public event Action? OnToastAdded;

    /// <summary>
    /// Event raised when a toast is removed.
    /// </summary>
    public event Action? OnToastRemoved;

    /// <summary>
    /// Event raised when all toasts are cleared.
    /// </summary>
    public event Action? OnToastsClear;

    /// <summary>
    /// Gets the current collection of toasts.
    /// </summary>
    public IReadOnlyList<Toast> Toasts => this.toasts.AsReadOnly();

    /// <summary>
    /// Show a default toast notification.
    /// </summary>
    /// <param name="message">Body text for the toast.</param>
    /// <param name="title">Optional title; falls back to type-specific labels for helpers.</param>
    /// <param name="type">Toast visual type.</param>
    /// <param name="duration">Milliseconds before auto-dismiss; 0 means persistent.</param>
    /// <returns>Identifier for the created toast.</returns>
    public string Show(string message, string? title = null, ToastType type = ToastType.Default, double duration = 4000)
    {
        var toast = new Toast
        {
            Message = message,
            Title = title,
            Type = type,
            Duration = duration,
        };

        this.toasts.Add(toast);
        this.OnToastAdded?.Invoke();

        return toast.Id;
    }

    /// <summary>
    /// Show a success toast.
    /// </summary>
    /// <param name="message">Body text for the toast.</param>
    /// <param name="title">Optional title.</param>
    /// <param name="duration">Milliseconds before auto-dismiss.</param>
    /// <returns>Identifier for the created toast.</returns>
    public string Success(string message, string? title = null, double duration = 4000)
        => this.Show(message, title ?? "Success", ToastType.Success, duration);

    /// <summary>
    /// Show an error toast.
    /// </summary>
    /// <param name="message">Body text for the toast.</param>
    /// <param name="title">Optional title.</param>
    /// <param name="duration">Milliseconds before auto-dismiss.</param>
    /// <returns>Identifier for the created toast.</returns>
    public string Error(string message, string? title = null, double duration = 4000)
        => this.Show(message, title ?? "Error", ToastType.Error, duration);

    /// <summary>
    /// Show an info toast.
    /// </summary>
    /// <param name="message">Body text for the toast.</param>
    /// <param name="title">Optional title.</param>
    /// <param name="duration">Milliseconds before auto-dismiss.</param>
    /// <returns>Identifier for the created toast.</returns>
    public string Info(string message, string? title = null, double duration = 4000)
        => this.Show(message, title ?? "Info", ToastType.Info, duration);

    /// <summary>
    /// Show a warning toast.
    /// </summary>
    /// <param name="message">Body text for the toast.</param>
    /// <param name="title">Optional title.</param>
    /// <param name="duration">Milliseconds before auto-dismiss.</param>
    /// <returns>Identifier for the created toast.</returns>
    public string Warning(string message, string? title = null, double duration = 4000)
        => this.Show(message, title ?? "Warning", ToastType.Warning, duration);

    /// <summary>
    /// Show a loading toast.
    /// </summary>
    /// <param name="message">Body text for the toast.</param>
    /// <param name="title">Optional title.</param>
    /// <returns>Identifier for the created toast.</returns>
    public string Loading(string message, string? title = null)
        => this.Show(message, title ?? "Loading", ToastType.Loading, 0);

    /// <summary>
    /// Remove a toast by ID.
    /// </summary>
    /// <param name="toastId">Identifier of the toast to remove.</param>
    public void Remove(string toastId)
    {
        this.toasts.RemoveAll(t => t.Id == toastId);
        this.OnToastRemoved?.Invoke();
    }

    /// <summary>
    /// Clear all toasts.
    /// </summary>
    public void Clear()
    {
        this.toasts.Clear();
        this.OnToastsClear?.Invoke();
    }

    /// <summary>
    /// Update an existing toast.
    /// </summary>
    /// <param name="toastId">Identifier of the toast to update.</param>
    /// <param name="message">New message body.</param>
    /// <param name="title">Optional new title.</param>
    /// <param name="type">Optional new toast type.</param>
    public void Update(string toastId, string message, string? title = null, ToastType? type = null)
    {
        var toast = this.toasts.FirstOrDefault(t => t.Id == toastId);
        if (toast != null)
        {
            toast.Message = message;
            if (title != null)
            {
                toast.Title = title;
            }

            if (type.HasValue)
            {
                toast.Type = type.Value;
            }

            this.OnToastAdded?.Invoke();
        }
    }
}

#pragma warning restore SA1402, SA1649
