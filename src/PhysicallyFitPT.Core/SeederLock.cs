// <copyright file="SeederLock.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core;

/// <summary>
/// Represents a database lock to prevent concurrent seeding operations.
/// </summary>
public class SeederLock
{
  /// <summary>
  /// Gets or sets the lock identifier (always 1 for singleton lock).
  /// </summary>
  public int Id { get; set; } = 1;

  /// <summary>
  /// Gets or sets the timestamp when the lock was acquired.
  /// </summary>
  public DateTimeOffset AcquiredAtUtc { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// Gets or sets the process information that acquired the lock.
  /// </summary>
  public string? ProcessInfo { get; set; }
}
