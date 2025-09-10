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

public interface IAppointmentService
{
  /// <summary>
  ///
  /// </summary>
  /// <param name="patientId"></param>
  /// <param name="start"></param>
  /// <param name="end"></param>
  /// <param name="visitType"></param>
  /// <param name="location"></param>
  /// <param name="clinicianName"></param>
  /// <param name="clinicianNpi"></param>
  /// <param name="cancellationToken"></param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<AppointmentDto> ScheduleAsync(Guid patientId, DateTimeOffset start, DateTimeOffset? end, VisitType visitType, string? location = null, string? clinicianName = null, string? clinicianNpi = null, CancellationToken cancellationToken = default);

  /// <summary>
  ///
  /// </summary>
  /// <param name="appointmentId"></param>
  /// <param name="cancellationToken"></param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<bool> CancelAsync(Guid appointmentId, CancellationToken cancellationToken = default);

  /// <summary>
  ///
  /// </summary>
  /// <param name="patientId"></param>
  /// <param name="fromUtc"></param>
  /// <param name="take"></param>
  /// <param name="cancellationToken"></param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<IReadOnlyList<AppointmentDto>> GetUpcomingByPatientAsync(Guid patientId, DateTimeOffset fromUtc, int take = 50, CancellationToken cancellationToken = default);
}
