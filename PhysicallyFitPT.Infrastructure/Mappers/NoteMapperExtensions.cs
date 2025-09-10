// <copyright file="NoteMapperExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Mappers
{
  using System.Linq;
  using PhysicallyFitPT.Domain;
  using PhysicallyFitPT.Shared;

  public static class NoteMapperExtensions
    {
        public static NoteDtoSummary ToSummaryDto(this Note note)
        {
            return new NoteDtoSummary
            {
                Id = note.Id,
                VisitType = note.VisitType.ToString(),
                IsSigned = note.IsSigned,
                SignedAt = note.SignedAt,
                SignedBy = note.SignedBy,

                // Use appointment scheduled date if available, else note creation date
                Date = note.Appointment?.ScheduledStart ?? note.CreatedAt,
            };
        }

        public static NoteDtoDetail ToDetailDto(this Note note)
        {
            var dto = new NoteDtoDetail
            {
                Id = note.Id,
                PatientId = note.PatientId,
                AppointmentId = note.AppointmentId,
                VisitType = note.VisitType.ToString(),
                IsSigned = note.IsSigned,
                SignedAt = note.SignedAt,
                SignedBy = note.SignedBy,
            };

            // Map Subjective
            dto.Subjective = new SubjectiveDto
            {
                ChiefComplaint = note.Subjective.ChiefComplaint,
                HistoryOfPresentIllness = note.Subjective.HistoryOfPresentIllness,
                PainLocationsCsv = note.Subjective.PainLocationsCsv,
                PainSeverity0to10 = note.Subjective.PainSeverity0to10,
                AggravatingFactors = note.Subjective.AggravatingFactors,
                EasingFactors = note.Subjective.EasingFactors,
                FunctionalLimitations = note.Subjective.FunctionalLimitations,
                PatientGoalsNarrative = note.Subjective.PatientGoalsNarrative,
            };

            // Map Objective and its collections
            dto.Objective = new ObjectiveDto
            {
                Rom = note.Objective.Rom.Select(r => r.ToDto()).ToList(),
                Mmt = note.Objective.Mmt.Select(m => m.ToDto()).ToList(),
                SpecialTests = note.Objective.SpecialTests.Select(s => s.ToDto()).ToList(),
                OutcomeMeasures = note.Objective.OutcomeMeasures.Select(o => o.ToDto()).ToList(),
                ProvidedInterventions = note.Objective.ProvidedInterventions.Select(pi => pi.ToDto()).ToList(),
            };

            // Map Assessment and its collections
            dto.Assessment = new AssessmentDto
            {
                ClinicalImpression = note.Assessment.ClinicalImpression,
                RehabPotential = note.Assessment.RehabPotential,
                Icd10Codes = note.Assessment.Icd10Codes.Select(i => i.ToDto()).ToList(),
                Goals = note.Assessment.Goals.Select(g => g.ToDto()).ToList(),
            };

            // Map Plan and HEP
            dto.Plan = new PlanDto
            {
                Frequency = note.Plan.Frequency,
                Duration = note.Plan.Duration,
                PlannedInterventionsCsv = note.Plan.PlannedInterventionsCsv,
                NextVisitFocus = note.Plan.NextVisitFocus,
                Hep = note.Plan.Hep.Select(h => h.ToDto()).ToList(),
            };
            return dto;
        }

        // Extension mappers for nested measure types:
        public static RomMeasureDto ToDto(this RomMeasure rom) => new RomMeasureDto
        {
            Id = rom.Id,
            Joint = rom.Joint,
            Movement = rom.Movement,
            Side = (int)rom.Side,
            MeasuredDegrees = rom.MeasuredDegrees,
            NormalDegrees = rom.NormalDegrees,
            WithPain = rom.WithPain,
            Notes = rom.Notes,
        };

        public static MmtMeasureDto ToDto(this MmtMeasure mmt) => new MmtMeasureDto
        {
            Id = mmt.Id,
            MuscleGroup = mmt.MuscleGroup,
            Side = (int)mmt.Side,
            Grade = mmt.Grade,
            WithPain = mmt.WithPain,
            Notes = mmt.Notes,
        };

        public static SpecialTestDto ToDto(this SpecialTest st) => new SpecialTestDto
        {
            Id = st.Id,
            Name = st.Name,
            Side = (int)st.Side,
            Result = (int)st.Result,
            Notes = st.Notes,
        };

        public static OutcomeMeasureScoreDto ToDto(this OutcomeMeasureScore om) => new OutcomeMeasureScoreDto
        {
            Id = om.Id,
            Instrument = om.Instrument,
            RawScore = om.RawScore,
            Percent = om.Percent,
            CollectedOn = om.CollectedOn,
        };

        public static ProvidedInterventionDto ToDto(this ProvidedIntervention pi) => new ProvidedInterventionDto
        {
            Id = pi.Id,
            CptCode = pi.CptCode,
            Description = pi.Description,
            Units = pi.Units,
            Minutes = pi.Minutes,
        };

        public static Icd10LinkDto ToDto(this Icd10Link link) => new Icd10LinkDto
        {
            Id = link.Id,
            Code = link.Code,
            Description = link.Description,
        };

        public static GoalDto ToDto(this Goal goal) => new GoalDto
        {
            Id = goal.Id,
            IsLongTerm = goal.IsLongTerm,
            Description = goal.Description,
            MeasureType = goal.MeasureType,
            BaselineValue = goal.BaselineValue,
            TargetValue = goal.TargetValue,
            TargetDate = goal.TargetDate,
            Status = goal.Status.ToString(),
        };

        public static ExercisePrescriptionDto ToDto(this ExercisePrescription ex) => new ExercisePrescriptionDto
        {
            Id = ex.Id,
            Name = ex.Name,
            Dosage = ex.Dosage,
            Notes = ex.Notes,
        };
    }
}
