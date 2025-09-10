// <copyright file="AutoMessagingService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;

/// <summary>
/// Service for managing automated messaging functionality including check-in notifications.
/// </summary>
public class AutoMessagingService : IAutoMessagingService
{
  private readonly IDbContextFactory<ApplicationDbContext> factory;

  /// <summary>
  /// Initializes a new instance of the <see cref="AutoMessagingService"/> class.
  /// </summary>
  /// <param name="factory">Database context factory for data access.</param>
  public AutoMessagingService(IDbContextFactory<ApplicationDbContext> factory) => this.factory = factory;

  /// <inheritdoc/>
  public async Task<CheckInMessageLog> EnqueueCheckInAsync(Guid patientId, Guid appointmentId, VisitType visitType, QuestionnaireType questionnaireType, DeliveryMethod method, DateTimeOffset scheduledSendAtUtc)
  {
    using var db = await this.factory.CreateDbContextAsync();
    var log = new CheckInMessageLog
    {
      PatientId = patientId,
      AppointmentId = appointmentId,
      VisitType = visitType,
      QuestionnaireType = questionnaireType,
      Method = method,
      ScheduledSendAt = scheduledSendAtUtc,
      Status = "Pending",
    };
    db.CheckInMessageLogs.Add(log);
    await db.SaveChangesAsync();
    return log;
  }

  /// <inheritdoc/>
  public async Task<IReadOnlyList<CheckInMessageLog>> GetLogAsync(Guid? patientId = null, int take = 100)
  {
    using var db = await this.factory.CreateDbContextAsync();
    var q = db.CheckInMessageLogs.AsNoTracking().OrderByDescending(x => x.CreatedAt);
    if (patientId.HasValue)
    {
      q = q.Where(x => x.PatientId == patientId.Value).OrderByDescending(x => x.CreatedAt);
    }

    return await q.Take(take).ToListAsync();
  }
}
