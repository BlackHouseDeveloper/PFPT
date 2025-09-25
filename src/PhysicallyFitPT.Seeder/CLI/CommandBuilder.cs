// <copyright file="CommandBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Seeder.Seeding;
using PhysicallyFitPT.Seeder.Utils;

namespace PhysicallyFitPT.Seeder.CLI;

/// <summary>
/// Builds the command-line interface for the seeder application.
/// </summary>
public static class CommandBuilder
{
  /// <summary>
  /// Creates the root command with all subcommands.
  /// </summary>
  /// <param name="serviceProvider">Service provider for dependency injection.</param>
  /// <returns>The configured root command.</returns>
  public static RootCommand CreateRootCommand(IServiceProvider serviceProvider)
  {
    var rootCommand = new RootCommand("PFPT Seeder - Professional-grade database seeding tool");

    // Global options
    var connectionOption = new Option<string?>(
      "--connection",
      "Override connection string");

    var logLevelOption = new Option<string>(
      "--log-level",
      () => "Information",
      "Set log level (Debug, Information, Warning, Error)");

    rootCommand.AddGlobalOption(connectionOption);
    rootCommand.AddGlobalOption(logLevelOption);

    // Add subcommands
    rootCommand.AddCommand(CreateSeedCommand(serviceProvider));
    rootCommand.AddCommand(CreateMigrateCommand(serviceProvider));
    rootCommand.AddCommand(CreateVerifyCommand(serviceProvider));
    rootCommand.AddCommand(CreateDumpCommand(serviceProvider));

    return rootCommand;
  }

  private static Command CreateSeedCommand(IServiceProvider serviceProvider)
  {
    var seedCommand = new Command("seed", "Run seed tasks");

    var envOption = new Option<string>(
      "--env",
      () => "Development",
      "Environment (Development, Staging, Production)");

    var taskOption = new Option<string?>(
      "--task",
      "Run a specific task by ID or name");

    var listOption = new Option<bool>(
      "--list",
      "List tasks and their status without running them");

    var replayChangedOption = new Option<bool>(
      "--replay-changed",
      "Re-run tasks whose content has changed");

    var forceOption = new Option<bool>(
      "--force",
      "Override environment restrictions");

    var dryRunOption = new Option<bool>(
      "--dry-run",
      "Show what would happen without making changes");

    seedCommand.AddOption(envOption);
    seedCommand.AddOption(taskOption);
    seedCommand.AddOption(listOption);
    seedCommand.AddOption(replayChangedOption);
    seedCommand.AddOption(forceOption);
    seedCommand.AddOption(dryRunOption);

    seedCommand.SetHandler(async (env, task, list, replayChanged, force, dryRun) =>
    {
      var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
      var seedRunner = serviceProvider.GetRequiredService<SeedRunner>();

      try
      {
        if (list)
        {
          await ListTasksAsync(seedRunner, env, logger);
          return;
        }

        var options = new SeedRunOptions
        {
          Environment = env,
          TaskFilter = task,
          ReplayChanged = replayChanged,
          Force = force,
          DryRun = dryRun,
          ContinueOnError = false,
        };

        var success = await seedRunner.RunAsync(options);
        Environment.ExitCode = success ? 0 : 1;
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Seed command failed");
        Environment.ExitCode = 1;
      }
    }, envOption, taskOption, listOption, replayChangedOption, forceOption, dryRunOption);

    return seedCommand;
  }

  private static Command CreateMigrateCommand(IServiceProvider serviceProvider)
  {
    var migrateCommand = new Command("migrate", "Run EF Core migrations");

    migrateCommand.SetHandler(async () =>
    {
      var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
      var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

      try
      {
        logger.LogInformation("Running database migrations...");
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("Database migrations completed successfully");
        Environment.ExitCode = 0;
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Migration failed");
        Environment.ExitCode = 1;
      }
    });

    return migrateCommand;
  }

