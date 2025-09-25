// <copyright file="SeederOptions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Seeder.Configuration;

/// <summary>
/// Configuration options for the seeder application.
/// </summary>
public class SeederOptions
{
  /// <summary>
  /// Configuration section name.
  /// </summary>
  public const string SectionName = "Seeder";

  /// <summary>
  /// Gets or sets the database connection string.
  /// </summary>
  public string? ConnectionString { get; set; }

  /// <summary>
  /// Gets or sets the default environment.
  /// </summary>
  public string Environment { get; set; } = "Development";

  /// <summary>
  /// Gets or sets the default log level.
  /// </summary>
  public string LogLevel { get; set; } = "Information";

  /// <summary>
  /// Gets or sets a value indicating whether to use migrations instead of EnsureCreated.
  /// </summary>
  public bool UseMigrations { get; set; } = true;

  /// <summary>
  /// Gets or sets the data directory path.
  /// </summary>
  public string? DataDirectory { get; set; }
}
