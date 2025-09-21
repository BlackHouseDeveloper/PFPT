// <copyright file="QuestionnaireResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

/// <summary>
/// Represents a patient's response to a questionnaire.
/// </summary>
public class QuestionnaireResponse : Entity
{
  /// <summary>
  /// Gets or sets the patient identifier who submitted the response.
  /// </summary>
  public Guid PatientId { get; set; }

  /// <summary>
  /// Gets or sets the appointment identifier associated with the response.
  /// </summary>
  public Guid AppointmentId { get; set; }

  /// <summary>
  /// Gets or sets the questionnaire definition identifier that was answered.
  /// </summary>
  public Guid QuestionnaireDefinitionId { get; set; }

  /// <summary>
  /// Gets or sets the timestamp when the questionnaire was submitted.
  /// </summary>
  public DateTimeOffset SubmittedAt { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// Gets or sets the patient's answers serialized as JSON.
  /// </summary>
  public string AnswersJson { get; set; } = "{}";
}
