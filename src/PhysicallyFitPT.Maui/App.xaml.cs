// <copyright file="App.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT;

using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using PhysicallyFitPT.Shared;

/// <summary>
/// Represents the main application class for the MAUI application.
/// </summary>
public partial class App : Application
{
  private readonly ISyncService syncService;

  /// <summary>
  /// Initializes a new instance of the <see cref="App"/> class.
  /// </summary>
  /// <param name="syncService">Hybrid sync service used to keep offline data fresh.</param>
  public App(ISyncService syncService)
  {
    this.syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
    this.InitializeComponent();

    this.syncService.StartPeriodicSync(5);
    _ = Task.Run(() => this.syncService.SyncAsync());
  }

  /// <inheritdoc/>
  protected override Window CreateWindow(IActivationState? activationState)
  {
    return new Window(new MainPage()) { Title = "PhysicallyFitPT" };
  }
}
