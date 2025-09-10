// <copyright file="INoteBuilderService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Shared;

/// <summary>
/// Interface for note building services including creation, retrieval, updating, and signing.
/// </summary>
public interface INoteBuilderService
{
  /// <summary>
  /// Creates a new evaluation note for a patient appointment.
  /// </summary>
  /// <param name="patientId">The unique identifier of the patient.</param>
  /// <param name="appointmentId">The unique identifier of the appointment.</param>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<NoteDtoDetail> CreateEvalNoteAsync(Guid patientId, Guid appointmentId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Retrieves a note by its unique identifier.
  /// </summary>
  /// <param name="noteId">The unique identifier of the note.</param>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<NoteDtoDetail?> GetAsync(Guid noteId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Updates the objective section of a clinical note with various measurement data.
  /// </summary>
  /// <param name="noteId">The unique identifier of the note to update.</param>
  /// <param name="rom">Range of motion measurements to include.</param>
  /// <param name="mmt">Manual muscle test measurements to include.</param>
  /// <param name="specialTests">Special test results to include.</param>
  /// <param name="outcomes">Outcome measure scores to include.</param>
  /// <param name="interventions">Provided interventions to include.</param>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<bool> UpdateObjectiveAsync(Guid noteId, IEnumerable<RomMeasureDto>? rom = null, IEnumerable<MmtMeasureDto>? mmt = null, IEnumerable<SpecialTestDto>? specialTests = null, IEnumerable<OutcomeMeasureScoreDto>? outcomes = null, IEnumerable<ProvidedInterventionDto>? interventions = null, CancellationToken cancellationToken = default);

  /// <summary>
  /// Signs a clinical note with the specified user identifier.
  /// </summary>
  /// <param name="noteId">The unique identifier of the note to sign.</param>
  /// <param name="signedBy">The identifier of the person signing the note.</param>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<bool> SignAsync(Guid noteId, string signedBy, CancellationToken cancellationToken = default);
}
