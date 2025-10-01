// <copyright file="WebApiDataService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Web.Services;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhysicallyFitPT.Core;
using PhysicallyFitPT.Shared;

/// <summary>
/// Web API-based data service implementation with Polly resilience patterns.
/// This service is stateless and communicates with the API for all data operations.
/// </summary>
public class WebApiDataService : IDataService
{
  private readonly HttpClient httpClient;
  private readonly ILogger<WebApiDataService> logger;
  private readonly JsonSerializerOptions jsonOptions;
  private readonly string apiBaseUrl;

  /// <summary>
  /// Initializes a new instance of the <see cref="WebApiDataService"/> class.
  /// </summary>
  /// <param name="httpClient">HTTP client configured with Polly resilience.</param>
  /// <param name="apiConfig">API configuration options.</param>
  /// <param name="logger">Logger instance.</param>
  public WebApiDataService(
    HttpClient httpClient,
    IOptions<ApiConfiguration> apiConfig,
    ILogger<WebApiDataService> logger)
  {
    this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    this.apiBaseUrl = apiConfig?.Value?.BaseUrl ?? throw new ArgumentNullException(nameof(apiConfig));

    this.jsonOptions = new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      PropertyNameCaseInsensitive = true,
    };

    // Set base address if not already set
    if (this.httpClient.BaseAddress == null)
    {
      this.httpClient.BaseAddress = new Uri(this.apiBaseUrl);
    }
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<PatientDto>> SearchPatientsAsync(string query, int take = 50, CancellationToken cancellationToken = default)
  {
    try
    {
      this.logger.LogInformation("Searching patients with query: {Query}, take: {Take}", query, take);

      var response = await this.httpClient.GetAsync(
        $"api/patients/search?query={Uri.EscapeDataString(query ?? string.Empty)}&take={take}",
        cancellationToken);

      if (response.IsSuccessStatusCode)
      {
        var patients = await response.Content.ReadFromJsonAsync<IEnumerable<PatientDto>>(this.jsonOptions, cancellationToken) ?? [];
        this.logger.LogInformation("Found {Count} patients", patients.Count());
        return patients;
      }

      this.logger.LogWarning("Patient search failed with status: {StatusCode}", response.StatusCode);
      return [];
    }
    catch (HttpRequestException ex)
    {
      this.logger.LogError(ex, "HTTP error during patient search");
      return [];
    }
    catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
    {
      this.logger.LogError(ex, "Timeout during patient search");
      return [];
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Unexpected error during patient search");
      return [];
    }
  }

  /// <inheritdoc/>
  public async Task<PatientDto?> GetPatientByIdAsync(Guid patientId, CancellationToken cancellationToken = default)
  {
    try
    {
      this.logger.LogInformation("Getting patient by ID: {PatientId}", patientId);

      var response = await this.httpClient.GetAsync($"api/patients/{patientId}", cancellationToken);

      if (response.IsSuccessStatusCode)
      {
        var patient = await response.Content.ReadFromJsonAsync<PatientDto>(this.jsonOptions, cancellationToken);
        this.logger.LogInformation("Retrieved patient: {PatientId}", patientId);
        return patient;
      }

      if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
      {
        this.logger.LogInformation("Patient not found: {PatientId}", patientId);
        return null;
      }

      this.logger.LogWarning("Failed to get patient {PatientId} with status: {StatusCode}", patientId, response.StatusCode);
      return null;
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Error getting patient by ID: {PatientId}", patientId);
      return null;
    }
  }

