// <copyright file="QuestionnaireService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Logging;
  using PhysicallyFitPT.Infrastructure.Data;
  using PhysicallyFitPT.Infrastructure.Mappers;
  using PhysicallyFitPT.Infrastructure.Services.Interfaces;
  using PhysicallyFitPT.Shared;

  /// <summary>
  /// Service for managing questionnaire operations including retrieval and submission handling.
  /// </summary>
  public class QuestionnaireService : BaseService, IQuestionnaireService
  {
    private readonly IDbContextFactory<ApplicationDbContext> factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionnaireService"/> class.
    /// </summary>
    /// <param name="factory">Database context factory for data access.</param>
    /// <param name="logger">Logger instance for logging operations.</param>
    public QuestionnaireService(IDbContextFactory<ApplicationDbContext> factory, ILogger<QuestionnaireService> logger)
        : base(logger)
    {
      this.factory = factory;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<QuestionnaireDto>> GetAllDefinitionsAsync(CancellationToken cancellationToken = default)
    {
      try
      {
        using var db = await this.factory.CreateDbContextAsync(cancellationToken);
        var defs = await db.QuestionnaireDefinitions.AsNoTracking().ToListAsync(cancellationToken);
        return defs.Select(d => d.ToDto()).ToList();
      }
      catch (Exception ex)
      {
        this.Logger.LogError(ex, "Error executing GetAllDefinitionsAsync: {ErrorMessage}", ex.Message);
        return Array.Empty<QuestionnaireDto>();
      }
    }

    /// <inheritdoc/>
    public async Task<QuestionnaireDto?> GetDefinitionAsync(Guid definitionId, CancellationToken cancellationToken = default)
    {
      try
      {
        using var db = await this.factory.CreateDbContextAsync(cancellationToken);
        var def = await db.QuestionnaireDefinitions.AsNoTracking().FirstOrDefaultAsync(q => q.Id == definitionId, cancellationToken);
        return def?.ToDto();
      }
      catch (Exception ex)
      {
        this.Logger.LogError(ex, "Error executing GetDefinitionAsync: {ErrorMessage}", ex.Message);
        return null;
      }
    }

    /// <inheritdoc/>
    public async Task<QuestionnaireResponseDto?> GetResponseForAppointmentAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
      try
      {
        using var db = await this.factory.CreateDbContextAsync(cancellationToken);
        var response = await db.QuestionnaireResponses.AsNoTracking()
            .Where(r => r.AppointmentId == appointmentId)
            .OrderByDescending(r => r.SubmittedAt)
            .FirstOrDefaultAsync(cancellationToken);
        return response?.ToDto();
      }
      catch (Exception ex)
      {
        this.Logger.LogError(ex, "Error executing GetResponseForAppointmentAsync: {ErrorMessage}", ex.Message);
        return null;
      }
    }

    /// <inheritdoc/>
    public async Task<QuestionnaireResponseDto> SubmitResponseAsync(Guid patientId, Guid appointmentId, Guid questionnaireDefinitionId, string answersJson, CancellationToken cancellationToken = default)
    {
      try
      {
        using var db = await this.factory.CreateDbContextAsync(cancellationToken);

        // Ensure related entities exist
        bool patientExists = await db.Patients.AnyAsync(p => p.Id == patientId, cancellationToken);
        bool appointmentExists = await db.Appointments.AnyAsync(a => a.Id == appointmentId, cancellationToken);
        bool defExists = await db.QuestionnaireDefinitions.AnyAsync(q => q.Id == questionnaireDefinitionId, cancellationToken);
        if (!patientExists)
        {
          throw new ArgumentException("Patient not found", nameof(patientId));
        }

        if (!appointmentExists)
        {
          throw new ArgumentException("Appointment not found", nameof(appointmentId));
        }

        if (!defExists)
        {
          throw new ArgumentException("Questionnaire definition not found", nameof(questionnaireDefinitionId));
        }

        // Create and save new response
        var response = new Domain.QuestionnaireResponse
        {
          PatientId = patientId,
          AppointmentId = appointmentId,
          QuestionnaireDefinitionId = questionnaireDefinitionId,
          SubmittedAt = DateTimeOffset.UtcNow,
          AnswersJson = answersJson,
        };
        db.QuestionnaireResponses.Add(response);

        // Update appointment check-in status if exists
        var appt = await db.Appointments.FindAsync(new object?[] { appointmentId }, cancellationToken);
        if (appt != null)
        {
          appt.QuestionnaireCompletedAt = response.SubmittedAt;
          appt.IsCheckedIn = true;
        }

        await db.SaveChangesAsync(cancellationToken);
        return response.ToDto();
      }
      catch (Exception ex)
      {
        this.Logger.LogError(ex, "Error executing SubmitResponseAsync: {ErrorMessage}", ex.Message);
        throw;
      }
    }
  }
}
