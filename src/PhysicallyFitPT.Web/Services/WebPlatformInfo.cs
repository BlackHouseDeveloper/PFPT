// <copyright file="WebPlatformInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Web.Services;

using PhysicallyFitPT.Shared;

/// <summary>
/// Web platform-specific information implementation.
/// </summary>
public class WebPlatformInfo : IPlatformInfo
{
  /// <inheritdoc/>
  public PlatformType Platform => PlatformType.Web;

  /// <inheritdoc/>
  public DeviceType Device => DeviceType.Desktop; // Default, could be enhanced with JS interop

  /// <inheritdoc/>
  public bool SupportsOffline => true; // PWA capabilities

  /// <inheritdoc/>
  public bool SupportsPushNotifications => true; // Web Push API

  /// <inheritdoc/>
  public bool HasNativeFileAccess => false; // Limited to File API

  /// <inheritdoc/>
  public int? ScreenWidth => null; // Would require JS interop

  /// <inheritdoc/>
  public int? ScreenHeight => null; // Would require JS interop
}
