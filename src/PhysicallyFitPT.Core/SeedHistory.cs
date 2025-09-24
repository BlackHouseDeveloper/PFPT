// <copyright file="SeedHistory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core;

/// <summary>
/// Represents the history of applied seed tasks for tracking seeding operations.
/// </summary>
public class SeedHistory
{
  /// <summary>
  /// Gets or sets the unique identifier of the seed task (e.g., "001.cpt.codes").
  /// </summary>
  public string TaskId { get; set; } = null!;

  /// <summary>
  /// Gets or sets the human-readable name of the seed task.
  /// </summary>
  public string Name { get; set; } = null!;

  /// <summary>
  /// Gets or sets the SHA256 hash of the task content for change detection.
  /// </summary>
  public string Hash { get; set; } = null!;

  /// <summary>
  /// Gets or sets the timestamp when the seed task was applied.
  /// </summary>
  public DateTimeOffset AppliedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
