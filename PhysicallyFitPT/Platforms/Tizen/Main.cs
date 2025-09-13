namespace PhysicallyFitPT;

class Program : MauiApplication
{
  /// <inheritdoc/>
  protected override static MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

  static void Main(string[] args)
  {
    var app = new Program();
    app.Run(args);
  }
}
