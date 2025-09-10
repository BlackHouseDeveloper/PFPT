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

  public interface IQuestionnaireService
  {
    /// <summary>
    ///
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<IReadOnlyList<QuestionnaireDto>> GetAllDefinitionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///
    /// </summary>
    /// <param name="definitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<QuestionnaireDto?> GetDefinitionAsync(Guid definitionId, CancellationToken cancellationToken = default);

    /// <summary>
    ///
    /// </summary>
    /// <param name="appointmentId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<QuestionnaireResponseDto?> GetResponseForAppointmentAsync(Guid appointmentId, CancellationToken cancellationToken = default);

    /// <summary>
    ///
    /// </summary>
    /// <param name="patientId"></param>
    /// <param name="appointmentId"></param>
    /// <param name="questionnaireDefinitionId"></param>
    /// <param name="answersJson"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<QuestionnaireResponseDto> SubmitResponseAsync(Guid patientId, Guid appointmentId, Guid questionnaireDefinitionId, string answersJson, CancellationToken cancellationToken = default);
  }
}
