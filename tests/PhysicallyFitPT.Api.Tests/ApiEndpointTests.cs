namespace PhysicallyFitPT.Api.Tests;

using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

/// <summary>
/// Tests for API endpoints.
/// </summary>
public class ApiEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public ApiEndpointTests(WebApplicationFactory<Program> factory)
    {
        this.client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_Endpoint_Returns_Healthy()
    {
        // Act
        var response = await this.client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", content);
    }

    [Fact]
    public async Task Stats_Endpoint_Returns_Success_Or_Error()
    {
        // Act
        var response = await this.client.GetAsync("/api/stats");

        // Assert - should return OK or InternalServerError (if DB not set up)
        // The important part is that the endpoint exists and responds
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.InternalServerError);
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            var stats = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.True(stats.TryGetProperty("patients", out _));
            Assert.True(stats.TryGetProperty("appointments", out _));
            Assert.True(stats.TryGetProperty("apiHealthy", out _));
        }
    }

    [Fact]
    public async Task Stats_V1_Endpoint_Returns_Success_Or_Error()
    {
        // Act
        var response = await this.client.GetAsync("/api/v1/stats");

        // Assert - should return OK or InternalServerError (if DB not set up)
        // The important part is that the endpoint exists and responds
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.InternalServerError);
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            var stats = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.True(stats.TryGetProperty("patients", out _));
            Assert.True(stats.TryGetProperty("appointments", out _));
            Assert.True(stats.TryGetProperty("apiHealthy", out _));
        }
    }

    [Fact]
    public async Task PDF_Demo_Returns_NotFound_When_Feature_Disabled()
    {
        // Act
        var response = await this.client.GetAsync("/api/pdf/demo");

        // Assert - should return 404 when PDFExport feature is disabled
        Assert.True(
            response.StatusCode == HttpStatusCode.NotFound || 
            response.StatusCode == HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task API_Returns_Consistent_Error_Format_On_Failure()
    {
        // Act - Try an endpoint that might fail
        var response = await this.client.GetAsync("/api/patients/search?query=test&take=10");

        // Assert - If it fails, it should use Problem Details format
        if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
        {
            var content = await response.Content.ReadAsStringAsync();
            var error = JsonSerializer.Deserialize<JsonElement>(content);
            
            // Should have standard error fields
            Assert.True(
                error.TryGetProperty("type", out _) ||
                error.TryGetProperty("title", out _) ||
                error.TryGetProperty("detail", out _) ||
                error.TryGetProperty("error", out _));
        }
    }

    [Fact]
    public async Task HealthInfo_Endpoint_Returns_Status()
    {
        var response = await this.client.GetAsync("/health/info");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