  /// <inheritdoc/>
  public async Task<AppointmentDto> ScheduleAppointmentAsync(
    Guid patientId,
    DateTimeOffset start,
    DateTimeOffset? end,
    VisitType visitType,
    string? location = null,
    string? clinicianName = null,
    string? clinicianNpi = null,
    CancellationToken cancellationToken = default)
  {
    try
    {
      this.logger.LogInformation("Scheduling appointment for patient: {PatientId}", patientId);

      var request = new
      {
        PatientId = patientId,
        Start = start,
        End = end,
        VisitType = visitType,
        Location = location,
        ClinicianName = clinicianName,
        ClinicianNpi = clinicianNpi,
      };

      var response = await this.httpClient.PostAsJsonAsync("api/appointments", request, this.jsonOptions, cancellationToken);

      if (response.IsSuccessStatusCode)
      {
        var appointment = await response.Content.ReadFromJsonAsync<AppointmentDto>(this.jsonOptions, cancellationToken);
        this.logger.LogInformation("Scheduled appointment: {AppointmentId}", appointment?.Id);
        return appointment ?? throw new InvalidOperationException("Failed to deserialize appointment response");
      }

      var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
      this.logger.LogError("Failed to schedule appointment. Status: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);
      throw new HttpRequestException($"Failed to schedule appointment: {response.StatusCode}");
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Error scheduling appointment for patient: {PatientId}", patientId);
      throw;
    }
  }

  /// <inheritdoc/>
  public async Task<IReadOnlyList<AppointmentDto>> GetUpcomingAppointmentsByPatientAsync(
    Guid patientId,
    DateTimeOffset fromUtc,
    int take = 50,
    CancellationToken cancellationToken = default)
  {
    try
    {
      this.logger.LogInformation("Getting upcoming appointments for patient: {PatientId}", patientId);

      var fromIso = fromUtc.ToString("O");
      var response = await this.httpClient.GetAsync(
        $"api/patients/{patientId}/appointments/upcoming?from={Uri.EscapeDataString(fromIso)}&take={take}",
        cancellationToken);

      if (response.IsSuccessStatusCode)
      {
        var appointments = await response.Content.ReadFromJsonAsync<IReadOnlyList<AppointmentDto>>(this.jsonOptions, cancellationToken) ?? [];
        this.logger.LogInformation("Found {Count} upcoming appointments for patient: {PatientId}", appointments.Count, patientId);
        return appointments;
      }

      this.logger.LogWarning("Failed to get upcoming appointments for patient {PatientId} with status: {StatusCode}", patientId, response.StatusCode);
      return [];
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Error getting upcoming appointments for patient: {PatientId}", patientId);
      return [];
    }
  }

  /// <inheritdoc/>
  public async Task<bool> CancelAppointmentAsync(Guid appointmentId, CancellationToken cancellationToken = default)
  {
    try
    {
      this.logger.LogInformation("Cancelling appointment: {AppointmentId}", appointmentId);

      var response = await this.httpClient.DeleteAsync($"api/appointments/{appointmentId}", cancellationToken);

      if (response.IsSuccessStatusCode)
      {
        this.logger.LogInformation("Cancelled appointment: {AppointmentId}", appointmentId);
        return true;
      }

      this.logger.LogWarning("Failed to cancel appointment {AppointmentId} with status: {StatusCode}", appointmentId, response.StatusCode);
      return false;
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Error cancelling appointment: {AppointmentId}", appointmentId);
      return false;
    }
  }

  /// <inheritdoc/>
  public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      this.logger.LogInformation("Getting dashboard statistics");

      var response = await this.httpClient.GetAsync("api/dashboard/stats", cancellationToken);

      if (response.IsSuccessStatusCode)
      {
        var stats = await response.Content.ReadFromJsonAsync<DashboardStatsDto>(this.jsonOptions, cancellationToken);
        this.logger.LogInformation("Retrieved dashboard statistics");
        return stats ?? new DashboardStatsDto();
      }

      this.logger.LogWarning("Failed to get dashboard stats with status: {StatusCode}", response.StatusCode);
      return new DashboardStatsDto();
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Error getting dashboard statistics");
      return new DashboardStatsDto();
    }
  }

  /// <inheritdoc/>
  public async Task<AppStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      this.logger.LogInformation("Getting application statistics");

      var response = await this.httpClient.GetAsync("api/stats", cancellationToken);

      if (response.IsSuccessStatusCode)
      {
        var stats = await response.Content.ReadFromJsonAsync<AppStatsDto>(this.jsonOptions, cancellationToken);
        this.logger.LogInformation("Retrieved application statistics");
        return stats ?? new AppStatsDto { ApiHealthy = true };
      }

      this.logger.LogWarning("Failed to get app stats with status: {StatusCode}", response.StatusCode);
      return new AppStatsDto { ApiHealthy = false };
    }
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Error getting application statistics");
      return new AppStatsDto { ApiHealthy = false };
    }
  }
}
