// <copyright file="App.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT;

using Microsoft.Maui;
using Microsoft.Maui.Controls;

/// <summary>
/// Represents the main application class for the MAUI application.
/// </summary>
public partial class App : Application
{
  /// <summary>
  /// Initializes a new instance of the <see cref="App"/> class.
  /// </summary>
  public App()
  {
    this.InitializeComponent();
  }

  /// <inheritdoc/>
  protected override Window CreateWindow(IActivationState? activationState)
  {
    return new Window(new MainPage()) { Title = "PhysicallyFitPT" };
  }
}
