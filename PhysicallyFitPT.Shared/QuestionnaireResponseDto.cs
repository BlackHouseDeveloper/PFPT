// <copyright file="QuestionnaireResponseDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

using System;

/// <summary>
/// Data transfer object for questionnaire responses.
/// </summary>
public class QuestionnaireResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the questionnaire response.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the patient.
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the appointment.
    /// </summary>
    public Guid AppointmentId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the questionnaire definition.
    /// </summary>
    public Guid QuestionnaireDefinitionId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the response was submitted.
    /// </summary>
    public DateTimeOffset SubmittedAt { get; set; }

    /// <summary>
    /// Gets or sets the questionnaire answers in JSON format.
    /// </summary>
    public string AnswersJson { get; set; } = "{}";
}
