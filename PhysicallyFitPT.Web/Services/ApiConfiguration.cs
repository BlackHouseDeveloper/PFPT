// <copyright file="ApiConfiguration.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Web.Services;

/// <summary>
/// Configuration options for API communication.
/// </summary>
public class ApiConfiguration
{
  /// <summary>
  /// Gets or sets the base URL for the API.
  /// </summary>
  public string BaseUrl { get; set; } = "https://api.physicallyfitpt.com";

  /// <summary>
  /// Gets or sets the timeout for HTTP requests in seconds.
  /// </summary>
  public int TimeoutSeconds { get; set; } = 30;

  /// <summary>
  /// Gets or sets the number of retry attempts for failed requests.
  /// </summary>
  public int RetryAttempts { get; set; } = 3;

  /// <summary>
  /// Gets or sets the circuit breaker failure threshold.
  /// </summary>
  public int CircuitBreakerFailureThreshold { get; set; } = 5;

  /// <summary>
  /// Gets or sets the circuit breaker duration in seconds.
  /// </summary>
  public int CircuitBreakerDurationSeconds { get; set; } = 30;
}
