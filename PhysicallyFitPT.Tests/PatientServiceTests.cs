using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using Xunit;

namespace PhysicallyFitPT.Tests;

public class PatientServiceTests
{
    [Fact]
    public async Task SearchAsync_Returns_Empty_On_Empty_DB()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;

        var factory = new TestDbContextFactory(options);
        IPatientService svc = new PatientService(factory);

        var results = await svc.SearchAsync("john", 10);
        results.Should().BeEmpty();
    }
}
