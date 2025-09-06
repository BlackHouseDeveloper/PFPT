using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;

namespace PhysicallyFitPT.Infrastructure.Services;

public class NoteBuilderService : INoteBuilderService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    public NoteBuilderService(IDbContextFactory<ApplicationDbContext> factory) => _factory = factory;

    public async Task<Note> CreateEvalNoteAsync(Guid patientId, Guid appointmentId)
    {
        using var db = await _factory.CreateDbContextAsync();
        var note = new Note
        {
            PatientId = patientId,
            AppointmentId = appointmentId,
            VisitType = VisitType.Eval,
            Subjective = new SubjectiveSection(),
            Objective  = new ObjectiveSection(),
            Assessment = new AssessmentSection(),
            Plan       = new PlanSection()
        };
        db.Notes.Add(note);
        await db.SaveChangesAsync();
        return note;
    }

    public async Task<Note?> GetAsync(Guid noteId)
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Notes.AsNoTracking().FirstOrDefaultAsync(n => n.Id == noteId);
    }

    public async Task<bool> UpdateObjectiveAsync(Guid noteId, IEnumerable<RomMeasure>? rom = null, IEnumerable<MmtMeasure>? mmt = null, IEnumerable<SpecialTest>? specialTests = null, IEnumerable<OutcomeMeasureScore>? outcomes = null, IEnumerable<ProvidedIntervention>? interventions = null)
    {
        using var db = await _factory.CreateDbContextAsync();
        var note = await db.Notes.Include(n => n.Objective).FirstOrDefaultAsync(n => n.Id == noteId);
        if (note is null) return false;

        note.Objective.Rom = rom?.ToList() ?? note.Objective.Rom;
        note.Objective.Mmt = mmt?.ToList() ?? note.Objective.Mmt;
        note.Objective.SpecialTests = specialTests?.ToList() ?? note.Objective.SpecialTests;
        note.Objective.OutcomeMeasures = outcomes?.ToList() ?? note.Objective.OutcomeMeasures;
        note.Objective.ProvidedInterventions = interventions?.ToList() ?? note.Objective.ProvidedInterventions;

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SignAsync(Guid noteId, string signedBy)
    {
        using var db = await _factory.CreateDbContextAsync();
        var note = await db.Notes.FindAsync(noteId);
        if (note is null) return false;
        note.IsSigned = true;
        note.SignedBy = signedBy;
        note.SignedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }
}
