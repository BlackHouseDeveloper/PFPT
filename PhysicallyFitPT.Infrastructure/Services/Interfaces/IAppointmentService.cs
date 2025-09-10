// <copyright file="IAppointmentService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Shared;

/// <summary>
/// Interface for appointment management services.
/// </summary>
public interface IAppointmentService
{
  /// <summary>
  /// Schedules a new appointment for a patient.
  /// </summary>
  /// <param name="patientId">The unique identifier of the patient.</param>
  /// <param name="start">The scheduled start time of the appointment.</param>
  /// <param name="end">The scheduled end time of the appointment.</param>
  /// <param name="visitType">The type of visit for the appointment.</param>
  /// <param name="location">The location where the appointment will take place.</param>
  /// <param name="clinicianName">The name of the clinician conducting the appointment.</param>
  /// <param name="clinicianNpi">The NPI number of the clinician.</param>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<AppointmentDto> ScheduleAsync(Guid patientId, DateTimeOffset start, DateTimeOffset? end, VisitType visitType, string? location = null, string? clinicianName = null, string? clinicianNpi = null, CancellationToken cancellationToken = default);

  /// <summary>
  /// Cancels an existing appointment.
  /// </summary>
  /// <param name="appointmentId">The unique identifier of the appointment to cancel.</param>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<bool> CancelAsync(Guid appointmentId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Retrieves upcoming appointments for a specific patient.
  /// </summary>
  /// <param name="patientId">The unique identifier of the patient.</param>
  /// <param name="fromUtc">The starting date/time to search from in UTC.</param>
  /// <param name="take">The maximum number of appointments to return.</param>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<IReadOnlyList<AppointmentDto>> GetUpcomingByPatientAsync(Guid patientId, DateTimeOffset fromUtc, int take = 50, CancellationToken cancellationToken = default);
}
