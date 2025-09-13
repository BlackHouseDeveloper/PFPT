// <copyright file="SqliteDataStore.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;

/// <summary>
/// SQLite-based data store implementation for mobile and desktop platforms.
/// Uses Entity Framework Core with SQLite provider.
/// </summary>
public class SqliteDataStore : IDataStore
{
  private readonly IDbContextFactory<ApplicationDbContext> contextFactory;

  /// <summary>
  /// Initializes a new instance of the <see cref="SqliteDataStore"/> class.
  /// </summary>
  /// <param name="contextFactory">The database context factory.</param>
  public SqliteDataStore(IDbContextFactory<ApplicationDbContext> contextFactory)
  {
    this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
  }

  /// <inheritdoc/>
  public string StorageBackend => "SQLite";

  /// <inheritdoc/>
  public async Task InitializeAsync()
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    await context.Database.EnsureCreatedAsync();
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<Patient>> GetPatientsAsync()
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    return await context.Patients.AsNoTracking().OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToListAsync();
  }

  /// <inheritdoc/>
  public async Task<Patient?> GetPatientByIdAsync(Guid id)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    return await context.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<Patient>> SearchPatientsAsync(string query, int take = 50)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    var q = (query ?? string.Empty).Trim().ToLower();
    if (string.IsNullOrEmpty(q))
    {
      return await context.Patients.AsNoTracking().OrderBy(p => p.LastName).ThenBy(p => p.FirstName).Take(take).ToListAsync();
    }

    var like = $"%{q}%";
    return await context.Patients.AsNoTracking()
      .Where(p => EF.Functions.Like((p.FirstName + " " + p.LastName).ToLower(), like))
      .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
      .Take(take).ToListAsync();
  }

  /// <inheritdoc/>
  public async Task<Patient> CreatePatientAsync(Patient patient)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    patient.Id = Guid.NewGuid();
    context.Patients.Add(patient);
    await context.SaveChangesAsync();
    return patient;
  }

  /// <inheritdoc/>
  public async Task<Patient> UpdatePatientAsync(Patient patient)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    context.Patients.Update(patient);
    await context.SaveChangesAsync();
    return patient;
  }

  /// <inheritdoc/>
  public async Task DeletePatientAsync(Guid id)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    var patient = await context.Patients.FirstOrDefaultAsync(p => p.Id == id);
    if (patient != null)
    {
      patient.IsDeleted = true;
      await context.SaveChangesAsync();
    }
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<Appointment>> GetAppointmentsAsync()
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    return await context.Appointments.AsNoTracking()
      .Include(a => a.Patient)
      .OrderBy(a => a.ScheduledStart)
      .ToListAsync();
  }

  /// <inheritdoc/>
  public async Task<Appointment?> GetAppointmentByIdAsync(Guid id)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    return await context.Appointments.AsNoTracking()
      .Include(a => a.Patient)
      .FirstOrDefaultAsync(a => a.Id == id);
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    return await context.Appointments.AsNoTracking()
      .Include(a => a.Patient)
      .Where(a => a.PatientId == patientId)
      .OrderBy(a => a.ScheduledStart)
      .ToListAsync();
  }

  /// <inheritdoc/>
  public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    appointment.Id = Guid.NewGuid();
    context.Appointments.Add(appointment);
    await context.SaveChangesAsync();
    return appointment;
  }

  /// <inheritdoc/>
  public async Task<Appointment> UpdateAppointmentAsync(Appointment appointment)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    context.Appointments.Update(appointment);
    await context.SaveChangesAsync();
    return appointment;
  }

  /// <inheritdoc/>
  public async Task DeleteAppointmentAsync(Guid id)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    var appointment = await context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
    if (appointment != null)
    {
      context.Appointments.Remove(appointment);
      await context.SaveChangesAsync();
    }
  }

  /// <inheritdoc/>
  public async Task<Note?> GetNoteByAppointmentIdAsync(Guid appointmentId)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    return await context.Notes.AsNoTracking()
      .Include(n => n.Appointment)
      .FirstOrDefaultAsync(n => n.AppointmentId == appointmentId);
  }

  /// <inheritdoc/>
  public async Task<Note> CreateNoteAsync(Note note)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    note.Id = Guid.NewGuid();
    context.Notes.Add(note);
    await context.SaveChangesAsync();
    return note;
  }

  /// <inheritdoc/>
  public async Task<Note> UpdateNoteAsync(Note note)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    context.Notes.Update(note);
    await context.SaveChangesAsync();
    return note;
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<QuestionnaireDefinition>> GetQuestionnaireDefinitionsAsync()
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    return await context.QuestionnaireDefinitions.AsNoTracking().ToListAsync();
  }

  /// <inheritdoc/>
  public async Task<QuestionnaireDefinition?> GetQuestionnaireDefinitionByIdAsync(Guid id)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    return await context.QuestionnaireDefinitions.AsNoTracking().FirstOrDefaultAsync(q => q.Id == id);
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<QuestionnaireResponse>> GetQuestionnaireResponsesByAppointmentIdAsync(Guid appointmentId)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    return await context.QuestionnaireResponses.AsNoTracking()
      .Where(q => q.AppointmentId == appointmentId)
      .ToListAsync();
  }

  /// <inheritdoc/>
  public async Task<QuestionnaireResponse> CreateQuestionnaireResponseAsync(QuestionnaireResponse response)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    response.Id = Guid.NewGuid();
    context.QuestionnaireResponses.Add(response);
    await context.SaveChangesAsync();
    return response;
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<CptCode>> GetCptCodesAsync()
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    return await context.CptCodes.AsNoTracking().ToListAsync();
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<Icd10Code>> GetIcd10CodesAsync()
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    return await context.Icd10Codes.AsNoTracking().ToListAsync();
  }

  /// <inheritdoc/>
  public async Task<CheckInMessageLog> CreateCheckInMessageLogAsync(CheckInMessageLog log)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    log.Id = Guid.NewGuid();
    context.CheckInMessageLogs.Add(log);
    await context.SaveChangesAsync();
    return log;
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<CheckInMessageLog>> GetPendingCheckInMessagesAsync()
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    return await context.CheckInMessageLogs.AsNoTracking()
      .Where(c => c.SentAt == null)
      .OrderBy(c => c.ScheduledSendAt)
      .ToListAsync();
  }

  /// <inheritdoc/>
  public async Task UpdateCheckInMessageLogAsync(CheckInMessageLog log)
  {
    using var context = await this.contextFactory.CreateDbContextAsync();
    context.CheckInMessageLogs.Update(log);
    await context.SaveChangesAsync();
  }
}
