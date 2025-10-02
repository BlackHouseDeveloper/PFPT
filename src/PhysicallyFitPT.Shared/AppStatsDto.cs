// <copyright file="AppStatsDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

using System;

/// <summary>
/// Represents application statistics for the debug/diagnostics bar.
/// </summary>
public class AppStatsDto
{
  /// <summary>
  /// Gets or sets the total number of patients in the system.
  /// </summary>
  public int Patients { get; set; }

  /// <summary>
  /// Gets or sets the date and time when the last patient was updated.
  /// </summary>
  public DateTimeOffset? LastPatientUpdated { get; set; }

  /// <summary>
  /// Gets or sets the total number of appointments in the system.
  /// </summary>
  public int Appointments { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether the API is healthy.
  /// </summary>
  public bool ApiHealthy { get; set; }
}
