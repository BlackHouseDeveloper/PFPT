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
    Task<IReadOnlyList<QuestionnaireDto>> GetAllDefinitionsAsync(CancellationToken cancellationToken = default);

    Task<QuestionnaireDto?> GetDefinitionAsync(Guid definitionId, CancellationToken cancellationToken = default);

    Task<QuestionnaireResponseDto?> GetResponseForAppointmentAsync(Guid appointmentId, CancellationToken cancellationToken = default);

    Task<QuestionnaireResponseDto> SubmitResponseAsync(Guid patientId, Guid appointmentId, Guid questionnaireDefinitionId, string answersJson, CancellationToken cancellationToken = default);
  }

}
