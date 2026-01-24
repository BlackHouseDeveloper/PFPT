// <copyright file="IsMobileService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1201 // Constructor follows event.

namespace PhysicallyFitPT.Shared.Services;

using Microsoft.JSInterop;

/// <summary>
/// Service for detecting mobile viewport size using media queries.
/// Provides reactive mobile state detection with a 768px breakpoint.
/// </summary>
public class IsMobileService : IAsyncDisposable
{
  private const int MobileBreakpoint = 768;
  private readonly IJSRuntime jsRuntime;
  private IJSObjectReference? module;
  private bool isMobile;
  private bool initialized;

  /// <summary>
  /// Event raised when mobile state changes.
  /// </summary>
  public event EventHandler<bool>? OnMobileStateChanged;

  /// <summary>
  /// Initializes a new instance of the <see cref="IsMobileService"/> class.
  /// </summary>
  /// <param name="jsRuntime">JS runtime used for interop.</param>
  public IsMobileService(IJSRuntime jsRuntime)
  {
    this.jsRuntime = jsRuntime;
  }

  /// <summary>
  /// Gets a value indicating whether the current viewport is mobile-sized (less than 768px).
  /// </summary>
  public bool IsMobile => this.isMobile;

  /// <summary>
  /// Gets a value indicating whether the service has been initialized.
  /// </summary>
  public bool IsInitialized => this.initialized;

  /// <summary>
  /// Initializes the mobile detection service with media query listener.
  /// Should be called once when the application starts or in a component's OnInitializedAsync.
  /// </summary>
  /// <returns>Task tracking initialization completion.</returns>
  public async Task InitializeAsync()
  {
    if (this.initialized)
    {
      return;
    }

    try
    {
      this.module = await this.jsRuntime.InvokeAsync<IJSObjectReference>(
          "import", "./Services/is-mobile.js");

      this.isMobile = await this.module.InvokeAsync<bool>("getIsMobile", MobileBreakpoint);
      this.initialized = true;

      // Set up the media query listener
      await this.module.InvokeVoidAsync(
          "setupMobileListener",
          MobileBreakpoint,
          DotNetObjectReference.Create(this));
    }
    catch (Exception ex)
    {
      Console.Error.WriteLine($"Failed to initialize IsMobileService: {ex.Message}");
      this.initialized = true; // Mark as initialized even on error to prevent retry loops
    }
  }

  /// <summary>
  /// Called from JavaScript when the media query state changes.
  /// </summary>
  /// <param name="newIsMobile">Updated mobile state.</param>
  [JSInvokable]
  public void OnMediaQueryChange(bool newIsMobile)
  {
    if (this.isMobile != newIsMobile)
    {
      this.isMobile = newIsMobile;
      this.OnMobileStateChanged?.Invoke(this, newIsMobile);
    }
  }

  /// <summary>
  /// Disposes the service and cleans up JavaScript resources.
  /// </summary>
  /// <returns>Task tracking disposal.</returns>
  public async ValueTask DisposeAsync()
  {
    if (this.module is not null)
    {
      try
      {
        await this.module.InvokeVoidAsync("cleanupMobileListener");
        await this.module.DisposeAsync();
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error disposing IsMobileService: {ex.Message}");
      }
    }
  }
}

#pragma warning restore SA1201
