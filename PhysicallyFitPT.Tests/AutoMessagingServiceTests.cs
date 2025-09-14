// <copyright file="AutoMessagingServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Tests;

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using Xunit;

/// <summary>
/// Tests for the AutoMessagingService class functionality.
/// </summary>
public class AutoMessagingServiceTests
{
  /// <summary>
  /// Tests that enqueuing a message adds a log row to the database.
  /// </summary>
  /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
  [Fact]
  public async Task Enqueue_Adds_Log_Row()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    var factory = new TestDbContextFactory(options);
    AutoMessagingService svc = new AutoMessagingService(factory);

    var log = await svc.EnqueueCheckInAsync(Guid.NewGuid(), Guid.NewGuid(), VisitType.Eval, QuestionnaireType.Eval, DeliveryMethod.SMS, DateTimeOffset.UtcNow.AddHours(1));
    log.Id.Should().NotBeEmpty();
    (await svc.GetLogAsync(null, 10)).Should().ContainSingle();
  }
}
