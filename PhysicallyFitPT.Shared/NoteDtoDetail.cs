// <copyright file="NoteDtoDetail.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;
  using System.Collections.Generic;

  public class NoteDtoDetail
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public Guid AppointmentId { get; set; }

        public string VisitType { get; set; } = null!;

        public bool IsSigned { get; set; }

        public DateTimeOffset? SignedAt { get; set; }

        public string? SignedBy { get; set; }

        public SubjectiveDto Subjective { get; set; } = new SubjectiveDto();

        public ObjectiveDto Objective { get; set; } = new ObjectiveDto();

        public AssessmentDto Assessment { get; set; } = new AssessmentDto();

        public PlanDto Plan { get; set; } = new PlanDto();
    }

  public class SubjectiveDto
    {
        public string? ChiefComplaint { get; set; }

        public string? HistoryOfPresentIllness { get; set; }

        public string? PainLocationsCsv { get; set; }

        public string? PainSeverity0to10 { get; set; }

        public string? AggravatingFactors { get; set; }

        public string? EasingFactors { get; set; }

        public string? FunctionalLimitations { get; set; }

        public string? PatientGoalsNarrative { get; set; }
    }

  public class ObjectiveDto
    {
        public List<RomMeasureDto> Rom { get; set; } = new();

        public List<MmtMeasureDto> Mmt { get; set; } = new();

        public List<SpecialTestDto> SpecialTests { get; set; } = new();

        public List<OutcomeMeasureScoreDto> OutcomeMeasures { get; set; } = new();

        public List<ProvidedInterventionDto> ProvidedInterventions { get; set; } = new();
    }

  public class AssessmentDto
    {
        public string? ClinicalImpression { get; set; }

        public string? RehabPotential { get; set; }

        public List<Icd10LinkDto> Icd10Codes { get; set; } = new();

        public List<GoalDto> Goals { get; set; } = new();
    }

  public class PlanDto
    {
        public string? Frequency { get; set; }

        public string? Duration { get; set; }

        public string? PlannedInterventionsCsv { get; set; }

        public string? NextVisitFocus { get; set; }

        public List<ExercisePrescriptionDto> Hep { get; set; } = new();
    }
}
