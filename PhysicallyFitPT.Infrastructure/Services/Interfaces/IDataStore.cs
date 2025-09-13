// <copyright file="IDataStore.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

using PhysicallyFitPT.Domain;

/// <summary>
/// Platform-agnostic data store abstraction for CRUD operations.
/// Implementations handle platform-specific storage (SQLite for mobile, IndexedDB for browser).
/// </summary>
public interface IDataStore
{
  /// <summary>
  /// Initializes the data store and ensures it's ready for operations.
  /// </summary>
  /// <returns>A task representing the asynchronous initialization operation.</returns>
  Task InitializeAsync();

  /// <summary>
  /// Gets the storage backend name for diagnostics.
  /// </summary>
  string StorageBackend { get; }
/// <inheritdoc/>

  // Patient operations
  Task<IEnumerable<Patient>> GetPatientsAsync();
  /// <inheritdoc/>
  Task<Patient?> GetPatientByIdAsync(Guid id);
  /// <inheritdoc/>
  Task<IEnumerable<Patient>> SearchPatientsAsync(string query, int take = 50);
  /// <inheritdoc/>
  Task<Patient> CreatePatientAsync(Patient patient);
  /// <inheritdoc/>
  Task<Patient> UpdatePatientAsync(Patient patient);
  /// <inheritdoc/>
  Task DeletePatientAsync(Guid id);
/// <inheritdoc/>

  // Appointment operations
  Task<IEnumerable<Appointment>> GetAppointmentsAsync();
  /// <inheritdoc/>
  Task<Appointment?> GetAppointmentByIdAsync(Guid id);
  /// <inheritdoc/>
  Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId);
  /// <inheritdoc/>
  Task<Appointment> CreateAppointmentAsync(Appointment appointment);
  /// <inheritdoc/>
  Task<Appointment> UpdateAppointmentAsync(Appointment appointment);
  /// <inheritdoc/>
  Task DeleteAppointmentAsync(Guid id);
/// <inheritdoc/>

  // Note operations
  Task<Note?> GetNoteByAppointmentIdAsync(Guid appointmentId);
  /// <inheritdoc/>
  Task<Note> CreateNoteAsync(Note note);
  /// <inheritdoc/>
  Task<Note> UpdateNoteAsync(Note note);
/// <inheritdoc/>

  // Questionnaire operations
  Task<IEnumerable<QuestionnaireDefinition>> GetQuestionnaireDefinitionsAsync();
  /// <inheritdoc/>
  Task<QuestionnaireDefinition?> GetQuestionnaireDefinitionByIdAsync(Guid id);
  /// <inheritdoc/>
  Task<IEnumerable<QuestionnaireResponse>> GetQuestionnaireResponsesByAppointmentIdAsync(Guid appointmentId);
  /// <inheritdoc/>
  Task<QuestionnaireResponse> CreateQuestionnaireResponseAsync(QuestionnaireResponse response);
/// <inheritdoc/>

  // Reference data operations
  Task<IEnumerable<CptCode>> GetCptCodesAsync();
  /// <inheritdoc/>
  Task<IEnumerable<Icd10Code>> GetIcd10CodesAsync();
/// <inheritdoc/>

  // Check-in messaging operations
  Task<CheckInMessageLog> CreateCheckInMessageLogAsync(CheckInMessageLog log);
  /// <inheritdoc/>
  Task<IEnumerable<CheckInMessageLog>> GetPendingCheckInMessagesAsync();
  /// <inheritdoc/>
  Task UpdateCheckInMessageLogAsync(CheckInMessageLog log);
}
