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

  // Patient operations
  Task<IEnumerable<Patient>> GetPatientsAsync();
  Task<Patient?> GetPatientByIdAsync(Guid id);
  Task<IEnumerable<Patient>> SearchPatientsAsync(string query, int take = 50);
  Task<Patient> CreatePatientAsync(Patient patient);
  Task<Patient> UpdatePatientAsync(Patient patient);
  Task DeletePatientAsync(Guid id);

  // Appointment operations
  Task<IEnumerable<Appointment>> GetAppointmentsAsync();
  Task<Appointment?> GetAppointmentByIdAsync(Guid id);
  Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId);
  Task<Appointment> CreateAppointmentAsync(Appointment appointment);
  Task<Appointment> UpdateAppointmentAsync(Appointment appointment);
  Task DeleteAppointmentAsync(Guid id);

  // Note operations
  Task<Note?> GetNoteByAppointmentIdAsync(Guid appointmentId);
  Task<Note> CreateNoteAsync(Note note);
  Task<Note> UpdateNoteAsync(Note note);

  // Questionnaire operations
  Task<IEnumerable<QuestionnaireDefinition>> GetQuestionnaireDefinitionsAsync();
  Task<QuestionnaireDefinition?> GetQuestionnaireDefinitionByIdAsync(Guid id);
  Task<IEnumerable<QuestionnaireResponse>> GetQuestionnaireResponsesByAppointmentIdAsync(Guid appointmentId);
  Task<QuestionnaireResponse> CreateQuestionnaireResponseAsync(QuestionnaireResponse response);

  // Reference data operations
  Task<IEnumerable<CptCode>> GetCptCodesAsync();
  Task<IEnumerable<Icd10Code>> GetIcd10CodesAsync();

  // Check-in messaging operations
  Task<CheckInMessageLog> CreateCheckInMessageLogAsync(CheckInMessageLog log);
  Task<IEnumerable<CheckInMessageLog>> GetPendingCheckInMessagesAsync();
  Task UpdateCheckInMessageLogAsync(CheckInMessageLog log);
}