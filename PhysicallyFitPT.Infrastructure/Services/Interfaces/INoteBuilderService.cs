using PhysicallyFitPT.Domain;
namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

public interface INoteBuilderService
{
    Task<Note> CreateEvalNoteAsync(Guid patientId, Guid appointmentId);
    Task<Note?> GetAsync(Guid noteId);
    Task<bool> UpdateObjectiveAsync(Guid noteId, IEnumerable<RomMeasure>? rom = null, IEnumerable<MmtMeasure>? mmt = null, IEnumerable<SpecialTest>? specialTests = null, IEnumerable<OutcomeMeasureScore>? outcomes = null, IEnumerable<ProvidedIntervention>? interventions = null);
    Task<bool> SignAsync(Guid noteId, string signedBy);
}
