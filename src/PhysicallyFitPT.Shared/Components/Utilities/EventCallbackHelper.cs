// <copyright file="EventCallbackHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared.Components.Utilities;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Helper utility to work with EventCallback in components without triggering Razor parser issues.
/// This class provides static extension-like methods to invoke callbacks safely.
/// </summary>
public static class EventCallbackHelper
{
  /// <summary>
  /// Safely invoke an EventCallback&lt;object&gt; with the specified argument.
  /// </summary>
  /// <param name="callback">Callback to invoke.</param>
  /// <param name="arg">Argument to pass to the callback.</param>
  /// <returns>Task tracking the callback invocation.</returns>
  public static async Task InvokeObjectAsync(EventCallback<object> callback, object? arg)
  {
    if (callback.HasDelegate)
    {
      await callback.InvokeAsync(arg);
    }
  }

  /// <summary>
  /// Safely invoke an EventCallback&lt;bool&gt; with the specified argument.
  /// </summary>
  /// <param name="callback">Callback to invoke.</param>
  /// <param name="arg">Argument to pass to the callback.</param>
  /// <returns>Task tracking the callback invocation.</returns>
  public static async Task InvokeBoolAsync(EventCallback<bool> callback, bool arg)
  {
    if (callback.HasDelegate)
    {
      await callback.InvokeAsync(arg);
    }
  }

  /// <summary>
  /// Safely invoke an EventCallback&lt;T&gt; with the specified argument.
  /// </summary>
  /// <typeparam name="T">Callback argument type.</typeparam>
  /// <param name="callback">Callback to invoke.</param>
  /// <param name="arg">Argument to pass to the callback.</param>
  /// <returns>Task tracking the callback invocation.</returns>
  public static async Task InvokeAsync<T>(EventCallback<T> callback, T arg)
  {
    if (callback.HasDelegate)
    {
      await callback.InvokeAsync(arg);
    }
  }

  /// <summary>
  /// Check if an EventCallback&lt;object&gt; has a delegate assigned.
  /// </summary>
  /// <param name="callback">Callback to inspect.</param>
  /// <returns>True when a delegate is attached.</returns>
  public static bool HasDelegate(EventCallback<object> callback) => callback.HasDelegate;

  /// <summary>
  /// Check if an EventCallback&lt;bool&gt; has a delegate assigned.
  /// </summary>
  /// <param name="callback">Callback to inspect.</param>
  /// <returns>True when a delegate is attached.</returns>
  public static bool HasDelegate(EventCallback<bool> callback) => callback.HasDelegate;

  /// <summary>
  /// Check if an EventCallback&lt;T&gt; has a delegate assigned.
  /// </summary>
  /// <typeparam name="T">Callback argument type.</typeparam>
  /// <param name="callback">Callback to inspect.</param>
  /// <returns>True when a delegate is attached.</returns>
  public static bool HasDelegate<T>(EventCallback<T> callback) => callback.HasDelegate;
}
