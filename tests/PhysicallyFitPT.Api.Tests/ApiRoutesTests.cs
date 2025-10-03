using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PhysicallyFitPT.Shared;
using Xunit;

namespace PhysicallyFitPT.Api.Tests;

/// <summary>
/// Unit tests covering the API route helper.
/// </summary>
public class ApiRoutesTests
{
  public ApiRoutesTests() => ApiRoutes.ResetForTests();

  [Fact]
  public void V1BuildsNormalizedRoute()
  {
    var route = ApiRoutes.V1("sync", "snapshot");
    Assert.Equal("/api/v1/sync/snapshot", route);
  }

  [Fact]
  public void CombineIncludesBasePathWhenConfigured()
  {
    ApiRoutes.ConfigureBasePath("pfpt");

    var route = ApiRoutes.Combine("dashboard", "stats");

    Assert.Equal("/pfpt/api/dashboard/stats", route);
  }

  [Fact]
  public void CombineNormalizesDoubleSlashes()
  {
    var route = ApiRoutes.Combine("/dashboard/", "//stats//");
    Assert.Equal("/api/dashboard/stats", route);
  }

  [Fact]
  public void ConfigureBasePathReadsFromConfiguration()
  {
    var configuration = new ConfigurationBuilder()
      .AddInMemoryCollection(new Dictionary<string, string?> { ["Api:BasePath"] = "/pfpt" })
      .Build();

    ApiRoutes.ConfigureBasePath(configuration["Api:BasePath"]);

    var route = ApiRoutes.V1("sync", "snapshot");

    Assert.Equal("/pfpt/api/v1/sync/snapshot", route);
  }
}
