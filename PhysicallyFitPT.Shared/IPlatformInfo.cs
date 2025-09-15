// <copyright file="IPlatformInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

/// <summary>
/// Platform identification enumeration for UI adaptation.
/// </summary>
public enum PlatformType
{
  /// <summary>
  /// Web browser platform (Blazor WebAssembly).
  /// </summary>
  Web,

  /// <summary>
  /// MAUI desktop application.
  /// </summary>
  Desktop,

  /// <summary>
  /// MAUI mobile application (Android/iOS).
  /// </summary>
  Mobile,
}

/// <summary>
/// Device type enumeration for responsive UI design.
/// </summary>
public enum DeviceType
{
  /// <summary>
  /// Desktop computer with large screen.
  /// </summary>
  Desktop,

  /// <summary>
  /// Tablet device with medium screen.
  /// </summary>
  Tablet,

  /// <summary>
  /// Mobile phone with small screen.
  /// </summary>
  Mobile,
}

/// <summary>
/// Interface for providing platform-specific information to enable responsive UI adaptation.
/// </summary>
public interface IPlatformInfo
{
  /// <summary>
  /// Gets the current platform type.
  /// </summary>
  PlatformType Platform { get; }

  /// <summary>
  /// Gets the current device type for responsive design.
  /// </summary>
  DeviceType Device { get; }

  /// <summary>
  /// Gets a value indicating whether the current platform supports offline functionality.
  /// </summary>
  bool SupportsOffline { get; }

  /// <summary>
  /// Gets a value indicating whether the current platform supports push notifications.
  /// </summary>
  bool SupportsPushNotifications { get; }

  /// <summary>
  /// Gets a value indicating whether the current platform has native file system access.
  /// </summary>
  bool HasNativeFileAccess { get; }

  /// <summary>
  /// Gets the current screen width in pixels (if available).
  /// </summary>
  int? ScreenWidth { get; }

  /// <summary>
  /// Gets the current screen height in pixels (if available).
  /// </summary>
  int? ScreenHeight { get; }
}