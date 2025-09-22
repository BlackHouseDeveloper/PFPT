// <copyright file="IDataService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PhysicallyFitPT.Core;

/// <summary>
/// Unified interface for data access abstraction across platforms.
/// Provides common data operations for both Web (API-based) and MAUI (SQLite-based) implementations.
/// </summary>
public interface IDataService
{
  /// <summary>
  /// Searches for patients based on a query string.
  /// </summary>
  /// <param name="query">The search query to match against patient names.</param>
  /// <param name="take">The maximum number of patients to return.</param>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>A collection of patient DTOs matching the search criteria.</returns>
  Task<IEnumerable<PatientDto>> SearchPatientsAsync(string query, int take = 50, CancellationToken cancellationToken = default);

  /// <summary>
  /// Gets a patient by their unique identifier.
  /// </summary>
  /// <param name="patientId">The unique identifier of the patient.</param>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>The patient DTO if found, null otherwise.</returns>
  Task<PatientDto?> GetPatientByIdAsync(Guid patientId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Schedules a new appointment for a patient.
  /// </summary>
  /// <param name="patientId">The unique identifier of the patient.</param>
  /// <param name="start">The scheduled start time of the appointment.</param>
  /// <param name="end">The scheduled end time of the appointment.</param>
  /// <param name="visitType">The type of visit for the appointment.</param>
  /// <param name="location">The location where the appointment will take place.</param>
  /// <param name="clinicianName">The name of the clinician conducting the appointment.</param>
  /// <param name="clinicianNpi">The NPI number of the clinician.</param>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>The created appointment DTO.</returns>
  Task<AppointmentDto> ScheduleAppointmentAsync(Guid patientId, DateTimeOffset start, DateTimeOffset? end, VisitType visitType, string? location = null, string? clinicianName = null, string? clinicianNpi = null, CancellationToken cancellationToken = default);

  /// <summary>
  /// Gets upcoming appointments for a specific patient.
  /// </summary>
  /// <param name="patientId">The unique identifier of the patient.</param>
  /// <param name="fromUtc">The starting date/time to search from in UTC.</param>
  /// <param name="take">The maximum number of appointments to return.</param>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>A list of upcoming appointment DTOs.</returns>
  Task<IReadOnlyList<AppointmentDto>> GetUpcomingAppointmentsByPatientAsync(Guid patientId, DateTimeOffset fromUtc, int take = 50, CancellationToken cancellationToken = default);

  /// <summary>
  /// Cancels an existing appointment.
  /// </summary>
  /// <param name="appointmentId">The unique identifier of the appointment to cancel.</param>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>True if the appointment was cancelled successfully, false otherwise.</returns>
  Task<bool> CancelAppointmentAsync(Guid appointmentId, CancellationToken cancellationToken = default);
}
