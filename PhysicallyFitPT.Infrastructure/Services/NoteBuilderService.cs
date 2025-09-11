// <copyright file="NoteBuilderService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Logging;
  using PhysicallyFitPT.Domain;
  using PhysicallyFitPT.Domain.Notes;
  using PhysicallyFitPT.Infrastructure.Data;
  using PhysicallyFitPT.Infrastructure.Mappers;
  using PhysicallyFitPT.Infrastructure.Services.Interfaces;
  using PhysicallyFitPT.Shared;

  /// <summary>
  /// Service for building and managing clinical notes including creation, updating, and signing.
  /// </summary>
  public class NoteBuilderService : BaseService, INoteBuilderService
  {
    private readonly IDbContextFactory<ApplicationDbContext> factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="NoteBuilderService"/> class.
    /// </summary>
    /// <param name="factory">Database context factory for data access.</param>
    /// <param name="logger">Logger instance for logging operations.</param>
    public NoteBuilderService(IDbContextFactory<ApplicationDbContext> factory, ILogger<NoteBuilderService> logger)
        : base(logger)
    {
      this.factory = factory;
    }

    /// <inheritdoc/>
    public async Task<NoteDtoDetail> CreateEvalNoteAsync(Guid patientId, Guid appointmentId, CancellationToken cancellationToken = default)
    {
      try
      {
        using var db = await this.factory.CreateDbContextAsync(cancellationToken);

        // Create a new evaluation note (VisitType.Eval) for given patient and appointment
        var note = new Note
        {
          PatientId = patientId,
          AppointmentId = appointmentId,
          VisitType = VisitType.Eval,
          Subjective = new SubjectiveSection(),
          Objective = new ObjectiveSection(),
          Assessment = new AssessmentSection(),
          Plan = new PlanSection(),
        };
        db.Notes.Add(note);
        await db.SaveChangesAsync(cancellationToken);
        return note.ToDetailDto();
      }
      catch (Exception ex)
      {
        this.Logger.LogError(ex, "Error executing CreateEvalNoteAsync: {ErrorMessage}", ex.Message);
        throw;
      }
    }

    /// <inheritdoc/>
    public async Task<NoteDtoDetail?> GetAsync(Guid noteId, CancellationToken cancellationToken = default)
    {
      try
      {
        using var db = await this.factory.CreateDbContextAsync(cancellationToken);

        // Include related data for full detail (sections and their collections)
        var note = await db.Notes
            .AsNoTracking()
            .Include(n => n.Appointment)
            .Include(n => n.Patient)
            .Include(n => n.Objective).ThenInclude(o => o.Rom)
            .Include(n => n.Objective).ThenInclude(o => o.Mmt)
            .Include(n => n.Objective).ThenInclude(o => o.SpecialTests)
            .Include(n => n.Objective).ThenInclude(o => o.OutcomeMeasures)
            .Include(n => n.Objective).ThenInclude(o => o.ProvidedInterventions)
            .Include(n => n.Assessment).ThenInclude(a => a.Icd10Codes)
            .Include(n => n.Assessment).ThenInclude(a => a.Goals)
            .Include(n => n.Plan).ThenInclude(p => p.Hep)
            .FirstOrDefaultAsync(n => n.Id == noteId, cancellationToken);
        if (note == null)
        {
          return null;
        }

        return note.ToDetailDto();
      }
      catch (Exception ex)
      {
        this.Logger.LogError(ex, "Error executing GetAsync: {ErrorMessage}", ex.Message);
        return null;
      }
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateObjectiveAsync(Guid noteId, IEnumerable<RomMeasureDto>? rom = null, IEnumerable<MmtMeasureDto>? mmt = null, IEnumerable<SpecialTestDto>? specialTests = null, IEnumerable<OutcomeMeasureScoreDto>? outcomes = null, IEnumerable<ProvidedInterventionDto>? interventions = null, CancellationToken cancellationToken = default)
    {
      try
      {
        using var db = await this.factory.CreateDbContextAsync(cancellationToken);
        var note = await db.Notes
            .Include(n => n.Objective).ThenInclude(o => o.Rom)
            .Include(n => n.Objective).ThenInclude(o => o.Mmt)
            .Include(n => n.Objective).ThenInclude(o => o.SpecialTests)
            .Include(n => n.Objective).ThenInclude(o => o.OutcomeMeasures)
            .Include(n => n.Objective).ThenInclude(o => o.ProvidedInterventions)
            .FirstOrDefaultAsync(n => n.Id == noteId, cancellationToken);
        if (note == null)
        {
          return false;
        }

        // local helpers to convert validated ints- enums
        static Side ToSide(int value)
        {
          if (!Enum.IsDefined(typeof(Side), value))
          {
            throw new ArgumentOutOfRangeException(nameof(value), $"Invalid Side value: {value}");
          }

          return (Side)value;
        }

        static SpecialTestResult ToSpecialTestResult(int value)
        {
          if (!Enum.IsDefined(typeof(SpecialTestResult), value))
          {
            throw new ArgumentOutOfRangeException(nameof(value), $"Invalid SpecialTestResult value: {value}");
          }

          return (SpecialTestResult)value;
        }

        // Update ROM measures in place
        if (rom != null)
        {
          var romList = note.Objective.Rom;
          var incomingRom = rom.ToList();

          // Update existing or add new
          foreach (var dto in incomingRom)
          {
            var existing = romList.FirstOrDefault(x => x.Id == dto.Id);
            if (existing != null)
            {
              // Update fields of existing
              existing.Joint = dto.Joint;
              existing.Movement = dto.Movement;
              existing.Side = ToSide(dto.Side);
              existing.MeasuredDegrees = dto.MeasuredDegrees;
              existing.NormalDegrees = dto.NormalDegrees;
              existing.WithPain = dto.WithPain;
              existing.Notes = dto.Notes;
            }
            else
            {
              // Add new measure
              var newMeasure = new RomMeasure
              {
                Joint = dto.Joint,
                Movement = dto.Movement,
                Side = ToSide(dto.Side),
                MeasuredDegrees = dto.MeasuredDegrees,
                NormalDegrees = dto.NormalDegrees,
                WithPain = dto.WithPain,
                Notes = dto.Notes,
              };

              // If the DTO provided an Id (not Guid.Empty), use it to maintain identity
              if (dto.Id != Guid.Empty)
              {
                newMeasure.Id = dto.Id;
              }

              romList.Add(newMeasure);
            }
          }

          // Remove any measures that were not re-provided (deleted by user)
          var romIdsIncoming = incomingRom.Select(d => d.Id).ToHashSet();
          note.Objective.Rom.RemoveAll(x => !romIdsIncoming.Contains(x.Id));
        }

        // Update MMT measures in place
        if (mmt != null)
        {
          var mmtList = note.Objective.Mmt;
          var incomingMmt = mmt.ToList();
          foreach (var dto in incomingMmt)
          {
            var existing = mmtList.FirstOrDefault(x => x.Id == dto.Id);
            if (existing != null)
            {
              existing.MuscleGroup = dto.MuscleGroup;
              existing.Side = ToSide(dto.Side);
              existing.Grade = dto.Grade;
              existing.WithPain = dto.WithPain;
              existing.Notes = dto.Notes;
            }
            else
            {
              var newMeasure = new MmtMeasure
              {
                MuscleGroup = dto.MuscleGroup,
                Side = ToSide(dto.Side),
                Grade = dto.Grade,
                WithPain = dto.WithPain,
                Notes = dto.Notes,
              };
              if (dto.Id != Guid.Empty)
              {
                newMeasure.Id = dto.Id;
              }

              mmtList.Add(newMeasure);
            }
          }

          var mmtIdsIncoming = incomingMmt.Select(d => d.Id).ToHashSet();
          note.Objective.Mmt.RemoveAll(x => !mmtIdsIncoming.Contains(x.Id));
        }

        // Update Special Tests in place
        if (specialTests != null)
        {
          var stList = note.Objective.SpecialTests;
          var incomingSt = specialTests.ToList();
          foreach (var dto in incomingSt)
          {
            var existing = stList.FirstOrDefault(x => x.Id == dto.Id);
            if (existing != null)
            {
              existing.Name = dto.Name;
              existing.Side = ToSide(dto.Side);
              existing.Result = ToSpecialTestResult(dto.Result);
              existing.Notes = dto.Notes;
            }
            else
            {
              var newTest = new SpecialTest
              {
                Name = dto.Name,
                Side = ToSide(dto.Side),
                Result = ToSpecialTestResult(dto.Result),
                Notes = dto.Notes,
              };
              if (dto.Id != Guid.Empty)
              {
                newTest.Id = dto.Id;
              }

              stList.Add(newTest);
            }
          }

          var stIdsIncoming = incomingSt.Select(d => d.Id).ToHashSet();
          note.Objective.SpecialTests.RemoveAll(x => !stIdsIncoming.Contains(x.Id));
        }

        // Update Outcome Measures in place
        if (outcomes != null)
        {
          var omList = note.Objective.OutcomeMeasures;
          var incomingOm = outcomes.ToList();
          foreach (var dto in incomingOm)
          {
            var existing = omList.FirstOrDefault(x => x.Id == dto.Id);
            if (existing != null)
            {
              existing.Instrument = dto.Instrument;
              existing.RawScore = dto.RawScore;
              existing.Percent = dto.Percent;
              existing.CollectedOn = dto.CollectedOn;
            }
            else
            {
              var newOm = new OutcomeMeasureScore
              {
                Instrument = dto.Instrument,
                RawScore = dto.RawScore,
                Percent = dto.Percent,
                CollectedOn = dto.CollectedOn,
              };
              if (dto.Id != Guid.Empty)
              {
                newOm.Id = dto.Id;
              }

              omList.Add(newOm);
            }
          }

          var omIdsIncoming = incomingOm.Select(d => d.Id).ToHashSet();
          note.Objective.OutcomeMeasures.RemoveAll(x => !omIdsIncoming.Contains(x.Id));
        }

        // Update Provided Interventions in place
        if (interventions != null)
        {
          var piList = note.Objective.ProvidedInterventions;
          var incomingPi = interventions.ToList();
          foreach (var dto in incomingPi)
          {
            var existing = piList.FirstOrDefault(x => x.Id == dto.Id);
            if (existing != null)
            {
              existing.CptCode = dto.CptCode;
              existing.Description = dto.Description;
              existing.Units = dto.Units;
              existing.Minutes = dto.Minutes;
            }
            else
            {
              var newPi = new ProvidedIntervention
              {
                CptCode = dto.CptCode,
                Description = dto.Description,
                Units = dto.Units,
                Minutes = dto.Minutes,
              };
              if (dto.Id != Guid.Empty)
              {
                newPi.Id = dto.Id;
              }

              piList.Add(newPi);
            }
          }

          var piIdsIncoming = incomingPi.Select(d => d.Id).ToHashSet();
          note.Objective.ProvidedInterventions.RemoveAll(x => !piIdsIncoming.Contains(x.Id));
        }

        await db.SaveChangesAsync(cancellationToken);
        return true;
      }
      catch (Exception ex)
      {
        this.Logger.LogError(ex, "Error executing UpdateObjectiveAsync: {ErrorMessage}", ex.Message);
        return false;
      }
    }

    /// <inheritdoc/>
    public async Task<bool> SignAsync(Guid noteId, string signedBy, CancellationToken cancellationToken = default)
    {
      try
      {
        using var db = await this.factory.CreateDbContextAsync(cancellationToken);
        var note = await db.Notes.FindAsync(new object?[] { noteId }, cancellationToken);
        if (note == null)
        {
          return false;
        }

        note.IsSigned = true;
        note.SignedBy = signedBy;
        note.SignedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return true;
      }
      catch (Exception ex)
      {
        this.Logger.LogError(ex, "Error executing SignAsync: {ErrorMessage}", ex.Message);
        return false;
      }
    }
  }
}
