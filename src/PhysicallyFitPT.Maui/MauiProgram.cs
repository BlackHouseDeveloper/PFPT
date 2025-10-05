// <copyright file="MauiProgram.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT
{
  using System;
  using System.IO;
  using System.Net.Http;
  using System.Net.Security;
  using Microsoft.AspNetCore.Components.WebView.Maui;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.DependencyInjection; // needed for AddHttpClient ext method
  using Microsoft.Extensions.Logging;
  using Microsoft.Maui.Controls.Hosting;
  using Microsoft.Maui.Hosting;
  using Microsoft.Maui.Storage;
  using PhysicallyFitPT.Infrastructure.Data;
  using PhysicallyFitPT.Infrastructure.Services;
  using PhysicallyFitPT.Infrastructure.Services.Configuration;
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

      ApiRoutes.ConfigureBasePath(Environment.GetEnvironmentVariable("PFP_API_BASEPATH"));

      var appStatsCacheTtl = Environment.GetEnvironmentVariable("PFP_APPSTATS_CACHE_TTL_SECONDS");
      var apiBaseUri = ResolveApiBaseUri(Environment.GetEnvironmentVariable("PFP_API_BASEURL"));
      var apiTimeoutSeconds = ResolveTimeoutSeconds(Environment.GetEnvironmentVariable("PFP_API_TIMEOUT_SECONDS"), 30);

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
      builder.Services.AddSingleton<IAppStatsService, AppStatsService>();
      builder.Services.AddSingleton<IAppStatsInvalidator>(sp => (IAppStatsInvalidator)sp.GetRequiredService<IAppStatsService>());
      builder.Services.AddSingleton<ISyncService, HybridSyncService>();

      builder.Services.AddHttpClient(); // if you just need the default factory
      builder.Services.AddHttpClient("integrations");
      builder.Services.AddHttpClient("api", client =>
      {
        client.BaseAddress = apiBaseUri;
        client.Timeout = TimeSpan.FromSeconds(apiTimeoutSeconds);
      })
        .ConfigurePrimaryHttpMessageHandler(() => CreateHttpMessageHandler(apiBaseUri));
      builder.Services.AddMauiBlazorWebView();

      builder.Services.AddMemoryCache();
      builder.Services.Configure<AppStatsOptions>(options =>
      {
        if (int.TryParse(appStatsCacheTtl, out var ttlSeconds) && ttlSeconds > 0)
        {
          options.CacheTtlSeconds = Math.Clamp(ttlSeconds, 1, 300);
        }
      });

      builder.Logging.AddDebug();
      builder.Logging.AddFilter("Microsoft.AspNetCore.Components.WebView", LogLevel.Debug);

#if DEBUG
      builder.Services.AddBlazorWebViewDeveloperTools();
#endif

      EnsureUiAssemblyBinding();

      return builder.Build();
    }

    private static Uri ResolveApiBaseUri(string? configuredBaseUrl)
    {
      if (Uri.TryCreate(configuredBaseUrl, UriKind.Absolute, out var uri))
      {
        return uri;
      }

#if DEBUG
      return new Uri(GetDefaultDebugApiBaseUrl());
#else
      return new Uri("https://api.physicallyfitpt.com");
#endif
    }

    private static int ResolveTimeoutSeconds(string? configuredTimeout, int defaultValue)
    {
      return int.TryParse(configuredTimeout, out var value) && value > 0
        ? value
        : defaultValue;
    }

#if DEBUG
    private static string GetDefaultDebugApiBaseUrl()
    {
      if (OperatingSystem.IsAndroid())
      {
        return "http://10.0.2.2:5114/";
      }

      if (OperatingSystem.IsIOS())
      {
        return "http://127.0.0.1:5114/";
      }

      if (OperatingSystem.IsMacCatalyst() || OperatingSystem.IsWindows())
      {
        return "http://localhost:5114/";
      }

      return "http://localhost:5114/";
    }
#endif

    private static HttpClientHandler CreateHttpMessageHandler(Uri apiBaseUri)
    {
      var handler = new HttpClientHandler();

#if DEBUG
      if (string.Equals(apiBaseUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
      {
        handler.ServerCertificateCustomValidationCallback = (_, _, _, errors) =>
        {
          if (errors == SslPolicyErrors.None)
          {
            return true;
          }

          return ShouldRelaxCertificateValidation(apiBaseUri);
        };
      }
#endif

      return handler;
    }

#if DEBUG
    private static bool ShouldRelaxCertificateValidation(Uri uri)
    {
      if (!uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
      {
        return false;
      }

      if (uri.IsLoopback)
      {
        return true;
      }

      var host = uri.Host;
      return string.Equals(host, "10.0.2.2", StringComparison.Ordinal)
        || string.Equals(host, "10.0.3.2", StringComparison.Ordinal);
    }
#endif

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
