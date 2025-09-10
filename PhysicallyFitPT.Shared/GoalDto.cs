// <copyright file="GoalDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  /// <summary>
  /// Data transfer object representing a treatment goal for a patient.
  /// </summary>
  public class GoalDto
  {
    /// <summary>
    /// Gets or sets the unique identifier of the goal.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is a long-term goal.
    /// </summary>
    public bool IsLongTerm { get; set; }

    /// <summary>
    /// Gets or sets the description of the goal.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Gets or sets the type of measurement used to track the goal.
    /// </summary>
    public string? MeasureType { get; set; }

    /// <summary>
    /// Gets or sets the baseline value or starting point for the goal.
    /// </summary>
    public string? BaselineValue { get; set; }

    /// <summary>
    /// Gets or sets the target value to be achieved for this goal.
    /// </summary>
    public string? TargetValue { get; set; }

    /// <summary>
    /// Gets or sets the target date for achieving this goal.
    /// </summary>
    public DateTime? TargetDate { get; set; }

    /// <summary>
    /// Gets or sets the current status of the goal.
    /// </summary>
    public string Status { get; set; } = null!;
  }
}
