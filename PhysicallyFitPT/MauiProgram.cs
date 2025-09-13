// <copyright file="MauiProgram.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT;

using System.IO;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;

/// <summary>
/// Provides methods for configuring and creating the MAUI application.
/// </summary>
public static class MauiProgram
{
  /// <summary>
  /// Creates and configures the MAUI application instance.
  /// </summary>
  /// <returns>The configured <see cref="MauiApp"/>.</returns>
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

    // Register SQLite data store for mobile platforms
    builder.Services.AddScoped<IDataStore, SqliteDataStore>();

    // DI: services
    builder.Services.AddScoped<IPatientService, PatientService>();
    builder.Services.AddScoped<IAppointmentService, AppointmentService>();
    builder.Services.AddScoped<IAutoMessagingService, AutoMessagingService>();
    builder.Services.AddScoped<INoteBuilderService, NoteBuilderService>();
    builder.Services.AddScoped<IQuestionnaireService, QuestionnaireService>();
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
