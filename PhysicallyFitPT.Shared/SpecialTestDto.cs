// <copyright file="SpecialTestDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

using System;

/// <summary>
/// Data transfer object for special test results.
/// </summary>
public class SpecialTestDto
{
  /// <summary>
  /// Gets or sets the unique identifier of the special test.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the name of the special test.
  /// </summary>
  public string Name { get; set; } = null!;

  /// <summary>
  /// Gets or sets the side of the body where the test was performed.
  /// </summary>
  public int Side { get; set; }

  /// <summary>
  /// Gets or sets the result of the special test.
  /// </summary>
  public int Result { get; set; }

  /// <summary>
  /// Gets or sets additional notes about the test.
  /// </summary>
  public string? Notes { get; set; }
}
