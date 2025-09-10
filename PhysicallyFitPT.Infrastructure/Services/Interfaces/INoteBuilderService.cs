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
  Task<NoteDtoDetail> CreateEvalNoteAsync(Guid patientId, Guid appointmentId, CancellationToken cancellationToken = default);

  Task<NoteDtoDetail> CreateDailyNoteAsync(Guid patientId, Guid appointmentId, CancellationToken cancellationToken = default);

  Task<NoteDtoDetail> CreateProgressNoteAsync(Guid patientId, Guid appointmentId, CancellationToken cancellationToken = default);

  Task<NoteDtoDetail> CreateDischargeNoteAsync(Guid patientId, Guid appointmentId, CancellationToken cancellationToken = default);

  Task<NoteDtoDetail?> GetAsync(Guid noteId, CancellationToken cancellationToken = default);

  Task<bool> UpdateObjectiveAsync(Guid noteId, IEnumerable<RomMeasureDto>? rom = null, IEnumerable<MmtMeasureDto>? mmt = null, IEnumerable<SpecialTestDto>? specialTests = null, IEnumerable<OutcomeMeasureScoreDto>? outcomes = null, IEnumerable<ProvidedInterventionDto>? interventions = null, CancellationToken cancellationToken = default);

  Task<bool> UpdateAssessmentAsync(Guid noteId, string? clinicalImpression = null, string? rehabPotential = null, IEnumerable<Icd10LinkDto>? icd10Codes = null, CancellationToken cancellationToken = default);

  Task<bool> SignAsync(Guid noteId, string signedBy, CancellationToken cancellationToken = default);
}
