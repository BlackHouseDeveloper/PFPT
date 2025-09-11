// <copyright file="QuestionnaireDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

using System;

/// <summary>
/// Data transfer object for questionnaire definitions.
/// </summary>
public class QuestionnaireDto
{
  /// <summary>
  /// Gets or sets the unique identifier of the questionnaire.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the name of the questionnaire.
  /// </summary>
  public string Name { get; set; } = null!;

  /// <summary>
  /// Gets or sets the type of the questionnaire.
  /// </summary>
  public int Type { get; set; }

  /// <summary>
  /// Gets or sets the body region associated with the questionnaire.
  /// </summary>
  public string? BodyRegion { get; set; }

  /// <summary>
  /// Gets or sets the version number of the questionnaire.
  /// </summary>
  public int Version { get; set; }

  /// <summary>
  /// Gets or sets the JSON schema for the questionnaire.
  /// </summary>
  public string JsonSchema { get; set; } = "{}";
}
