// <copyright file="OutcomeMeasure.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core;

/// <summary>
/// Represents a standardized outcome measure used in physical therapy assessment.
/// </summary>
public class OutcomeMeasure
{
  /// <summary>
  /// Gets or sets the full name of the outcome measure.
  /// </summary>
  public required string Name { get; set; }

  /// <summary>
  /// Gets or sets the common abbreviation (e.g., LEFS, DASH).
  /// </summary>
  public required string Abbreviation { get; set; }

  /// <summary>
  /// Gets or sets the body parts this measure is applicable to.
  /// </summary>
  public required List<string> BodyPart { get; set; }

  /// <summary>
  /// Gets or sets the minimum possible score.
  /// </summary>
  public required int MinScore { get; set; }

  /// <summary>
  /// Gets or sets the maximum possible score.
  /// </summary>
  public required int MaxScore { get; set; }

  /// <summary>
  /// Gets or sets the interpretation guide for scores.
  /// </summary>
  public required string Interpretation { get; set; }

  /// <summary>
  /// Gets or sets the Minimal Clinically Important Difference (MCID) in points.
  /// </summary>
  public int? Mcid { get; set; }
}
