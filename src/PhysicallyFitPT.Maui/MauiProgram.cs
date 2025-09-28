// <copyright file="MauiProgram.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT
{

  using System;
  using System.IO;
  using Microsoft.AspNetCore.Components.WebView.Maui;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.DependencyInjection; // needed for AddHttpClient ext method
  using Microsoft.Extensions.Logging;
  using Microsoft.Maui.Controls.Hosting;
  using Microsoft.Maui.Hosting;
  using Microsoft.Maui.Storage;
  using PhysicallyFitPT.Infrastructure.Data;
  using PhysicallyFitPT.Infrastructure.Services;
  using PhysicallyFitPT.Infrastructure.Services.Interfaces;
  using PhysicallyFitPT.Services;
  using PhysicallyFitPT.Shared;

  /// <summary>
  /// Provides methods for configuring and creating the MAUI application.
  /// </summary>
  public static class MauiProgram
  {
#if ANDROID || IOS || MACCATALYST || WINDOWS
    private static bool uiAssemblyHooked;
#endif

    /// <summary>
    /// Creates and configures the MAUI application instance.
    /// </summary>
    /// <returns>The configured <see cref="MauiApp"/>.</returns>
    public static MauiApp CreateMauiApp()
    {
      var builder = MauiApp.CreateBuilder();
      builder
        .UseMauiApp<App>()
        .ConfigureFonts(fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));

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
      builder.Services.AddScoped<IQuestionnaireService, QuestionnaireService>();
      builder.Services.AddScoped<IDashboardMetricsService, DashboardMetricsService>();
      builder.Services.AddScoped<IDataService, LocalDataService>();
      builder.Services.AddSingleton<IPdfRenderer, PdfRenderer>();
      builder.Services.AddSingleton<IPlatformInfo, MauiPlatformInfo>();

      builder.Services.AddHttpClient(); // if you just need the default factory
      builder.Services.AddHttpClient("integrations");
      builder.Services.AddMauiBlazorWebView();

      builder.Logging.AddDebug();
      builder.Logging.AddFilter("Microsoft.AspNetCore.Components.WebView", LogLevel.Debug);

#if DEBUG
      builder.Services.AddBlazorWebViewDeveloperTools();
#endif

      EnsureUiAssemblyBinding();

      return builder.Build();
    }

    private static void EnsureUiAssemblyBinding()
    {
#if ANDROID || IOS || MACCATALYST || WINDOWS
      if (uiAssemblyHooked)
      {
        return;
      }

      AppDomain.CurrentDomain.AssemblyResolve += static (_, args) =>
      {
        // Handle legacy references to PhysicallyFitPT.UI (now merged into PhysicallyFitPT.Shared)
        if (args.Name.StartsWith("PhysicallyFitPT.UI", StringComparison.Ordinal))
        {
          return typeof(App).Assembly;
        }

        return null;
      };

      uiAssemblyHooked = true;
#endif
    }
  }
}
