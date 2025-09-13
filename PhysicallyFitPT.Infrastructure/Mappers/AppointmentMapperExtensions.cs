// <copyright file="AppointmentMapperExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Mappers
{
    using PhysicallyFitPT.Domain;
    using PhysicallyFitPT.Shared;

    /// <summary>
    /// Extension methods for mapping Appointment domain objects to DTOs.
    /// </summary>
    public static class AppointmentMapperExtensions
    {
        /// <summary>
        /// Converts an Appointment domain object to its corresponding DTO.
        /// </summary>
        /// <param name="appointment">The appointment domain object to convert.</param>
        /// <returns>An AppointmentDto containing the appointment data.</returns>
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