  private static Command CreateVerifyCommand(IServiceProvider serviceProvider)
  {
    var verifyCommand = new Command("verify", "Verify required baseline tasks are applied");

    verifyCommand.SetHandler(async () =>
    {
      var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
      var seedRunner = serviceProvider.GetRequiredService<SeedRunner>();

      try
      {
        var env = EnvDetector.GetCurrentEnvironment();
        var taskStatuses = await seedRunner.ListTasksAsync(env);

        var requiredTasks = new[] { "001.cpt.codes", "002.icd10.codes" };
        var missingTasks = new List<string>();

        foreach (var requiredTask in requiredTasks)
        {
          var status = taskStatuses.FirstOrDefault(t => t.Id == requiredTask);
          if (status == null || !status.Applied)
          {
            missingTasks.Add(requiredTask);
          }
        }

        if (missingTasks.Count > 0)
        {
          logger.LogError("Required baseline tasks are missing: {MissingTasks}", string.Join(", ", missingTasks));
          Environment.ExitCode = 1;
        }
        else
        {
          logger.LogInformation("All required baseline tasks are applied");
          Environment.ExitCode = 0;
        }
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Verify command failed");
        Environment.ExitCode = 1;
      }
    });

    return verifyCommand;
  }

  private static Command CreateDumpCommand(IServiceProvider serviceProvider)
  {
    var dumpCommand = new Command("dump", "Export reference data as JSON");

    var outOption = new Option<string>(
      "--out",
      "Output directory path")
    {
      IsRequired = true,
    };

    dumpCommand.AddOption(outOption);

    dumpCommand.SetHandler(async (outPath) =>
    {
      var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
      var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

      try
      {
        await DumpDataAsync(dbContext, outPath, logger);
        Environment.ExitCode = 0;
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Dump command failed");
        Environment.ExitCode = 1;
      }
    }, outOption);

    return dumpCommand;
  }

  private static async Task ListTasksAsync(SeedRunner seedRunner, string environment, ILogger logger)
  {
    var tasks = await seedRunner.ListTasksAsync(environment);

    logger.LogInformation("Seed Task Status for Environment: {Environment}", environment);
    logger.LogInformation("{Header}", "ID".PadRight(20) + "Name".PadRight(35) + "Applied".PadRight(8) + "Status");
    logger.LogInformation("{Separator}", new string('-', 80));

    foreach (var task in tasks)
    {
      var appliedText = task.Applied ? "Yes" : "No";
      var status = task.PendingReason ?? (task.Applied ? "Applied" : "Pending");

      logger.LogInformation("{TaskInfo}",
        task.Id.PadRight(20) + task.Name.PadRight(35) + appliedText.PadRight(8) + status);
    }
  }

  private static async Task DumpDataAsync(ApplicationDbContext dbContext, string outPath, ILogger logger)
  {
    Directory.CreateDirectory(outPath);

    logger.LogInformation("Exporting reference data to {Path}", outPath);

    // Export CPT codes
    var cptCodes = await dbContext.CptCodes.ToListAsync();
    var cptJson = System.Text.Json.JsonSerializer.Serialize(
      cptCodes.Select(c => new { code = c.Code, description = c.Description }),
      new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    await File.WriteAllTextAsync(Path.Combine(outPath, "cpt.json"), cptJson);

    // Export ICD-10 codes
    var icd10Codes = await dbContext.Icd10Codes.ToListAsync();
    var icd10Json = System.Text.Json.JsonSerializer.Serialize(
      icd10Codes.Select(c => new { code = c.Code, description = c.Description }),
      new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    await File.WriteAllTextAsync(Path.Combine(outPath, "icd10.json"), icd10Json);

    // Export patients only in Development
    var env = EnvDetector.GetCurrentEnvironment();
    if (env.Equals("Development", StringComparison.OrdinalIgnoreCase))
    {
      var patients = await dbContext.Patients.ToListAsync();
      var patientsJson = System.Text.Json.JsonSerializer.Serialize(
        patients.Select(p => new { mrn = p.MRN, firstName = p.FirstName, lastName = p.LastName, email = p.Email }),
        new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
      await File.WriteAllTextAsync(Path.Combine(outPath, "patients.dev.json"), patientsJson);
    }

    logger.LogInformation("Data export completed");
  }
}