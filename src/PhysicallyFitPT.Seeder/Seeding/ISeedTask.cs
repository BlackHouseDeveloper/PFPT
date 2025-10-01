// <copyright file="ISeedTask.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Seeder.Seeding;

/// <summary>
/// Represents a versioned seed task that can be executed idempotently.
/// </summary>
public interface ISeedTask
{
  /// <summary>
  /// Gets the unique identifier for this seed task (e.g., "001.cpt.codes").
  /// </summary>
  string Id { get; }

  /// <summary>
  /// Gets the human-readable name of this seed task.
  /// </summary>
  string Name { get; }

  /// <summary>
  /// Gets the list of environments where this task is allowed to run.
  /// Empty list means all environments are allowed.
  /// </summary>
  IReadOnlyList<string> AllowedEnvironments { get; }

  /// <summary>
  /// Executes the seed task asynchronously.
  /// This method should be idempotent and use natural keys for upserts.
  /// </summary>
  /// <param name="cancellationToken">Cancellation token.</param>
  /// <returns>A task representing the asynchronous operation.</returns>
  Task ExecuteAsync(CancellationToken cancellationToken = default);

  /// <summary>
  /// Computes a content descriptor (hash) for change detection.
  /// This should include the task ID, data source hash, and any version tokens.
  /// </summary>
  /// <returns>A SHA256 hash string representing the current task content.</returns>
  Task<string> ComputeContentDescriptorAsync();
}
