// <copyright file="AppDelegate.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Foundation;
using UIKit;

namespace PhysicallyFitPT;

/// <summary>
/// Represents the iOS application delegate.
/// </summary>
[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    /// <summary>
    /// Creates the MAUI application instance.
    /// </summary>
    /// <returns>The configured MAUI application.</returns>
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
