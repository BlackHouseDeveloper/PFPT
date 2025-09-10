// <copyright file="IAutoMessagingService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

using PhysicallyFitPT.Domain;

public interface IAutoMessagingService
{
  Task<CheckInMessageLog> EnqueueCheckInAsync(Guid patientId, Guid appointmentId, VisitType visitType, QuestionnaireType questionnaireType, DeliveryMethod method, DateTimeOffset scheduledSendAtUtc);

  Task<IReadOnlyList<CheckInMessageLog>> GetLogAsync(Guid? patientId = null, int take = 100);
}
