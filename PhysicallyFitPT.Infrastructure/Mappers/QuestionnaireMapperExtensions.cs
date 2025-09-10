// <copyright file="QuestionnaireMapperExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Mappers
{
  using PhysicallyFitPT.Domain;
  using PhysicallyFitPT.Shared;

  public static class QuestionnaireMapperExtensions
    {
        public static QuestionnaireDto ToDto(this QuestionnaireDefinition def)
        {
            return new QuestionnaireDto
            {
                Id = def.Id,
                Name = def.Name,
                Type = (int)def.Type,
                BodyRegion = def.BodyRegion,
                Version = def.Version,
                JsonSchema = def.JsonSchema,
            };
        }

        public static QuestionnaireResponseDto ToDto(this QuestionnaireResponse response)
        {
            return new QuestionnaireResponseDto
            {
                Id = response.Id,
                PatientId = response.PatientId,
                AppointmentId = response.AppointmentId,
                QuestionnaireDefinitionId = response.QuestionnaireDefinitionId,
                SubmittedAt = response.SubmittedAt,
                AnswersJson = response.AnswersJson,
            };
        }
    }
}
