// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Seeder
{

  using System.CommandLine;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Options;
  using PhysicallyFitPT.Seeder.CLI;
  using PhysicallyFitPT.Seeder.Configuration;

  /// <summary>
  /// Main program class for the PFPT Seeder application.
  /// </summary>
  public class Program
  {
    /// <summary>
    /// Application entry point.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>Exit code.</returns>
    public static async Task<int> Main(string[] args)
    {
      // Special case: if no arguments provided, show help
      if (args.Length == 0)
      {
        args = new[] { "--help" };
      }

      try
      {
        // Build and start the host
        using var host = SeederHost.CreateHostBuilder(args).Build();
        await host.StartAsync();

        // Get services
        var serviceProvider = host.Services;

        // Ensure database is ready
        var seederOptions = serviceProvider.GetRequiredService<IOptions<SeederOptions>>().Value;
        await SeederHost.EnsureDatabaseReadyAsync(serviceProvider, seederOptions.UseMigrations);

        // Create command with service provider
        var rootCommand = CommandBuilder.CreateRootCommand(serviceProvider);

        // Invoke the command
        return await rootCommand.InvokeAsync(args);
      }
      catch (Exception ex)
      {
        // Fallback error handling if DI isn't available
        Console.Error.WriteLine($"Fatal error: {ex.Message}");
        if (Environment.GetEnvironmentVariable("PFP_LOGLEVEL")?.Equals("Debug", StringComparison.OrdinalIgnoreCase) == true)
        {
          Console.Error.WriteLine(ex.ToString());
        }
        return 1;
      }
    }
  }
}
