// <copyright file="AppStatsDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

using System;

/// <summary>
/// Represents application statistics for the debug/diagnostics bar.
/// </summary>
public record class AppStatsDto
{
  /// <summary>
  /// Gets the total number of patients in the system.
  /// </summary>
  public int Patients { get; init; }

  /// <summary>
  /// Gets the date and time when the last patient was updated.
  /// </summary>
  public DateTimeOffset? LastPatientUpdated { get; init; }

  /// <summary>
  /// Gets the total number of appointments in the system.
  /// </summary>
  public int Appointments { get; init; }

  /// <summary>
  /// Gets a value indicating whether the API is healthy.
  /// </summary>
  public bool ApiHealthy { get; init; } = true;
}
