// <copyright file="NoteBuilderServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Tests
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using FluentAssertions;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Logging.Abstractions;
  using PhysicallyFitPT.Domain;
  using PhysicallyFitPT.Infrastructure.Data;
  using PhysicallyFitPT.Infrastructure.Services;
  using PhysicallyFitPT.Infrastructure.Services.Interfaces;
  using PhysicallyFitPT.Shared;
  using Xunit;

  public class NoteBuilderServiceTests
    {
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

        [Fact]
        public async Task CreateDailyNote_ReturnsCorrectVisitType()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var factory = new TestDbContextFactory(options);
            var mockLogger = new NullLogger<NoteBuilderService>();
            INoteBuilderService svc = new NoteBuilderService(factory, mockLogger);

            await using var db = factory.CreateDbContext();

            var patient = new Patient { FirstName = "Daily", LastName = "Patient" };
            db.Patients.Add(patient);
            await db.SaveChangesAsync();

            var appt = new Appointment { PatientId = patient.Id, VisitType = VisitType.Daily, ScheduledStart = DateTimeOffset.UtcNow };
            db.Appointments.Add(appt);
            await db.SaveChangesAsync();

            var noteDto = await svc.CreateDailyNoteAsync(patient.Id, appt.Id);
            noteDto.VisitType.Should().Be(VisitType.Daily.ToString());
            noteDto.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CreateProgressNote_ReturnsCorrectVisitType()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var factory = new TestDbContextFactory(options);
            var mockLogger = new NullLogger<NoteBuilderService>();
            INoteBuilderService svc = new NoteBuilderService(factory, mockLogger);

            await using var db = factory.CreateDbContext();

            var patient = new Patient { FirstName = "Progress", LastName = "Patient" };
            db.Patients.Add(patient);
            await db.SaveChangesAsync();

            var appt = new Appointment { PatientId = patient.Id, VisitType = VisitType.Progress, ScheduledStart = DateTimeOffset.UtcNow };
            db.Appointments.Add(appt);
            await db.SaveChangesAsync();

            var noteDto = await svc.CreateProgressNoteAsync(patient.Id, appt.Id);
            noteDto.VisitType.Should().Be(VisitType.Progress.ToString());
        }

        [Fact]
        public async Task CreateDischargeNote_ReturnsCorrectVisitType()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var factory = new TestDbContextFactory(options);
            var mockLogger = new NullLogger<NoteBuilderService>();
            INoteBuilderService svc = new NoteBuilderService(factory, mockLogger);

            await using var db = factory.CreateDbContext();

            var patient = new Patient { FirstName = "Discharge", LastName = "Patient" };
            db.Patients.Add(patient);
            await db.SaveChangesAsync();

            var appt = new Appointment { PatientId = patient.Id, VisitType = VisitType.Discharge, ScheduledStart = DateTimeOffset.UtcNow };
            db.Appointments.Add(appt);
            await db.SaveChangesAsync();

            var noteDto = await svc.CreateDischargeNoteAsync(patient.Id, appt.Id);
            noteDto.VisitType.Should().Be(VisitType.Discharge.ToString());
        }

        [Fact]
        public async Task UpdateObjectiveAsync_WithValidCptCode_Works()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var factory = new TestDbContextFactory(options);
            var mockLogger = new NullLogger<NoteBuilderService>();
            INoteBuilderService svc = new NoteBuilderService(factory, mockLogger);

            await using var db = factory.CreateDbContext();

            // Set up test data
            var patient = new Patient { FirstName = "Test", LastName = "Patient" };
            db.Patients.Add(patient);

            var cptCode = new CptCode { Code = "97110", Description = "Therapeutic Exercise" };
            db.CptCodes.Add(cptCode);

            await db.SaveChangesAsync();

            var appt = new Appointment { PatientId = patient.Id, VisitType = VisitType.Eval, ScheduledStart = DateTimeOffset.UtcNow };
            db.Appointments.Add(appt);
            await db.SaveChangesAsync();

            var noteDto = await svc.CreateEvalNoteAsync(patient.Id, appt.Id);

            // Test updating with valid CPT code
            var interventions = new List<ProvidedInterventionDto>
            {
                new ProvidedInterventionDto { CptCode = "97110", Units = 2, Minutes = 30 }
            };

            var success = await svc.UpdateObjectiveAsync(noteDto.Id, interventions: interventions);
            success.Should().BeTrue();

            // Verify the intervention was added with auto-filled description
            var updatedNote = await svc.GetAsync(noteDto.Id);
            updatedNote.Should().NotBeNull();
            updatedNote!.Objective.ProvidedInterventions.Should().HaveCount(1);
            updatedNote.Objective.ProvidedInterventions.First().CptCode.Should().Be("97110");
            updatedNote.Objective.ProvidedInterventions.First().Description.Should().Be("Therapeutic Exercise");
        }

        [Fact]
        public async Task UpdateObjectiveAsync_WithInvalidCptCode_ReturnsFalse()
        {
            // The CPT validation works - it returns false when invalid CPT code is encountered
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var factory = new TestDbContextFactory(options);
            var mockLogger = new NullLogger<NoteBuilderService>();
            INoteBuilderService svc = new NoteBuilderService(factory, mockLogger);

            await using var db = factory.CreateDbContext();

            var patient = new Patient { FirstName = "Test", LastName = "Patient" };
            db.Patients.Add(patient);
            await db.SaveChangesAsync();

            var appt = new Appointment { PatientId = patient.Id, VisitType = VisitType.Eval, ScheduledStart = DateTimeOffset.UtcNow };
            db.Appointments.Add(appt);
            await db.SaveChangesAsync();

            var noteDto = await svc.CreateEvalNoteAsync(patient.Id, appt.Id);

            // Invalid CPT code should cause the operation to fail and return false
            var interventions = new List<ProvidedInterventionDto>
            {
                new ProvidedInterventionDto { CptCode = "INVALID", Units = 2 }
            };

            var result = await svc.UpdateObjectiveAsync(noteDto.Id, interventions: interventions);
            result.Should().BeFalse(); // Validation fails, so method returns false
        }

        [Fact]
        public async Task UpdateAssessmentAsync_WithValidIcd10Code_Works()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var factory = new TestDbContextFactory(options);
            var mockLogger = new NullLogger<NoteBuilderService>();
            INoteBuilderService svc = new NoteBuilderService(factory, mockLogger);

            await using var db = factory.CreateDbContext();

            // Set up test data
            var patient = new Patient { FirstName = "Test", LastName = "Patient" };
            db.Patients.Add(patient);

            var icd10Code = new Icd10Code { Code = "M25.561", Description = "Pain in right knee" };
            db.Icd10Codes.Add(icd10Code);

            await db.SaveChangesAsync();

            var appt = new Appointment { PatientId = patient.Id, VisitType = VisitType.Eval, ScheduledStart = DateTimeOffset.UtcNow };
            db.Appointments.Add(appt);
            await db.SaveChangesAsync();

            var noteDto = await svc.CreateEvalNoteAsync(patient.Id, appt.Id);

            // Test updating with valid ICD-10 code
            var icd10Codes = new List<Icd10LinkDto>
            {
                new Icd10LinkDto { Code = "M25.561" }
            };

            var success = await svc.UpdateAssessmentAsync(noteDto.Id, "Test clinical impression", "Good", icd10Codes);
            success.Should().BeTrue();

            // Verify the ICD-10 code was added with auto-filled description
            var updatedNote = await svc.GetAsync(noteDto.Id);
            updatedNote.Should().NotBeNull();
            updatedNote!.Assessment.ClinicalImpression.Should().Be("Test clinical impression");
            updatedNote.Assessment.RehabPotential.Should().Be("Good");
            updatedNote.Assessment.Icd10Codes.Should().HaveCount(1);
            updatedNote.Assessment.Icd10Codes.First().Code.Should().Be("M25.561");
            updatedNote.Assessment.Icd10Codes.First().Description.Should().Be("Pain in right knee");
        }

        [Fact]
        public async Task UpdateAssessmentAsync_WithInvalidIcd10Code_ThrowsException()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var factory = new TestDbContextFactory(options);
            var mockLogger = new NullLogger<NoteBuilderService>();
            INoteBuilderService svc = new NoteBuilderService(factory, mockLogger);

            await using var db = factory.CreateDbContext();

            var patient = new Patient { FirstName = "Test", LastName = "Patient" };
            db.Patients.Add(patient);
            await db.SaveChangesAsync();

            var appt = new Appointment { PatientId = patient.Id, VisitType = VisitType.Eval, ScheduledStart = DateTimeOffset.UtcNow };
            db.Appointments.Add(appt);
            await db.SaveChangesAsync();

            var noteDto = await svc.CreateEvalNoteAsync(patient.Id, appt.Id);

            // Test updating with invalid ICD-10 code
            var icd10Codes = new List<Icd10LinkDto>
            {
                new Icd10LinkDto { Code = "INVALID" }
            };

            await svc.Invoking(s => s.UpdateAssessmentAsync(noteDto.Id, icd10Codes: icd10Codes))
                .Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Invalid ICD-10 code: INVALID*");
        }
    }
}
