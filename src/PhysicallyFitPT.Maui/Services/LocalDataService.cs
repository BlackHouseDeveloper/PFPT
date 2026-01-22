// <copyright file="LocalDataService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Services;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhysicallyFitPT.Core;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Mappers;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using PhysicallyFitPT.Shared;

/// <summary>
/// Local data service implementation for the MAUI host. This implementation uses the
/// SQLite-backed infrastructure services so the shared Blazor components can run offline.
/// </summary>
public sealed class LocalDataService : IDataService
{
  private readonly IPatientService patientService;
  private readonly IAppointmentService appointmentService;
  private readonly IDbContextFactory<ApplicationDbContext> dbContextFactory;
  private readonly ILogger<LocalDataService> logger;
  private readonly IDashboardMetricsService dashboardMetricsService;
  private readonly ISyncService syncService;
  private readonly IAppStatsService appStatsService;

  /// <summary>
  /// Initializes a new instance of the <see cref="LocalDataService"/> class.
  /// </summary>
  /// <param name="patientService">Domain service used for patient lookups via SQLite.</param>
  /// <param name="appointmentService">Domain service that manages appointment persistence.</param>
  /// <param name="dbContextFactory">Factory that produces EF Core contexts targeting the local database.</param>
  /// <param name="dashboardMetricsService">Service that computes dashboard metrics from the local store.</param>
  /// <param name="syncService">Synchronization coordinator that surfaces remote aggregate data.</param>
  /// <param name="appStatsService">Shared statistics service used when remote sync data is unavailable.</param>
  /// <param name="logger">Logger used to capture unexpected errors from local queries.</param>
  public LocalDataService(
    IPatientService patientService,
    IAppointmentService appointmentService,
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IDashboardMetricsService dashboardMetricsService,
    ISyncService syncService,
    IAppStatsService appStatsService,
    ILogger<LocalDataService> logger)
  {
    this.patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
    this.appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
    this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    this.dashboardMetricsService = dashboardMetricsService ?? throw new ArgumentNullException(nameof(dashboardMetricsService));
    this.syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
    this.appStatsService = appStatsService ?? throw new ArgumentNullException(nameof(appStatsService));
    this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  /// <inheritdoc />
  public Task<IEnumerable<PatientDto>> SearchPatientsAsync(string query, int take = 50, CancellationToken cancellationToken = default)
  {
    return this.patientService.SearchAsync(query, take, cancellationToken);
  }

  /// <inheritdoc />
  public async Task<PatientDto?> GetPatientByIdAsync(Guid patientId, CancellationToken cancellationToken = default)
  {
    await using var db = await this.dbContextFactory.CreateDbContextAsync(cancellationToken);
    var patient = await db.Patients.AsNoTracking()
      .FirstOrDefaultAsync(p => p.Id == patientId, cancellationToken);

    return patient?.ToDto();
  }

  /// <inheritdoc />
  public Task<AppointmentDto> ScheduleAppointmentAsync(
    Guid patientId,
    DateTimeOffset start,
    DateTimeOffset? end,
    VisitType visitType,
    string? location = null,
    string? clinicianName = null,
    string? clinicianNpi = null,
    CancellationToken cancellationToken = default)
  {
    return this.appointmentService.ScheduleAsync(patientId, start, end, visitType, location, clinicianName, clinicianNpi, cancellationToken);
  }

  /// <inheritdoc />
  public Task<IReadOnlyList<AppointmentDto>> GetUpcomingAppointmentsByPatientAsync(
    Guid patientId,
    DateTimeOffset fromUtc,
    int take = 50,
    CancellationToken cancellationToken = default)
  {
    return this.appointmentService.GetUpcomingByPatientAsync(patientId, fromUtc, take, cancellationToken);
  }

  /// <inheritdoc />
  public Task<bool> CancelAppointmentAsync(Guid appointmentId, CancellationToken cancellationToken = default)
  {
    return this.appointmentService.CancelAsync(appointmentId, cancellationToken);
  }

  /// <inheritdoc />
  public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
  {
    var snapshot = this.syncService.LatestSnapshot;
    if (snapshot?.DashboardStats is DashboardStatsDto remoteDashboard)
    {
      return new DashboardStatsDto
      {
        TodaysAppointments = remoteDashboard.TodaysAppointments,
        ActivePatients = remoteDashboard.ActivePatients,
        PendingNotes = remoteDashboard.PendingNotes,
        OverdueOutcomeMeasures = remoteDashboard.OverdueOutcomeMeasures,
      };
    }

    return await this.dashboardMetricsService.GetDashboardStatsAsync(cancellationToken);
  }

  /// <inheritdoc />
  public async Task<AppStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
  {
    var snapshot = this.syncService.LatestSnapshot;
    if (snapshot?.AppStats is AppStatsDto remoteStats)
    {
      var status = this.syncService.Status;
      return remoteStats with
      {
        ApiHealthy = status != SyncStatus.Failed && remoteStats.ApiHealthy,
      };
    }

    try
    {
      return await this.appStatsService.GetAppStatsAsync(cancellationToken);
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Error retrieving application statistics");
      return new AppStatsDto { ApiHealthy = false };
    }
  }

  /// <inheritdoc />
  public async Task<IReadOnlyList<NoteDto>> GetPatientNotesAsync(Guid patientId, CancellationToken cancellationToken = default)
  {
    try
    {
      await using var db = await this.dbContextFactory.CreateDbContextAsync(cancellationToken);
      var notes = await db.Notes
        .AsNoTracking()
        .Where(n => n.PatientId == patientId && !n.IsDeleted)
        .OrderByDescending(n => n.CreatedAt)
        .Select(n => new NoteDto
        {
          Id = n.Id,
          PatientId = n.PatientId,
          AppointmentId = n.AppointmentId,
          VisitType = n.VisitType.ToString(),
          IsSigned = n.IsSigned,
          SignedAt = n.SignedAt,
          SignedBy = n.SignedBy,
          CreatedAt = n.CreatedAt,
          CreatedBy = n.CreatedBy,
          UpdatedAt = n.UpdatedAt,
          UpdatedBy = n.UpdatedBy,
        })
        .ToListAsync(cancellationToken);

      return notes;
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Error retrieving notes for patient: {PatientId}", patientId);
      return Array.Empty<NoteDto>();
    }
  }

  /// <inheritdoc />
  public Task<IReadOnlyList<GoalDto>> GetPatientGoalsAsync(Guid patientId, CancellationToken cancellationToken = default)
  {
    // TODO: Goals table not yet present in schema. Return empty until schema migration is complete.
    this.logger.LogWarning("Goals retrieval not yet implemented - Goals table missing from schema");
    return Task.FromResult<IReadOnlyList<GoalDto>>(Array.Empty<GoalDto>());
  }

  /// <inheritdoc />
  public Task<IReadOnlyList<OutcomeMeasureScoreDto>> GetPatientOutcomesAsync(Guid patientId, CancellationToken cancellationToken = default)
  {
    // TODO: OutcomeMeasureScores table not yet present in schema. Return empty until schema migration is complete.
    this.logger.LogWarning("Outcome measures retrieval not yet implemented - OutcomeMeasureScores table missing from schema");
    return Task.FromResult<IReadOnlyList<OutcomeMeasureScoreDto>>(Array.Empty<OutcomeMeasureScoreDto>());
  }
}
