

namespace PhysicallyFitPT.Platforms.Tizen
{
    using Microsoft.Maui;
    using Microsoft.Maui.Hosting;

    /// <summary>
    /// The main application class for the Tizen platform.
    /// </summary>
    public class Program : MauiApplication
    {
        /// <inheritdoc/>
        protected override static MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
