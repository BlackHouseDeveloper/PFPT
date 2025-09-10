// <copyright file="QuestionnaireResponseDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  public class QuestionnaireResponseDto
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public Guid AppointmentId { get; set; }

        public Guid QuestionnaireDefinitionId { get; set; }

        public DateTimeOffset SubmittedAt { get; set; }

        public string AnswersJson { get; set; } = "{}";
    }
}
