// <copyright file="QuestionnaireMapperExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Mappers
{
    using PhysicallyFitPT.Domain;
    using PhysicallyFitPT.Shared;

    /// <summary>
    /// Extension methods for mapping Questionnaire domain objects to DTOs.
    /// </summary>
    public static class QuestionnaireMapperExtensions
    {
        /// <summary>
        /// Converts a QuestionnaireDefinition domain object to its corresponding DTO.
        /// </summary>
        /// <param name="def">The questionnaire definition domain object to convert.</param>
        /// <returns>A QuestionnaireDto containing the questionnaire definition data.</returns>
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

        /// <summary>
        /// Converts a QuestionnaireResponse domain object to its corresponding DTO.
        /// </summary>
        /// <param name="response">The questionnaire response domain object to convert.</param>
        /// <returns>A QuestionnaireResponseDto containing the questionnaire response data.</returns>
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
