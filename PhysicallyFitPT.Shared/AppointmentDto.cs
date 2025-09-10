// <copyright file="AppointmentDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  /// <summary>
  /// Represents an appointment data transfer object.
  /// </summary>
  public class AppointmentDto
  {
    /// <summary>
    /// Gets or sets the unique identifier for the appointment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the patient identifier associated with this appointment.
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// Gets or sets the type of visit for this appointment.
    /// </summary>
    public string VisitType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the scheduled start time for the appointment.
    /// </summary>
    public DateTimeOffset ScheduledStart { get; set; }

    /// <summary>
    /// Gets or sets the scheduled end time for the appointment.
    /// </summary>
    public DateTimeOffset? ScheduledEnd { get; set; }

    /// <summary>
    /// Gets or sets the location where the appointment will take place.
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets the National Provider Identifier (NPI) of the clinician.
    /// </summary>
    public string? ClinicianNpi { get; set; }

    /// <summary>
    /// Gets or sets the name of the clinician conducting the appointment.
    /// </summary>
    public string? ClinicianName { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the questionnaire was sent.
    /// </summary>
    public DateTimeOffset? QuestionnaireSentAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the questionnaire was completed.
    /// </summary>
    public DateTimeOffset? QuestionnaireCompletedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the patient has checked in for the appointment.
    /// </summary>
    public bool IsCheckedIn { get; set; }
  }
}
