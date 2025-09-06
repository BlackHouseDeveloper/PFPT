using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;

namespace PhysicallyFitPT.Infrastructure.Services;

public class AppointmentService : BaseService, IAppointmentService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    
    public AppointmentService(IDbContextFactory<ApplicationDbContext> factory, ILogger<AppointmentService> logger) 
        : base(logger)
    {
        _factory = factory;
    }

    public async Task<Appointment> ScheduleAsync(Guid patientId, DateTimeOffset start, DateTimeOffset? end, VisitType visitType, string? location = null, string? clinicianName = null, string? clinicianNpi = null)
    {
        try
        {
            // Validate inputs
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));
                
            if (start < DateTimeOffset.UtcNow.AddMinutes(-5)) // Allow 5 minute buffer for past appointments
                throw new ArgumentException("Cannot schedule appointments in the past", nameof(start));
                
            if (end.HasValue && end <= start)
                throw new ArgumentException("End time must be after start time", nameof(end));

            using var db = await _factory.CreateDbContextAsync();
            
            // Verify patient exists
            var patientExists = await db.Patients.AnyAsync(p => p.Id == patientId);
            if (!patientExists)
                throw new ArgumentException("Patient not found", nameof(patientId));
                
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
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing ScheduleAsync: {ErrorMessage}", ex.Message);
            throw; // Re-throw for critical operations
        }
    }

    public async Task<bool> CancelAsync(Guid appointmentId)
    {
        try
        {
            if (appointmentId == Guid.Empty)
                throw new ArgumentException("Appointment ID cannot be empty", nameof(appointmentId));
                
            using var db = await _factory.CreateDbContextAsync();
            var appt = await db.Appointments.FindAsync(appointmentId);
            if (appt is null) return false;
            db.Appointments.Remove(appt);
            await db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing CancelAsync: {ErrorMessage}", ex.Message);
            return false;
        }
    }

    public async Task<IReadOnlyList<Appointment>> GetUpcomingByPatientAsync(Guid patientId, DateTimeOffset fromUtc, int take = 50)
    {
        try
        {
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));
                
            if (take <= 0 || take > 1000)
                throw new ArgumentException("Take parameter must be between 1 and 1000", nameof(take));
                
            using var db = await _factory.CreateDbContextAsync();
            return await db.Appointments.AsNoTracking()
                .Where(a => a.PatientId == patientId && a.ScheduledStart >= fromUtc)
                .OrderBy(a => a.ScheduledStart)
                .Take(take)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing GetUpcomingByPatientAsync: {ErrorMessage}", ex.Message);
            return new List<Appointment>().AsReadOnly();
        }
    }
}
