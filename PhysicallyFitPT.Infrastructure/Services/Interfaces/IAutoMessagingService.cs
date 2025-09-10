// <copyright file="IAutoMessagingService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

using PhysicallyFitPT.Domain;

public interface IAutoMessagingService
{
  /// <summary>
  ///
  /// </summary>
  /// <param name="patientId"></param>
  /// <param name="appointmentId"></param>
  /// <param name="visitType"></param>
  /// <param name="questionnaireType"></param>
  /// <param name="method"></param>
  /// <param name="scheduledSendAtUtc"></param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<CheckInMessageLog> EnqueueCheckInAsync(Guid patientId, Guid appointmentId, VisitType visitType, QuestionnaireType questionnaireType, DeliveryMethod method, DateTimeOffset scheduledSendAtUtc);

  /// <summary>
  ///
  /// </summary>
  /// <param name="patientId"></param>
  /// <param name="take"></param>
  /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
  Task<IReadOnlyList<CheckInMessageLog>> GetLogAsync(Guid? patientId = null, int take = 100);
}
