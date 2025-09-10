// <copyright file="AppointmentMapperExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Mappers
{
  using PhysicallyFitPT.Domain;
  using PhysicallyFitPT.Shared;

  public static class AppointmentMapperExtensions
    {
        public static AppointmentDto ToDto(this Appointment appointment)
        {
            return new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                VisitType = appointment.VisitType.ToString(),
                ScheduledStart = appointment.ScheduledStart,
                ScheduledEnd = appointment.ScheduledEnd,
                Location = appointment.Location,
                ClinicianName = appointment.ClinicianName,
                ClinicianNpi = appointment.ClinicianNpi,
                QuestionnaireSentAt = appointment.QuestionnaireSentAt,
                QuestionnaireCompletedAt = appointment.QuestionnaireCompletedAt,
                IsCheckedIn = appointment.IsCheckedIn,
            };
        }
    }
}
