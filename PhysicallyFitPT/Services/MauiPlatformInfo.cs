// <copyright file="MauiPlatformInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Services;

using System;
using Microsoft.Maui.Devices;
using IPlatformInfo = PhysicallyFitPT.Shared.IPlatformInfo;
using SharedDeviceType = PhysicallyFitPT.Shared.DeviceType;
using SharedPlatformType = PhysicallyFitPT.Shared.PlatformType;

/// <summary>
/// Provides platform and device metadata for the MAUI host so Blazor layouts can adapt responsively.
/// </summary>
public sealed class MauiPlatformInfo : IPlatformInfo
{
  /// <inheritdoc />
  public SharedPlatformType Platform
  {
    get
    {
      var platform = DeviceInfo.Current.Platform;

      if (platform == DevicePlatform.Android || platform == DevicePlatform.iOS)
      {
        return SharedPlatformType.Mobile;
      }

      if (platform == DevicePlatform.WinUI || platform == DevicePlatform.macOS || platform == DevicePlatform.MacCatalyst)
      {
        return SharedPlatformType.Desktop;
      }

      return SharedPlatformType.Mobile;
    }
  }

  /// <inheritdoc />
  public SharedDeviceType Device
  {
    get
    {
      var idiom = DeviceInfo.Current.Idiom;

      if (idiom == DeviceIdiom.Desktop)
      {
        return SharedDeviceType.Desktop;
      }

      if (idiom == DeviceIdiom.Tablet)
      {
        return SharedDeviceType.Tablet;
      }

      return SharedDeviceType.Mobile;
    }
  }

  /// <inheritdoc />
  public bool SupportsOffline => true;

  /// <inheritdoc />
  public bool SupportsPushNotifications
  {
    get
    {
      var platform = DeviceInfo.Current.Platform;
      return platform == DevicePlatform.Android || platform == DevicePlatform.iOS;
    }
  }

  /// <inheritdoc />
  public bool HasNativeFileAccess
  {
    get
    {
      var platform = DeviceInfo.Current.Platform;
      return platform == DevicePlatform.WinUI || platform == DevicePlatform.macOS || platform == DevicePlatform.MacCatalyst;
    }
  }

  /// <inheritdoc />
  public int? ScreenWidth => TryGetDisplayDimension(static info => info.Width);

  /// <inheritdoc />
  public int? ScreenHeight => TryGetDisplayDimension(static info => info.Height);

  private static int? TryGetDisplayDimension(Func<DisplayInfo, double> selector)
  {
    try
    {
      var info = DeviceDisplay.Current.MainDisplayInfo;
      if (info.Density <= 0)
      {
        return (int)Math.Round(selector(info));
      }

      return (int)Math.Round(selector(info) / info.Density);
    }
    catch
    {
      return null;
    }
  }
}
