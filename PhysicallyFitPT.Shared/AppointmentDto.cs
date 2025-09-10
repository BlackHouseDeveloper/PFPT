// <copyright file="AppointmentDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  public class AppointmentDto
  {
    public Guid Id { get; set; }

    public Guid PatientId { get; set; }

    public string VisitType { get; set; } = null!;

    public DateTimeOffset ScheduledStart { get; set; }

    public DateTimeOffset? ScheduledEnd { get; set; }

    public string? Location { get; set; }

    public string? ClinicianNpi { get; set; }

    public string? ClinicianName { get; set; }

    public DateTimeOffset? QuestionnaireSentAt { get; set; }

    public DateTimeOffset? QuestionnaireCompletedAt { get; set; }

    public bool IsCheckedIn { get; set; }
  }
}
