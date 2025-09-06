using PhysicallyFitPT.Domain;
namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

public interface IAppointmentService
{
    Task<Appointment> ScheduleAsync(Guid patientId, DateTimeOffset start, DateTimeOffset? end, VisitType visitType, string? location = null, string? clinicianName = null, string? clinicianNpi = null);
    Task<bool> CancelAsync(Guid appointmentId);
    Task<IReadOnlyList<Appointment>> GetUpcomingByPatientAsync(Guid patientId, DateTimeOffset fromUtc, int take = 50);
}
