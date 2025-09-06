using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using Xunit;

namespace PhysicallyFitPT.Tests;

public class NoteBuilderServiceTests
{
    [Fact]
    public async Task CreateEvalNote_Then_Sign_Works()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        await using var db = new ApplicationDbContext(options);
        await db.Database.OpenConnectionAsync();
        await db.Database.EnsureCreatedAsync();

        var factory = new PooledDbContextFactory<ApplicationDbContext>(options);
        INoteBuilderService svc = new NoteBuilderService(factory);

        var patient = new Patient { FirstName = "Eval", LastName = "Patient" };
        db.Patients.Add(patient);
        await db.SaveChangesAsync();

        var appt = new Appointment { PatientId = patient.Id, VisitType = VisitType.Eval, ScheduledStart = DateTimeOffset.UtcNow };
        db.Appointments.Add(appt);
        await db.SaveChangesAsync();

        var note = await svc.CreateEvalNoteAsync(patient.Id, appt.Id);
        note.VisitType.Should().Be(VisitType.Eval);

        var ok = await svc.SignAsync(note.Id, "PT Jane");
        ok.Should().BeTrue();
    }
}
