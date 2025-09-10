// <copyright file="AppointmentServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Tests
{
  using System;
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

  public class AppointmentServiceTests
    {
        [Fact]
        public async Task ScheduleAndCancel_Works_With_EmptyDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var factory = new TestDbContextFactory(options);
            var mockLogger = new NullLogger<AppointmentService>();
            IAppointmentService svc = new AppointmentService(factory, mockLogger);

            await using var db = factory.CreateDbContext();
            var patient = new Patient { FirstName = "A", LastName = "B" };
            db.Patients.Add(patient);
            await db.SaveChangesAsync();

            var apptDto = await svc.ScheduleAsync(patient.Id, DateTimeOffset.UtcNow.AddDays(1), null, VisitType.Eval, "Room 1", "PT Jane", "1234");
            apptDto.Id.Should().NotBeEmpty();

            (await svc.CancelAsync(apptDto.Id)).Should().BeTrue();
        }

        [Fact]
        public async Task ScheduleAsync_InvalidPatientId_ThrowsArgumentException()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var factory = new TestDbContextFactory(options);
            var mockLogger = new NullLogger<AppointmentService>();
            IAppointmentService svc = new AppointmentService(factory, mockLogger);

            await svc.Invoking(s => s.ScheduleAsync(Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(1), null, VisitType.Eval))
                .Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Patient not found*");
        }

        [Fact]
        public async Task GetByIdAsync_ExistingAppointment_ReturnsAppointment()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var factory = new TestDbContextFactory(options);
            var mockLogger = new NullLogger<AppointmentService>();
            IAppointmentService svc = new AppointmentService(factory, mockLogger);

            // Create test patient and appointment
            await using var db = factory.CreateDbContext();
            var patient = new Patient { FirstName = "Test", LastName = "Patient" };
            db.Patients.Add(patient);
            await db.SaveChangesAsync();

            var apptDto = await svc.ScheduleAsync(patient.Id, DateTimeOffset.UtcNow.AddDays(1), null, VisitType.Daily, "Room 2");

            // Get by ID
            var result = await svc.GetByIdAsync(apptDto.Id);

            result.Should().NotBeNull();
            result!.PatientId.Should().Be(patient.Id);
            result.VisitType.Should().Be("Daily");
        }

        [Fact]
        public async Task UpdateAsync_ExistingAppointment_ReturnsUpdatedAppointment()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var factory = new TestDbContextFactory(options);
            var mockLogger = new NullLogger<AppointmentService>();
            IAppointmentService svc = new AppointmentService(factory, mockLogger);

            // Create test patient and appointment
            await using var db = factory.CreateDbContext();
            var patient = new Patient { FirstName = "Test", LastName = "Patient" };
            db.Patients.Add(patient);
            await db.SaveChangesAsync();

            var apptDto = await svc.ScheduleAsync(patient.Id, DateTimeOffset.UtcNow.AddDays(1), null, VisitType.Eval, "Room 1");

            // Update the appointment
            var updateDto = new AppointmentDto
            {
                Id = apptDto.Id,
                PatientId = apptDto.PatientId,
                VisitType = "Progress",
                ScheduledStart = apptDto.ScheduledStart.AddHours(1),
                Location = "Room 3",
                ClinicianName = "PT Smith"
            };

            var result = await svc.UpdateAsync(apptDto.Id, updateDto);

            result.Should().NotBeNull();
            result!.VisitType.Should().Be("Progress");
            result.Location.Should().Be("Room 3");
            result.ClinicianName.Should().Be("PT Smith");
        }

        [Fact]
        public async Task GetUpcomingByPatientAsync_ReturnsOrderedAppointments()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var factory = new TestDbContextFactory(options);
            var mockLogger = new NullLogger<AppointmentService>();
            IAppointmentService svc = new AppointmentService(factory, mockLogger);

            // Create test patient
            await using var db = factory.CreateDbContext();
            var patient = new Patient { FirstName = "Test", LastName = "Patient" };
            db.Patients.Add(patient);
            await db.SaveChangesAsync();

            // Create multiple appointments
            var now = DateTimeOffset.UtcNow;
            await svc.ScheduleAsync(patient.Id, now.AddDays(3), null, VisitType.Daily);
            await svc.ScheduleAsync(patient.Id, now.AddDays(1), null, VisitType.Eval);
            await svc.ScheduleAsync(patient.Id, now.AddDays(2), null, VisitType.Progress);

            var results = await svc.GetUpcomingByPatientAsync(patient.Id, now);

            results.Should().HaveCount(3);
            // Should be ordered by ScheduledStart
            results.ElementAt(0).VisitType.Should().Be("Eval");
            results.ElementAt(1).VisitType.Should().Be("Progress");
            results.ElementAt(2).VisitType.Should().Be("Daily");
        }

        [Fact]
        public async Task UpdateAsync_InvalidEndTime_ThrowsArgumentException()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var factory = new TestDbContextFactory(options);
            var mockLogger = new NullLogger<AppointmentService>();
            IAppointmentService svc = new AppointmentService(factory, mockLogger);

            // Create test patient and appointment
            await using var db = factory.CreateDbContext();
            var patient = new Patient { FirstName = "Test", LastName = "Patient" };
            db.Patients.Add(patient);
            await db.SaveChangesAsync();

            var apptDto = await svc.ScheduleAsync(patient.Id, DateTimeOffset.UtcNow.AddDays(1), null, VisitType.Eval);

            // Try to update with invalid end time
            var invalidUpdate = new AppointmentDto
            {
                ScheduledStart = DateTimeOffset.UtcNow.AddDays(1),
                ScheduledEnd = DateTimeOffset.UtcNow.AddDays(1).AddMinutes(-30), // End before start
                VisitType = "Eval"
            };

            await svc.Invoking(s => s.UpdateAsync(apptDto.Id, invalidUpdate))
                .Should().ThrowAsync<ArgumentException>()
                .WithMessage("*End time must be after start time*");
        }
    }
}
