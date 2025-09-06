using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;

namespace PhysicallyFitPT.Infrastructure.Services;

public class AutoMessagingService : IAutoMessagingService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    public AutoMessagingService(IDbContextFactory<ApplicationDbContext> factory) => _factory = factory;

    public async Task<CheckInMessageLog> EnqueueCheckInAsync(Guid patientId, Guid appointmentId, VisitType visitType, QuestionnaireType questionnaireType, DeliveryMethod method, DateTimeOffset scheduledSendAtUtc)
    {
        using var db = await _factory.CreateDbContextAsync();
        var log = new CheckInMessageLog
        {
            PatientId = patientId,
            AppointmentId = appointmentId,
            VisitType = visitType,
            QuestionnaireType = questionnaireType,
            Method = method,
            ScheduledSendAt = scheduledSendAtUtc,
            Status = "Pending"
        };
        db.CheckInMessageLogs.Add(log);
        await db.SaveChangesAsync();
        return log;
    }

    public async Task<IReadOnlyList<CheckInMessageLog>> GetLogAsync(Guid? patientId = null, int take = 100)
    {
        using var db = await _factory.CreateDbContextAsync();
        var q = db.CheckInMessageLogs.AsNoTracking().OrderByDescending(x => x.CreatedAt);
        if (patientId.HasValue) q = q.Where(x => x.PatientId == patientId.Value).OrderByDescending(x => x.CreatedAt);
        return await q.Take(take).ToListAsync();
    }
}
