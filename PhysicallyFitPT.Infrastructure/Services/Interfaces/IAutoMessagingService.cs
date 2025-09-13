// <copyright file="IAutoMessagingService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

using PhysicallyFitPT.Domain;

/// <summary>
/// Interface for automated messaging services including check-in notifications.
/// </summary>
public interface IAutoMessagingService
{
    /// <summary>
    /// Enqueues a check-in message for a patient appointment.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient.</param>
    /// <param name="appointmentId">The unique identifier of the appointment.</param>
    /// <param name="visitType">The type of visit for the appointment.</param>
    /// <param name="questionnaireType">The type of questionnaire to be sent.</param>
    /// <param name="method">The delivery method for the message.</param>
    /// <param name="scheduledSendAtUtc">The scheduled time to send the message in UTC.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<CheckInMessageLog> EnqueueCheckInAsync(Guid patientId, Guid appointmentId, VisitType visitType, QuestionnaireType questionnaireType, DeliveryMethod method, DateTimeOffset scheduledSendAtUtc);

    /// <summary>
    /// Retrieves message logs for a specific patient or all patients.
    /// </summary>
    /// <param name="patientId">Optional patient identifier to filter logs by specific patient.</param>
    /// <param name="take">The maximum number of log entries to return.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<IReadOnlyList<CheckInMessageLog>> GetLogAsync(Guid? patientId = null, int take = 100);
}
