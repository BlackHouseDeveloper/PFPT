using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;

namespace PhysicallyFitPT.Infrastructure.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    public AppointmentService(IDbContextFactory<ApplicationDbContext> factory) => _factory = factory;

    public async Task<Appointment> ScheduleAsync(Guid patientId, DateTimeOffset start, DateTimeOffset? end, VisitType visitType, string? location = null, string? clinicianName = null, string? clinicianNpi = null)
    {
        using var db = await _factory.CreateDbContextAsync();
        var appt = new Appointment
        {
            PatientId = patientId,
            VisitType = visitType,
            ScheduledStart = start,
            ScheduledEnd = end,
            Location = location,
            ClinicianName = clinicianName,
            ClinicianNpi = clinicianNpi
        };
        db.Appointments.Add(appt);
        await db.SaveChangesAsync();
        return appt;
    }

    public async Task<bool> CancelAsync(Guid appointmentId)
    {
        using var db = await _factory.CreateDbContextAsync();
        var appt = await db.Appointments.FindAsync(appointmentId);
        if (appt is null) return false;
        db.Appointments.Remove(appt);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<Appointment>> GetUpcomingByPatientAsync(Guid patientId, DateTimeOffset fromUtc, int take = 50)
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Appointments.AsNoTracking()
            .Where(a => a.PatientId == patientId && a.ScheduledStart >= fromUtc)
            .OrderBy(a => a.ScheduledStart)
            .Take(take)
            .ToListAsync();
    }
}
