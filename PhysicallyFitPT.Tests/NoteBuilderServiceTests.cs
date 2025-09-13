// <copyright file="NoteBuilderServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;

namespace PhysicallyFitPT.Tests;

/// <summary>
/// Tests for the NoteBuilderService class functionality.
/// </summary>
public class NoteBuilderServiceTests
{
    /// <summary>
    /// Tests that creating an evaluation note and signing it works correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Fact]
    public async Task CreateEvalNote_Then_Sign_Works()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var factory = new TestDbContextFactory(options);
        var mockLogger = new NullLogger<NoteBuilderService>();
        INoteBuilderService svc = new NoteBuilderService(factory, mockLogger);

        await using var db = factory.CreateDbContext();

        var patient = new Patient { FirstName = "Eval", LastName = "Patient" };
        db.Patients.Add(patient);
        await db.SaveChangesAsync();

        var appt = new Appointment { PatientId = patient.Id, VisitType = VisitType.Eval, ScheduledStart = DateTimeOffset.UtcNow };
        db.Appointments.Add(appt);
        await db.SaveChangesAsync();

        var noteDto = await svc.CreateEvalNoteAsync(patient.Id, appt.Id);
        noteDto.VisitType.Should().Be(VisitType.Eval.ToString());

        var ok = await svc.SignAsync(noteDto.Id, "PT Jane");
        ok.Should().BeTrue();
    }
}
