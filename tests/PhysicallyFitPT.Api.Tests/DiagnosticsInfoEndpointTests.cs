using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhysicallyFitPT.Infrastructure.Data;
using Xunit;

namespace PhysicallyFitPT.Api.Tests;

public class DiagnosticsInfoEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
  private readonly WebApplicationFactory<Program> factory;

  public DiagnosticsInfoEndpointTests(WebApplicationFactory<Program> factory) => this.factory = factory;

  [Fact]
  public async Task DiagnosticsEndpoint_Development_ReturnsHeaderWhenEnabled()
  {
    using var client = this.factory.WithWebHostBuilder(builder =>
    {
      builder.UseEnvironment("Development");
      builder.ConfigureServices(services => ConfigureTestDatabase(services));
    }).CreateClient();

    var response = await client.GetAsync("/api/v1/diagnostics/info");

    response.EnsureSuccessStatusCode();
    Assert.True(response.Headers.TryGetValues("PFPT-Diagnostics", out var values) && values.Contains("true"));
  }

  [Fact]
  public async Task DiagnosticsEndpoint_Production_ReturnsUnauthorizedWhenAnonymous()
  {
    using var client = this.factory.WithWebHostBuilder(builder =>
    {
      builder.UseEnvironment("Production");
      builder.ConfigureServices(services => ConfigureTestDatabase(services));
    }).CreateClient();

    var response = await client.GetAsync("/api/v1/diagnostics/info");

    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }

  [Fact]
  public async Task DiagnosticsEndpoint_Disabled_ReturnsNotFoundWithoutHeader()
  {
    using var client = this.factory.WithWebHostBuilder(builder =>
    {
      builder.UseEnvironment("Development");
      builder.ConfigureAppConfiguration((context, configBuilder) =>
      {
        var overrides = new Dictionary<string, string?>
        {
          ["App:DeveloperMode"] = "false",
        };
        configBuilder.AddInMemoryCollection(overrides);
      });
      builder.ConfigureServices(services => ConfigureTestDatabase(services));
    })
      .CreateClient();

    var response = await client.GetAsync("/api/v1/diagnostics/info");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    Assert.False(response.Headers.Contains("PFPT-Diagnostics"));
}

  private static void ConfigureTestDatabase(IServiceCollection services)
  {
    var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(IDbContextFactory<ApplicationDbContext>));
    if (descriptor is not null)
    {
      services.Remove(descriptor);
    }

    services.AddDbContextFactory<ApplicationDbContext>(options =>
      options.UseInMemoryDatabase($"DiagnosticsTests-{Guid.NewGuid()}")
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
  }
}
