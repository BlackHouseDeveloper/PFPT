using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;

// and if you use AddPooledDbContextFactory in test DI:
using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace PhysicallyFitPT.Tests;

public class AppointmentServiceTests
{
    [Fact]
    public async Task ScheduleAndCancel_Works_With_EmptyDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        await using var db = new ApplicationDbContext(options);
        await db.Database.OpenConnectionAsync();
        await db.Database.EnsureCreatedAsync();

        var factory = new PooledDbContextFactory<ApplicationDbContext>(options);
        IAppointmentService svc = new AppointmentService(factory);

        var patient = new Patient { FirstName = "A", LastName = "B" };
        db.Patients.Add(patient);
        await db.SaveChangesAsync();

        var appt = await svc.ScheduleAsync(patient.Id, DateTimeOffset.UtcNow.AddDays(1), null, VisitType.Eval, "Room 1", "PT Jane", "1234");
        appt.Id.Should().NotBeEmpty();

        (await svc.CancelAsync(appt.Id)).Should().BeTrue();
    }
}
