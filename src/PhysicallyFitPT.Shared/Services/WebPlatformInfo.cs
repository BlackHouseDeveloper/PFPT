// <copyright file="WebPlatformInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared.Services;

using PhysicallyFitPT.Shared;

/// <summary>
/// Default web platform information implementation.
/// This will be replaced by platform-specific implementations in the hosting projects.
/// </summary>
public class WebPlatformInfo : IPlatformInfo
{
  /// <inheritdoc/>
  public PlatformType Platform => PlatformType.Web;

  /// <inheritdoc/>
  public DeviceType Device => DetermineDeviceType();

  /// <inheritdoc/>
  public bool SupportsOffline => true; // PWA support

  /// <inheritdoc/>
  public bool SupportsPushNotifications => true; // Web Push API

  /// <inheritdoc/>
  public bool HasNativeFileAccess => false; // Limited to File API

  /// <inheritdoc/>
  public int? ScreenWidth => null; // Would need JS interop to determine

  /// <inheritdoc/>
  public int? ScreenHeight => null; // Would need JS interop to determine

  private static DeviceType DetermineDeviceType()
  {
    // Default to desktop for web, would normally use JS interop for detection
    return DeviceType.Desktop;
  }
}
