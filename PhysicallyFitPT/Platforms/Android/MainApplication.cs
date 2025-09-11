// <copyright file="MainApplication.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT;

using Android.App;
using Android.Runtime;

/// <summary>
/// Represents the main Android application class.
/// </summary>
[Application]
public class MainApplication : MauiApplication
{
  /// <summary>
  /// Initializes a new instance of the <see cref="MainApplication"/> class.
  /// </summary>
  /// <param name="handle">The handle to the Android application instance.</param>
  /// <param name="ownership">The ownership type for the JNI handle.</param>
  public MainApplication(IntPtr handle, JniHandleOwnership ownership)
    : base(handle, ownership)
  {
  }

  /// <inheritdoc/>
  protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
