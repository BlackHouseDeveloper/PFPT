// <copyright file="SeederHost.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Seeder.Configuration;
using PhysicallyFitPT.Seeder.Seeding;
using PhysicallyFitPT.Seeder.Utils;

namespace PhysicallyFitPT.Seeder;

/// <summary>
/// Host builder and service configuration for the seeder application.
/// </summary>
public static class SeederHost
{
  /// <summary>
  /// Creates and configures the application host.
  /// </summary>
  /// <param name="args">Command line arguments.</param>
  /// <returns>Configured host builder.</returns>
  public static IHostBuilder CreateHostBuilder(string[] args)
  {
    return Host.CreateDefaultBuilder(args)
      .ConfigureAppConfiguration((context, config) =>
      {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        config.AddJsonFile("appsettings.Seeder.json", optional: true, reloadOnChange: false);
        config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
          optional: true, reloadOnChange: false);
        config.AddEnvironmentVariables("PFP_");
      })
      .ConfigureServices((context, services) =>
      {
        var configuration = context.Configuration;

        // Configure options
        services.Configure<SeederOptions>(configuration.GetSection(SeederOptions.SectionName));

        // Configure database
        ConfigureDatabase(services, configuration);

        // Add seeding services
        services.AddSeedingServices();
      })
      .ConfigureLogging((context, logging) =>
      {
        logging.ClearProviders();
        logging.AddConsole();

        // Apply log level from environment variables
        var pfpLogLevel = Environment.GetEnvironmentVariable("PFP_LOGLEVEL");
        if (!string.IsNullOrEmpty(pfpLogLevel) && Enum.TryParse<LogLevel>(pfpLogLevel, true, out var logLevel))
        {
          logging.SetMinimumLevel(logLevel);
        }
      });
  }

  /// <summary>
  /// Configures the database context and connection.
  /// </summary>
  /// <param name="services">Service collection.</param>
  /// <param name="configuration">Configuration.</param>
  private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = GetConnectionString(configuration);

    services.AddDbContext<ApplicationDbContext>(options =>
    {
      options.UseSqlite(connectionString);
    });
  }

  /// <summary>
  /// Gets the database connection string from configuration or environment.
  /// </summary>
  /// <param name="configuration">Configuration.</param>
  /// <returns>Connection string.</returns>
  private static string GetConnectionString(IConfiguration configuration)
  {
    // Priority: PFP_DB_PATH environment variable > configuration > default
    var envPath = Environment.GetEnvironmentVariable("PFP_DB_PATH");
    if (!string.IsNullOrWhiteSpace(envPath))
    {
      return $"Data Source={envPath}";
    }

    var configConnectionString = configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrWhiteSpace(configConnectionString))
    {
      return configConnectionString;
    }

    // Default path
    var defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "dev.physicallyfitpt.db");
    return $"Data Source={defaultPath}";
  }

  /// <summary>
  /// Ensures the database is ready for seeding operations.
  /// </summary>
  /// <param name="serviceProvider">Service provider.</param>
  /// <param name="useMigrations">Whether to use migrations or EnsureCreated.</param>
  /// <returns>Task representing the async operation.</returns>
  public static async Task EnsureDatabaseReadyAsync(IServiceProvider serviceProvider, bool useMigrations = true)
  {
    using var scope = serviceProvider.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
      if (useMigrations)
      {
        logger.LogInformation("Running database migrations...");
        await dbContext.Database.MigrateAsync();
        logger.LogDebug("Database migrations completed");
      }
      else
      {
        logger.LogInformation("Ensuring database exists...");
        await dbContext.Database.EnsureCreatedAsync();
        logger.LogDebug("Database creation completed");
      }
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to prepare database");
      throw;
    }
  }
}