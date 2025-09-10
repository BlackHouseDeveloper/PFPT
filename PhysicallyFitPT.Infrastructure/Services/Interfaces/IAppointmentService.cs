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
  Task<AppointmentDto> ScheduleAsync(Guid patientId, DateTimeOffset start, DateTimeOffset? end, VisitType visitType, string? location = null, string? clinicianName = null, string? clinicianNpi = null, CancellationToken cancellationToken = default);

  Task<bool> CancelAsync(Guid appointmentId, CancellationToken cancellationToken = default);

  Task<IReadOnlyList<AppointmentDto>> GetUpcomingByPatientAsync(Guid patientId, DateTimeOffset fromUtc, int take = 50, CancellationToken cancellationToken = default);
}
