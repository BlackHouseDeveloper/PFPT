// <copyright file="QuestionnaireType.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

/// <summary>
/// Specifies the type of questionnaire to be administered to patients.
/// </summary>
public enum QuestionnaireType
{
  /// <summary>
  /// Initial evaluation questionnaire.
  /// </summary>
  Eval = 0,

  /// <summary>
  /// Daily treatment questionnaire.
  /// </summary>
  Daily = 1,

  /// <summary>
  /// Progress assessment questionnaire.
  /// </summary>
  Progress = 2,

  /// <summary>
  /// Discharge questionnaire.
  /// </summary>
  Discharge = 3,

  /// <summary>
  /// Body part specific questionnaire.
  /// </summary>
  BodyPartSpecific = 4,
}
