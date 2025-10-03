using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PhysicallyFitPT.Core;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using Xunit;

namespace PhysicallyFitPT.Api.Tests;

public class AppointmentServiceTests
{
  [Fact]
  public async Task ScheduleAsync_InvalidatesStatsCache()
  {
    var factory = CreateFactory();
    Guid patientId;
    await using (var ctx = await factory.CreateDbContextAsync())
    {
      var patient = new Patient { FirstName = "Test", LastName = "Patient", MRN = "MRN-1" };
      ctx.Patients.Add(patient);
      await ctx.SaveChangesAsync();
      patientId = patient.Id;
    }

    var invalidator = new TestInvalidator();
    var service = new AppointmentService(factory, NullLogger<AppointmentService>.Instance, invalidator);

    await service.ScheduleAsync(patientId, DateTimeOffset.UtcNow.AddMinutes(10), DateTimeOffset.UtcNow.AddMinutes(40), VisitType.Daily);

    Assert.Equal(1, invalidator.InvalidationCount);
  }

  [Fact]
  public async Task CancelAsync_InvalidatesStatsCacheWhenSuccessful()
  {
    var factory = CreateFactory();
    Guid appointmentId;
    await using (var ctx = await factory.CreateDbContextAsync())
    {
      var patient = new Patient { FirstName = "Test", LastName = "Patient", MRN = "MRN-1" };
      ctx.Patients.Add(patient);
      var appt = new Appointment
      {
        PatientId = patient.Id,
        ScheduledStart = DateTimeOffset.UtcNow.AddHours(1),
        VisitType = VisitType.Daily,
      };
      ctx.Appointments.Add(appt);
      await ctx.SaveChangesAsync();
      appointmentId = appt.Id;
    }

    var invalidator = new TestInvalidator();
    var service = new AppointmentService(factory, NullLogger<AppointmentService>.Instance, invalidator);

    var result = await service.CancelAsync(appointmentId);

    Assert.True(result);
    Assert.Equal(1, invalidator.InvalidationCount);
  }

  private static CountingDbContextFactory CreateFactory()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
      .Options;

    return new CountingDbContextFactory(options);
  }

  private sealed class TestInvalidator : IAppStatsInvalidator
  {
    public int InvalidationCount { get; private set; }

    public void InvalidateCache() => this.InvalidationCount++;
  }

  private sealed class CountingDbContextFactory : IDbContextFactory<ApplicationDbContext>
  {
    private readonly DbContextOptions<ApplicationDbContext> options;

    public CountingDbContextFactory(DbContextOptions<ApplicationDbContext> options) => this.options = options;

    public ApplicationDbContext CreateDbContext() => new(this.options);

    public Task<ApplicationDbContext> CreateDbContextAsync(System.Threading.CancellationToken cancellationToken = default)
      => Task.FromResult(new ApplicationDbContext(this.options));
  }
}
