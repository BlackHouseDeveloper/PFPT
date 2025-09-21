// <copyright file="WebApiDataServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Tests;

using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using PhysicallyFitPT.Shared;
using PhysicallyFitPT.Web.Services;
using Xunit;

/// <summary>
/// Tests for the WebApiDataService implementation.
/// </summary>
public class WebApiDataServiceTests
{
  /// <summary>
  /// Cached JsonSerializerOptions instance to avoid creating new instances for every serialization operation.
  /// </summary>
  private static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
  };

  private readonly Mock<ILogger<WebApiDataService>> mockLogger;
  private readonly Mock<HttpMessageHandler> mockHttpMessageHandler;
  private readonly HttpClient httpClient;
  private readonly IOptions<ApiConfiguration> apiConfig;

  /// <summary>
  /// Initializes a new instance of the <see cref="WebApiDataServiceTests"/> class.
  /// </summary>
  public WebApiDataServiceTests()
  {
    this.mockLogger = new Mock<ILogger<WebApiDataService>>();
    this.mockHttpMessageHandler = new Mock<HttpMessageHandler>();
    this.httpClient = new HttpClient(this.mockHttpMessageHandler.Object)
    {
      BaseAddress = new Uri("https://test-api.com/"),
    };

    this.apiConfig = Options.Create(new ApiConfiguration
    {
      BaseUrl = "https://test-api.com",
      TimeoutSeconds = 30,
    });
  }

  /// <summary>
  /// Test that SearchPatientsAsync returns empty collection when API returns empty response.
  /// </summary>
  /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
  [Fact]
  public async Task SearchPatientsAsync_ReturnsEmpty_WhenApiReturnsEmptyResponse()
  {
    // Arrange
    const string emptyResponse = "[]";
    this.SetupHttpResponse(HttpStatusCode.OK, emptyResponse);

    var service = new WebApiDataService(this.httpClient, this.apiConfig, this.mockLogger.Object);

    // Act
    var result = await service.SearchPatientsAsync("test", 10);

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result);

    // Verify the correct endpoint was called
    this.mockHttpMessageHandler.Protected().Verify(
      "SendAsync",
      Times.Once(),
      ItExpr.Is<HttpRequestMessage>(req =>
        req.Method == HttpMethod.Get &&
        req.RequestUri!.ToString().Contains("api/patients/search")),
      ItExpr.IsAny<CancellationToken>());
  }

  /// <summary>
  /// Test that SearchPatientsAsync handles HTTP errors gracefully.
  /// </summary>
  /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
  [Fact]
  public async Task SearchPatientsAsync_ReturnsEmpty_WhenApiReturns404()
  {
    // Arrange
    this.SetupHttpResponse(HttpStatusCode.NotFound, string.Empty);

    var service = new WebApiDataService(this.httpClient, this.apiConfig, this.mockLogger.Object);

    // Act
    var result = await service.SearchPatientsAsync("test", 10);

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result);
  }

  /// <summary>
  /// Test that SearchPatientsAsync returns patients when API returns valid data.
  /// </summary>
  /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
  [Fact]
  public async Task SearchPatientsAsync_ReturnsPatients_WhenApiReturnsValidData()
  {
    // Arrange
    var patients = new[]
    {
      new PatientDto { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
      new PatientDto { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" },
    };

    var jsonResponse = JsonSerializer.Serialize(patients, JsonOptions);

    this.SetupHttpResponse(HttpStatusCode.OK, jsonResponse);

    var service = new WebApiDataService(this.httpClient, this.apiConfig, this.mockLogger.Object);

    // Act
    var result = await service.SearchPatientsAsync("test", 10);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(2, result.Count());

    var resultList = result.ToList();
    Assert.Equal("John", resultList[0].FirstName);
    Assert.Equal("Jane", resultList[1].FirstName);
  }

  /// <summary>
  /// Test that GetPatientByIdAsync returns null when patient is not found.
  /// </summary>
  /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
  [Fact]
  public async Task GetPatientByIdAsync_ReturnsNull_WhenPatientNotFound()
  {
    // Arrange
    this.SetupHttpResponse(HttpStatusCode.NotFound, string.Empty);

    var service = new WebApiDataService(this.httpClient, this.apiConfig, this.mockLogger.Object);
    var patientId = Guid.NewGuid();

    // Act
    var result = await service.GetPatientByIdAsync(patientId);

    // Assert
    Assert.Null(result);

    // Verify the correct endpoint was called
    this.mockHttpMessageHandler.Protected().Verify(
      "SendAsync",
      Times.Once(),
      ItExpr.Is<HttpRequestMessage>(req =>
        req.Method == HttpMethod.Get &&
        req.RequestUri!.ToString().Contains($"api/patients/{patientId}")),
      ItExpr.IsAny<CancellationToken>());
  }

  private void SetupHttpResponse(HttpStatusCode statusCode, string content)
  {
    var response = new HttpResponseMessage(statusCode)
    {
      Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json"),
    };

    this.mockHttpMessageHandler
      .Protected()
      .Setup<Task<HttpResponseMessage>>(
        "SendAsync",
        ItExpr.IsAny<HttpRequestMessage>(),
        ItExpr.IsAny<CancellationToken>())
      .ReturnsAsync(response);
  }
}
