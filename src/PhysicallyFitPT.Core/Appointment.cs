// <copyright file="Appointment.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core;

/// <summary>
/// Represents an appointment for a patient in the physical therapy system.
/// </summary>
public class Appointment : Entity
{
  /// <summary>
  /// Gets or sets the patient identifier associated with this appointment.
  /// </summary>
  public Guid PatientId { get; set; }

  /// <summary>
  /// Gets or sets the patient entity associated with this appointment.
  /// </summary>
  public Patient Patient { get; set; } = null!;

  /// <summary>
  /// Gets or sets the type of visit for this appointment.
  /// </summary>
  public VisitType VisitType { get; set; }

  /// <summary>
  /// Gets or sets the scheduled start time of the appointment.
  /// </summary>
  public DateTimeOffset ScheduledStart { get; set; }

  /// <summary>
  /// Gets or sets the scheduled end time of the appointment.
  /// </summary>
  public DateTimeOffset? ScheduledEnd { get; set; }

  /// <summary>
  /// Gets or sets the location where the appointment takes place.
  /// </summary>
  public string? Location { get; set; }

  /// <summary>
  /// Gets or sets the clinician's National Provider Identifier (NPI).
  /// </summary>
  public string? ClinicianNpi { get; set; }

  /// <summary>
  /// Gets or sets the name of the clinician conducting the appointment.
  /// </summary>
  public string? ClinicianName { get; set; }

  /// <summary>
  /// Gets or sets the timestamp when the questionnaire was sent to the patient.
  /// </summary>
  public DateTimeOffset? QuestionnaireSentAt { get; set; }

  /// <summary>
  /// Gets or sets the timestamp when the patient completed the questionnaire.
  /// </summary>
  public DateTimeOffset? QuestionnaireCompletedAt { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether the patient has checked in for the appointment.
  /// </summary>
  public bool IsCheckedIn { get; set; }

  /// <summary>
  /// Gets or sets the clinical note associated with this appointment.
  /// </summary>
  public Note? Note { get; set; }
}
