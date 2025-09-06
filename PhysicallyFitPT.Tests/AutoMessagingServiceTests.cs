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

public class AutoMessagingServiceTests
{
    [Fact]
    public async Task Enqueue_Adds_Log_Row()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var factory = new TestDbContextFactory(options);
        IAutoMessagingService svc = new AutoMessagingService(factory);

        var log = await svc.EnqueueCheckInAsync(Guid.NewGuid(), Guid.NewGuid(), VisitType.Eval, QuestionnaireType.Eval, DeliveryMethod.SMS, DateTimeOffset.UtcNow.AddHours(1));
        log.Id.Should().NotBeEmpty();
        (await svc.GetLogAsync(null, 10)).Should().ContainSingle();
    }
}
