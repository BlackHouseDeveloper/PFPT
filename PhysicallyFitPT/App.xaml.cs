// <copyright file="App.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT;
/// <summary>
/// Represents the main application class for the MAUI application.
/// </summary>
public partial class App : Application
/* Unmerged change from project 'PhysicallyFitPT(net8.0-ios)'
Before:
{
  /// <summary>
  /// Initializes a new instance of the <see cref="App"/> class.
  /// </summary>
  public App()
After:
{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
*/

/* Unmerged change from project 'PhysicallyFitPT(net8.0-maccatalyst)'
Before:
{
  /// <summary>
  /// Initializes a new instance of the <see cref="App"/> class.
  /// </summary>
  public App()
After:
{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
*/

{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
    
/* Unmerged change from project 'PhysicallyFitPT(net8.0-ios)'
Before:
    this.InitializeComponent();
  }

  /// <inheritdoc/>
  protected override Window CreateWindow(IActivationState? activationState)
After:
    this.InitializeComponent();
    }

    /// <inheritdoc/>
    protected override Window CreateWindow(IActivationState? activationState)
*/

/* Unmerged change from project 'PhysicallyFitPT(net8.0-maccatalyst)'
Before:
    this.InitializeComponent();
  }

  /// <inheritdoc/>
  protected override Window CreateWindow(IActivationState? activationState)
After:
    this.InitializeComponent();
    }

    /// <inheritdoc/>
    protected override Window CreateWindow(IActivationState? activationState)
*/
    this.InitializeComponent();
    }

    /// <inheritdoc/>
    protected override Window CreateWindow(IActivationState? activationState)
    {
    
/* Unmerged change from project 'PhysicallyFitPT(net8.0-ios)'
Before:
    return new Window(new MainPage()) { Title = "PhysicallyFitPT" };
  }
After:
    return new Window(new MainPage()) { Title = "PhysicallyFitPT" };
    }
*/

/* Unmerged change from project 'PhysicallyFitPT(net8.0-maccatalyst)'
Before:
    return new Window(new MainPage()) { Title = "PhysicallyFitPT" };
  }
After:
    return new Window(new MainPage()) { Title = "PhysicallyFitPT" };
    }
*/
    return new Window(new MainPage()) { Title = "PhysicallyFitPT" };
    }
}
