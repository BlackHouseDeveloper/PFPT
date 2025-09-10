// <copyright file="AppointmentService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Mappers;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using PhysicallyFitPT.Shared;

/// <summary>
/// Service for managing appointment operations including scheduling, cancellation, and retrieval.
/// </summary>
public class AppointmentService : BaseService, IAppointmentService
{
  private readonly IDbContextFactory<ApplicationDbContext> factory;

  /// <summary>
  /// Initializes a new instance of the <see cref="AppointmentService"/> class.
  /// </summary>
  /// <param name="factory">Database context factory for data access.</param>
  /// <param name="logger">Logger instance for logging operations.</param>
  public AppointmentService(IDbContextFactory<ApplicationDbContext> factory, ILogger<AppointmentService> logger)
      : base(logger)
  {
    this.factory = factory;
  }

  /// <inheritdoc/>
  public async Task<AppointmentDto> ScheduleAsync(Guid patientId, DateTimeOffset start, DateTimeOffset? end, VisitType visitType, string? location = null, string? clinicianName = null, string? clinicianNpi = null, CancellationToken cancellationToken = default)
  {
    try
    {
      // Validate inputs
      if (patientId == Guid.Empty)
      {
        throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));
      }

      if (start < DateTimeOffset.UtcNow.AddMinutes(-5))
      {
        throw new ArgumentException("Cannot schedule appointments in the past", nameof(start));
      }

      if (end.HasValue && end <= start)
      {
        throw new ArgumentException("End time must be after start time", nameof(end));
      }

      using var db = await this.factory.CreateDbContextAsync();

      // Verify patient exists
      bool patientExists = await db.Patients.AnyAsync(p => p.Id == patientId);
      if (!patientExists)
      {
        throw new ArgumentException("Patient not found", nameof(patientId));
      }

      var appt = new Appointment
      {
        PatientId = patientId,
        VisitType = visitType,
        ScheduledStart = start,
        ScheduledEnd = end,
        Location = location,
        ClinicianName = clinicianName,
        ClinicianNpi = clinicianNpi,
      };
      db.Appointments.Add(appt);
      await db.SaveChangesAsync(cancellationToken);

      // Map to DTO
      return appt.ToDto();
    }
    catch (Exception ex)
    {
      this.this.Logger.LogError(ex, "Error executing ScheduleAsync: {ErrorMessage}", ex.Message);
      throw; // Re-throw for critical operations
    }
  }

  /// <inheritdoc/>
  public async Task<bool> CancelAsync(Guid appointmentId, CancellationToken cancellationToken = default)
  {
    try
    {
      if (appointmentId == Guid.Empty)
      {
        throw new ArgumentException("Appointment ID cannot be empty", nameof(appointmentId));
      }

      using var db = await this.factory.CreateDbContextAsync(cancellationToken);
      var appt = await db.Appointments.FindAsync(new object?[] { appointmentId }, cancellationToken);
      if (appt is null)
      {
        return false;
      }

      db.Appointments.Remove(appt);
      await db.SaveChangesAsync(cancellationToken);
      return true;
    }
    catch (Exception ex)
    {
      this.this.Logger.LogError(ex, "Error executing CancelAsync: {ErrorMessage}", ex.Message);
      return false;
    }
  }

  /// <inheritdoc/>
  public async Task<IReadOnlyList<AppointmentDto>> GetUpcomingByPatientAsync(Guid patientId, DateTimeOffset fromUtc, int take = 50, CancellationToken cancellationToken = default)
  {
    try
    {
      if (patientId == Guid.Empty)
      {
        throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));
      }

      if (take <= 0 || take > 1000)
      {
        throw new ArgumentException("Take parameter must be between 1 and 1000", nameof(take));
      }

      using var db = await this.factory.CreateDbContextAsync(cancellationToken);
      var upcoming = await db.Appointments.AsNoTracking()
          .Where(a => a.PatientId == patientId && a.ScheduledStart >= fromUtc)
          .OrderBy(a => a.ScheduledStart)
          .Take(take)
          .ToListAsync(cancellationToken);

      // Map each to DTO
      var dtoList = upcoming.Select(a => a.ToDto()).ToList();
      return dtoList;
    }
    catch (Exception ex)
    {
      this.Logger.LogError(ex, "Error executing GetUpcomingByPatientAsync: {ErrorMessage}", ex.Message);
      return Array.Empty<AppointmentDto>();
    }
  }
}
