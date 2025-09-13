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
    /// Gets the storage backend name for diagnostics.
    /// </summary>
    string StorageBackend { get; }

    /// <summary>
    /// Initializes the data store and ensures it's ready for operations.
    /// </summary>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    Task InitializeAsync();

    // Patient operations

    /// <summary>
    /// Gets all patients.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the enumerable of patients.</returns>
    Task<IEnumerable<Patient>> GetPatientsAsync();

    /// <summary>
    /// Gets a patient by identifier.
    /// </summary>
    /// <param name="id">The patient identifier.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the patient or null.</returns>
    Task<Patient?> GetPatientByIdAsync(Guid id);

    /// <summary>
    /// Searches patients by query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="take">The maximum number of results to return.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the enumerable of patients.</returns>
    Task<IEnumerable<Patient>> SearchPatientsAsync(string query, int take = 50);

    /// <summary>
    /// Creates a new patient.
    /// </summary>
    /// <param name="patient">The patient to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created patient.</returns>
    Task<Patient> CreatePatientAsync(Patient patient);

    /// <summary>
    /// Updates an existing patient.
    /// </summary>
    /// <param name="patient">The patient to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated patient.</returns>
    Task<Patient> UpdatePatientAsync(Patient patient);

    /// <summary>
    /// Deletes a patient by identifier.
    /// </summary>
    /// <param name="id">The patient identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeletePatientAsync(Guid id);

    // Appointment operations

    /// <summary>
    /// Gets all appointments.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the enumerable of appointments.</returns>
    Task<IEnumerable<Appointment>> GetAppointmentsAsync();

    /// <summary>
    /// Gets an appointment by identifier.
    /// </summary>
    /// <param name="id">The appointment identifier.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the appointment or null.</returns>
    Task<Appointment?> GetAppointmentByIdAsync(Guid id);

    /// <summary>
    /// Gets appointments by patient identifier.
    /// </summary>
    /// <param name="patientId">The patient identifier.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the enumerable of appointments.</returns>
    Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId);

    /// <summary>
    /// Creates a new appointment.
    /// </summary>
    /// <param name="appointment">The appointment to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created appointment.</returns>
    Task<Appointment> CreateAppointmentAsync(Appointment appointment);

    /// <summary>
    /// Updates an existing appointment.
    /// </summary>
    /// <param name="appointment">The appointment to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated appointment.</returns>
    Task<Appointment> UpdateAppointmentAsync(Appointment appointment);

    /// <summary>
    /// Deletes an appointment by identifier.
    /// </summary>
    /// <param name="id">The appointment identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAppointmentAsync(Guid id);

    // Note operations

    /// <summary>
    /// Gets a note by appointment identifier.
    /// </summary>
    /// <param name="appointmentId">The appointment identifier.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the note or null.</returns>
    Task<Note?> GetNoteByAppointmentIdAsync(Guid appointmentId);

    /// <summary>
    /// Creates a new note.
    /// </summary>
    /// <param name="note">The note to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created note.</returns>
    Task<Note> CreateNoteAsync(Note note);

    /// <summary>
    /// Updates an existing note.
    /// </summary>
    /// <param name="note">The note to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated note.</returns>
    Task<Note> UpdateNoteAsync(Note note);

    // Questionnaire operations

    /// <summary>
    /// Gets all questionnaire definitions.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the enumerable of questionnaire definitions.</returns>
    Task<IEnumerable<QuestionnaireDefinition>> GetQuestionnaireDefinitionsAsync();

    /// <summary>
    /// Gets a questionnaire definition by identifier.
    /// </summary>
    /// <param name="id">The questionnaire definition identifier.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the questionnaire definition or null.</returns>
    Task<QuestionnaireDefinition?> GetQuestionnaireDefinitionByIdAsync(Guid id);

    /// <summary>
    /// Gets questionnaire responses by appointment identifier.
    /// </summary>
    /// <param name="appointmentId">The appointment identifier.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the enumerable of questionnaire responses.</returns>
    Task<IEnumerable<QuestionnaireResponse>> GetQuestionnaireResponsesByAppointmentIdAsync(Guid appointmentId);

    /// <summary>
    /// Creates a new questionnaire response.
    /// </summary>
    /// <param name="response">The questionnaire response to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created questionnaire response.</returns>
    Task<QuestionnaireResponse> CreateQuestionnaireResponseAsync(QuestionnaireResponse response);

    // Reference data operations

    /// <summary>
    /// Gets all CPT codes.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the enumerable of CPT codes.</returns>
    Task<IEnumerable<CptCode>> GetCptCodesAsync();

    /// <summary>
    /// Gets all ICD-10 codes.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the enumerable of ICD-10 codes.</returns>
    Task<IEnumerable<Icd10Code>> GetIcd10CodesAsync();

    // Check-in messaging operations

    /// <summary>
    /// Creates a new check-in message log.
    /// </summary>
    /// <param name="log">The check-in message log to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created check-in message log.</returns>
    Task<CheckInMessageLog> CreateCheckInMessageLogAsync(CheckInMessageLog log);

    /// <summary>
    /// Gets pending check-in messages.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the enumerable of pending check-in message logs.</returns>
    Task<IEnumerable<CheckInMessageLog>> GetPendingCheckInMessagesAsync();

    /// <summary>
    /// Updates a check-in message log.
    /// </summary>
    /// <param name="log">The check-in message log to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateCheckInMessageLogAsync(CheckInMessageLog log);
}
