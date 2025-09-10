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

        public static void UpdateFromDto(this Appointment appointment, AppointmentDto dto)
        {
            // Parse VisitType from string
            if (Enum.TryParse<VisitType>(dto.VisitType, out var visitType))
            {
                appointment.VisitType = visitType;
            }
            
            appointment.ScheduledStart = dto.ScheduledStart;
            appointment.ScheduledEnd = dto.ScheduledEnd;
            appointment.Location = dto.Location;
            appointment.ClinicianName = dto.ClinicianName;
            appointment.ClinicianNpi = dto.ClinicianNpi;
            appointment.QuestionnaireSentAt = dto.QuestionnaireSentAt;
            appointment.QuestionnaireCompletedAt = dto.QuestionnaireCompletedAt;
            appointment.IsCheckedIn = dto.IsCheckedIn;
        }
    }
}
