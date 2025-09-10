// <copyright file="MainApplication.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT;

using Android.App;
using Android.Runtime;

[Application]
public class MainApplication : MauiApplication
{
  public MainApplication(IntPtr handle, JniHandleOwnership ownership)
    : base(handle, ownership)
  {
  }

  /// <inheritdoc/>
  protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
