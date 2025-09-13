// <copyright file="IQuestionnaireService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using PhysicallyFitPT.Shared;

    /// <summary>
    /// Interface for questionnaire management services including definitions and responses.
    /// </summary>
    public interface IQuestionnaireService
    {
        /// <summary>
        /// Retrieves all available questionnaire definitions.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IReadOnlyList<QuestionnaireDto>> GetAllDefinitionsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a specific questionnaire definition by its unique identifier.
        /// </summary>
        /// <param name="definitionId">The unique identifier of the questionnaire definition.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<QuestionnaireDto?> GetDefinitionAsync(Guid definitionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the questionnaire response associated with a specific appointment.
        /// </summary>
        /// <param name="appointmentId">The unique identifier of the appointment.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<QuestionnaireResponseDto?> GetResponseForAppointmentAsync(Guid appointmentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Submits a questionnaire response for a patient appointment.
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient.</param>
        /// <param name="appointmentId">The unique identifier of the appointment.</param>
        /// <param name="questionnaireDefinitionId">The unique identifier of the questionnaire definition.</param>
        /// <param name="answersJson">The questionnaire answers in JSON format.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<QuestionnaireResponseDto> SubmitResponseAsync(Guid patientId, Guid appointmentId, Guid questionnaireDefinitionId, string answersJson, CancellationToken cancellationToken = default);
    }
}
