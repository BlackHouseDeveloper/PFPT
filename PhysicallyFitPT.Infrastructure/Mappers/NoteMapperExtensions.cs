// <copyright file="NoteMapperExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Mappers
{
    using System.Linq;
    using PhysicallyFitPT.Domain;
    using PhysicallyFitPT.Domain.Notes;
    using PhysicallyFitPT.Shared;

    /// <summary>
    /// Extension methods for mapping Note domain objects to DTOs.
    /// </summary>
    public static class NoteMapperExtensions
    {
        /// <summary>
        /// Converts a Note domain object to its corresponding summary DTO.
        /// </summary>
        /// <param name="note">The note domain object to convert.</param>
        /// <returns>A NoteDtoSummary containing the note summary data.</returns>
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

        /// <summary>
        /// Converts a Note domain object to its corresponding detail DTO.
        /// </summary>
        /// <param name="note">The note domain object to convert.</param>
        /// <returns>A NoteDtoDetail containing the comprehensive note data.</returns>
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

        /// <summary>
        /// Converts a ROM measure domain object to its corresponding DTO.
        /// </summary>
        /// <param name="rom">The ROM measure domain object to convert.</param>
        /// <returns>A RomMeasureDto containing the ROM measure data.</returns>
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

        /// <summary>
        /// Converts an MMT measure domain object to its corresponding DTO.
        /// </summary>
        /// <param name="mmt">The MMT measure domain object to convert.</param>
        /// <returns>An MmtMeasureDto containing the MMT measure data.</returns>
        public static MmtMeasureDto ToDto(this MmtMeasure mmt) => new MmtMeasureDto
        {
            Id = mmt.Id,
            MuscleGroup = mmt.MuscleGroup,
            Side = (int)mmt.Side,
            Grade = mmt.Grade,
            WithPain = mmt.WithPain,
            Notes = mmt.Notes,
        };

        /// <summary>
        /// Converts a special test domain object to its corresponding DTO.
        /// </summary>
        /// <param name="st">The special test domain object to convert.</param>
        /// <returns>A SpecialTestDto containing the special test data.</returns>
        public static SpecialTestDto ToDto(this SpecialTest st) => new SpecialTestDto
        {
            Id = st.Id,
            Name = st.Name,
            Side = (int)st.Side,
            Result = (int)st.Result,
            Notes = st.Notes,
        };

        /// <summary>
        /// Converts an outcome measure score domain object to its corresponding DTO.
        /// </summary>
        /// <param name="om">The outcome measure score domain object to convert.</param>
        /// <returns>An OutcomeMeasureScoreDto containing the outcome measure score data.</returns>
        public static OutcomeMeasureScoreDto ToDto(this OutcomeMeasureScore om) => new OutcomeMeasureScoreDto
        {
            Id = om.Id,
            Instrument = om.Instrument,
            RawScore = om.RawScore,
            Percent = om.Percent,
            CollectedOn = om.CollectedOn,
        };

        /// <summary>
        /// Converts a provided intervention domain object to its corresponding DTO.
        /// </summary>
        /// <param name="pi">The provided intervention domain object to convert.</param>
        /// <returns>A ProvidedInterventionDto containing the provided intervention data.</returns>
        public static ProvidedInterventionDto ToDto(this ProvidedIntervention pi) => new ProvidedInterventionDto
        {
            Id = pi.Id,
            CptCode = pi.CptCode,
            Description = pi.Description,
            Units = pi.Units,
            Minutes = pi.Minutes,
        };

        /// <summary>
        /// Converts an ICD-10 link domain object to its corresponding DTO.
        /// </summary>
        /// <param name="link">The ICD-10 link domain object to convert.</param>
        /// <returns>An Icd10LinkDto containing the ICD-10 link data.</returns>
        public static Icd10LinkDto ToDto(this Icd10Link link) => new Icd10LinkDto
        {
            Id = link.Id,
            Code = link.Code,
            Description = link.Description,
        };

        /// <summary>
        /// Converts a goal domain object to its corresponding DTO.
        /// </summary>
        /// <param name="goal">The goal domain object to convert.</param>
        /// <returns>A GoalDto containing the goal data.</returns>
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

        /// <summary>
        /// Converts an exercise prescription domain object to its corresponding DTO.
        /// </summary>
        /// <param name="ex">The exercise prescription domain object to convert.</param>
        /// <returns>An ExercisePrescriptionDto containing the exercise prescription data.</returns>
        public static ExercisePrescriptionDto ToDto(this ExercisePrescription ex) => new ExercisePrescriptionDto
        {
            Id = ex.Id,
            Name = ex.Name,
            Dosage = ex.Dosage,
            Notes = ex.Notes,
        };
    }
}
