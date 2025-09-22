// <copyright file="DashboardStatsDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  /// <summary>
  /// Represents dashboard statistics data transfer object.
  /// </summary>
  public class DashboardStatsDto
  {
    /// <summary>
    /// Gets or sets the number of appointments scheduled for today.
    /// </summary>
    public int TodaysAppointments { get; set; }

    /// <summary>
    /// Gets or sets the total number of active patients.
    /// </summary>
    public int ActivePatients { get; set; }

    /// <summary>
    /// Gets or sets the number of pending notes that need to be completed.
    /// </summary>
    public int PendingNotes { get; set; }

    /// <summary>
    /// Gets or sets the number of patients with overdue outcome measures.
    /// </summary>
    public int OverdueOutcomeMeasures { get; set; }
  }
}