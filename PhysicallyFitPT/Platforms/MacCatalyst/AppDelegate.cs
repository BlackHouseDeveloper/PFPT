// <copyright file="AppDelegate.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT
{
    using Foundation;
    using UIKit;

    /// <summary>
    /// Represents the macOS Catalyst application delegate.
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
}
