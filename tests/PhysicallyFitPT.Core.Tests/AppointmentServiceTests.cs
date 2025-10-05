// <copyright file="AppointmentServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Tests;

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PhysicallyFitPT.Core;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using Xunit;

/// <summary>
/// Tests for the AppointmentService class functionality.
/// </summary>
public class AppointmentServiceTests
{
  /// <summary>
  /// Tests that appointment scheduling and cancellation works correctly with an empty database.
  /// </summary>
  /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
  [Fact]
  public async Task ScheduleAndCancel_Works_With_EmptyDb()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

    var factory = new TestDbContextFactory(options);
    var mockLogger = new NullLogger<AppointmentService>();
    var invalidator = new NoOpStatsInvalidator();
    IAppointmentService svc = new AppointmentService(factory, mockLogger, invalidator);

    await using var db = factory.CreateDbContext();
    var patient = new Patient { FirstName = "A", LastName = "B" };
    db.Patients.Add(patient);
    await db.SaveChangesAsync();

    var apptDto = await svc.ScheduleAsync(patient.Id, DateTimeOffset.UtcNow.AddDays(1), null, VisitType.Eval, "Room 1", "PT Jane", "1234");
    apptDto.Id.Should().NotBeEmpty();

    (await svc.CancelAsync(apptDto.Id)).Should().BeTrue();
  }
}

/// <summary>
/// Simple stats invalidator used by unit tests.
/// </summary>
internal sealed class NoOpStatsInvalidator : IAppStatsInvalidator
{
  public int Calls { get; private set; }

  public void InvalidateCache() => this.Calls++;
}
