// <copyright file="Goal.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core;

/// <summary>
/// Represents the status of a goal.
/// </summary>
public enum GoalStatus
{
  /// <summary>
  /// The goal has not been started.
  /// </summary>
  NotStarted = 0,

  /// <summary>
  /// The goal is currently in progress.
  /// </summary>
  InProgress = 1,

  /// <summary>
  /// The goal has been met.
  /// </summary>
  Met = 2,

  /// <summary>
  /// The goal has been partially met.
  /// </summary>
  PartiallyMet = 3,

  /// <summary>
  /// The goal has not been met.
  /// </summary>
  NotMet = 4,
}

/// <summary>
/// Represents a goal for a user, including its description, measurement, target, and status.
/// </summary>
public sealed class Goal
{
  /// <summary>
  /// Gets or sets the unique identifier for the goal.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether the goal is long-term.
  /// </summary>
  public bool IsLongTerm { get; set; }

  // 500-char max enforced in DB; Domain is annotation-free by design.

  /// <summary>
  /// Gets or sets the description of the goal.
  /// </summary>
  public string Description { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the type of measurement associated with the goal (optional).
  /// </summary>
  public string? MeasureType { get; set; }

  /// <summary>
  /// Gets or sets the baseline value associated with the goal (optional).
  /// </summary>
  public string? BaselineValue { get; set; }

  /// <summary>
  /// Gets or sets the target value for the goal (optional).
  /// </summary>
  public string? TargetValue { get; set; }

  /// <summary>
  /// Gets or sets the target completion date for the goal (optional).
  /// </summary>
  public DateTime? TargetDate { get; set; }

  /// <summary>
  /// Gets or sets the current status of the goal.
  /// </summary>
  public GoalStatus Status { get; set; } = GoalStatus.NotStarted;
}
