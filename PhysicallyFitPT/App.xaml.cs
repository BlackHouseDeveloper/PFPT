// <copyright file="App.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT;

using Microsoft.Maui;
using Microsoft.Maui.Controls;

public partial class App : Application
{
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
