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

public class AppointmentServiceTests
{
    [Fact]
    public async Task ScheduleAndCancel_Works_With_EmptyDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var factory = new TestDbContextFactory(options);
        var mockLogger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<AppointmentService>();
        IAppointmentService svc = new AppointmentService(factory, mockLogger);

        await using var db = factory.CreateDbContext();
        var patient = new Patient { FirstName = "A", LastName = "B" };
        db.Patients.Add(patient);
        await db.SaveChangesAsync();

        var appt = await svc.ScheduleAsync(patient.Id, DateTimeOffset.UtcNow.AddDays(1), null, VisitType.Eval, "Room 1", "PT Jane", "1234");
        appt.Id.Should().NotBeEmpty();

        (await svc.CancelAsync(appt.Id)).Should().BeTrue();
    }
}
