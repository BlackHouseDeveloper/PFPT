// <copyright file="QuestionnaireDefinition.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

/// <summary>
/// Represents a questionnaire definition template used to create patient questionnaires.
/// </summary>
public class QuestionnaireDefinition : Entity
{
  /// <summary>
  /// Gets or sets the name of the questionnaire.
  /// </summary>
  public string Name { get; set; } = null!;

  /// <summary>
  /// Gets or sets the type of questionnaire.
  /// </summary>
  public QuestionnaireType Type { get; set; }

  /// <summary>
  /// Gets or sets the body region this questionnaire is specific to (optional).
  /// </summary>
  public string? BodyRegion { get; set; }

  /// <summary>
  /// Gets or sets the version number of the questionnaire schema.
  /// </summary>
  public int Version { get; set; } = 1;

  /// <summary>
  /// Gets or sets the JSON schema defining the questionnaire structure and validation.
  /// </summary>
  public string JsonSchema { get; set; } = "{}";
}
