// <copyright file="DashboardMetricsService.cs" company="PlaceholderCompany">
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
using PhysicallyFitPT.Core;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using PhysicallyFitPT.Shared;

/// <summary>
/// Aggregates statistics used by the dashboard experience.
/// </summary>
public sealed class DashboardMetricsService : IDashboardMetricsService
{
  private readonly IDbContextFactory<ApplicationDbContext> dbContextFactory;
  private readonly ILogger<DashboardMetricsService> logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="DashboardMetricsService"/> class.
  /// </summary>
  /// <param name="dbContextFactory">Factory for providing database contexts against the local store.</param>
  /// <param name="logger">Logger for capturing diagnostic information.</param>
  public DashboardMetricsService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    ILogger<DashboardMetricsService> logger)
  {
    this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  /// <inheritdoc />
  public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      await using var db = await this.dbContextFactory.CreateDbContextAsync(cancellationToken);

      await EnsureSeedDataAsync(db, cancellationToken);

      var utcNow = DateTimeOffset.UtcNow;
      var startOfDay = new DateTimeOffset(utcNow.Date, utcNow.Offset);
      var endOfDay = startOfDay.AddDays(1);

      var todaysAppointments = await db.Appointments.AsNoTracking()
        .CountAsync(a => a.ScheduledStart >= startOfDay && a.ScheduledStart < endOfDay, cancellationToken);

      var activePatients = await db.Patients.AsNoTracking()
        .CountAsync(cancellationToken);

      var pendingNotes = await db.Notes.AsNoTracking()
        .CountAsync(n => !n.IsSigned, cancellationToken);

      var overdueOutcomeMeasures = await db.Appointments.AsNoTracking()
        .CountAsync(a => a.QuestionnaireSentAt != null && a.QuestionnaireCompletedAt == null && a.ScheduledStart < utcNow, cancellationToken);

      return new DashboardStatsDto
      {
        TodaysAppointments = todaysAppointments,
        ActivePatients = activePatients,
        PendingNotes = pendingNotes,
        OverdueOutcomeMeasures = overdueOutcomeMeasures,
      };
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Failed to compute dashboard stats from persistence layer");
      return new DashboardStatsDto();
    }
  }

  private static async Task EnsureSeedDataAsync(ApplicationDbContext db, CancellationToken cancellationToken)
  {
    var hadChanges = false;

    if (!await db.CptCodes.AnyAsync(cancellationToken))
    {
      await db.CptCodes.AddRangeAsync(
        new[]
        {
          new CptCode { Code = "97110", Description = "Therapeutic exercise" },
          new CptCode { Code = "97140", Description = "Manual therapy" },
          new CptCode { Code = "97530", Description = "Therapeutic activities" },
        },
        cancellationToken);
      hadChanges = true;
    }

    if (!await db.Icd10Codes.AnyAsync(cancellationToken))
    {
      await db.Icd10Codes.AddRangeAsync(
        new[]
        {
          new Icd10Code { Code = "M25.561", Description = "Pain in right knee" },
          new Icd10Code { Code = "M25.562", Description = "Pain in left knee" },
        },
        cancellationToken);
      hadChanges = true;
    }

    const int seedPatientSampleSize = 8;

    var patientIds = await db.Patients.AsNoTracking()
      .Select(p => p.Id)
      .Take(seedPatientSampleSize)
      .ToListAsync(cancellationToken);

    if (patientIds.Count == 0)
    {
      var seededPatients = new List<Patient>
      {
        new() { MRN = "A1001", FirstName = "Jane", LastName = "Doe", Email = "jane@example.com" },
        new() { MRN = "A1002", FirstName = "John", LastName = "Smith", Email = "john@example.com" },
      };

      await db.Patients.AddRangeAsync(seededPatients, cancellationToken);
      patientIds = seededPatients.Select(p => p.Id).ToList();
      hadChanges = true;
    }

    if (!await db.Appointments.AnyAsync(cancellationToken) && patientIds.Count > 0)
    {
      var utcNow = DateTimeOffset.UtcNow;
      var startOfDay = new DateTimeOffset(utcNow.Date, utcNow.Offset).AddHours(8);

      var sampleAppointments = Enumerable.Range(0, 8)
        .Select(
          index => new Appointment
          {
            PatientId = patientIds[index % patientIds.Count],
            VisitType = index == 0 ? VisitType.Eval : VisitType.Daily,
            ScheduledStart = startOfDay.AddHours(index),
            ScheduledEnd = startOfDay.AddHours(index).AddMinutes(45),
            Location = index % 2 == 0 ? "Main Clinic" : "Telehealth",
            ClinicianName = index % 2 == 0 ? "A. Therapist" : "B. Therapist",
            QuestionnaireSentAt = index < 5 ? utcNow.AddDays(-2) : null,
            QuestionnaireCompletedAt = index < 2 ? utcNow.AddDays(-1) : null,
          })
        .ToList();

      await db.Appointments.AddRangeAsync(sampleAppointments, cancellationToken);
      hadChanges = true;

      if (!await db.Notes.AnyAsync(cancellationToken))
      {
        var pendingNotes = sampleAppointments
          .Take(3)
          .Select(
            appt => new Note
            {
              AppointmentId = appt.Id,
              PatientId = appt.PatientId,
              VisitType = appt.VisitType,
              IsSigned = false,
            });

        await db.Notes.AddRangeAsync(pendingNotes, cancellationToken);
        hadChanges = true;
      }
    }

    if (hadChanges)
    {
      await db.SaveChangesAsync(cancellationToken);
    }
  }
}
