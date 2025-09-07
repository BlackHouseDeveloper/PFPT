using System.IO;
using Microsoft.Maui.Storage;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection; // needed for AddHttpClient ext method


namespace PhysicallyFitPT;

public static class MauiProgram
{
  public static MauiApp CreateMauiApp()
  {
    var builder = MauiApp.CreateBuilder();
    builder
      .UseMauiApp<App>()
      .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

#if DEBUG
    builder.Services.AddBlazorWebViewDeveloperTools();
    builder.Logging.AddDebug();
#endif

    SQLitePCL.Batteries_V2.Init();

    var envPath = Environment.GetEnvironmentVariable("PFP_DB_PATH");
    string dbPath = !string.IsNullOrWhiteSpace(envPath)
      ? envPath!
      : Path.Combine(FileSystem.AppDataDirectory, "physicallyfitpt.db");

    builder.Services.AddDbContextFactory<ApplicationDbContext>(opt => opt.UseSqlite($"Data Source={dbPath}"));

    // DI: services
    builder.Services.AddScoped<IPatientService, PatientService>();
    builder.Services.AddScoped<IAppointmentService, AppointmentService>();
    builder.Services.AddScoped<IAutoMessagingService, AutoMessagingService>();
    builder.Services.AddScoped<INoteBuilderService, NoteBuilderService>();
    builder.Services.AddSingleton<IPdfRenderer, PdfRenderer>();

    builder.Services.AddHttpClient(); // if you just need the default factory
    builder.Services.AddHttpClient("integrations");
    builder.Services.AddMauiBlazorWebView();

    builder.Logging.AddDebug();
    builder.Logging.AddFilter("Microsoft.AspNetCore.Components.WebView", LogLevel.Debug);

#if DEBUG
    builder.Services.AddBlazorWebViewDeveloperTools();
#endif


    return builder.Build();
  }
}
