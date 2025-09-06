using PhysicallyFitPT.Domain;
namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

public interface IAutoMessagingService
{
    Task<CheckInMessageLog> EnqueueCheckInAsync(Guid patientId, Guid appointmentId, VisitType visitType, QuestionnaireType questionnaireType, DeliveryMethod method, DateTimeOffset scheduledSendAtUtc);
    Task<IReadOnlyList<CheckInMessageLog>> GetLogAsync(Guid? patientId = null, int take = 100);
}
