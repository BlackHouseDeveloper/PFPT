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

public interface INoteBuilderService
{
  /// <summary>
  ///
  /// </summary>
  /// <param name="patientId"></param>
  /// <param name="appointmentId"></param>
  /// <param name="cancellationToken"></param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<NoteDtoDetail> CreateEvalNoteAsync(Guid patientId, Guid appointmentId, CancellationToken cancellationToken = default);

  /// <summary>
  ///
  /// </summary>
  /// <param name="noteId"></param>
  /// <param name="cancellationToken"></param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<NoteDtoDetail?> GetAsync(Guid noteId, CancellationToken cancellationToken = default);

  /// <summary>
  ///
  /// </summary>
  /// <param name="noteId"></param>
  /// <param name="rom"></param>
  /// <param name="mmt"></param>
  /// <param name="specialTests"></param>
  /// <param name="outcomes"></param>
  /// <param name="interventions"></param>
  /// <param name="cancellationToken"></param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<bool> UpdateObjectiveAsync(Guid noteId, IEnumerable<RomMeasureDto>? rom = null, IEnumerable<MmtMeasureDto>? mmt = null, IEnumerable<SpecialTestDto>? specialTests = null, IEnumerable<OutcomeMeasureScoreDto>? outcomes = null, IEnumerable<ProvidedInterventionDto>? interventions = null, CancellationToken cancellationToken = default);

  /// <summary>
  ///
  /// </summary>
  /// <param name="noteId"></param>
  /// <param name="signedBy"></param>
  /// <param name="cancellationToken"></param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<bool> SignAsync(Guid noteId, string signedBy, CancellationToken cancellationToken = default);
}
