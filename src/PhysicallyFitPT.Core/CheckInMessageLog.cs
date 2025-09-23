// <copyright file="CheckInMessageLog.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core;

/// <summary>
/// Represents a log entry for check-in messages sent to patients.
/// </summary>
public class CheckInMessageLog : Entity
{
  /// <summary>
  /// Gets or sets the patient identifier for whom the message is intended.
  /// </summary>
  public Guid PatientId { get; set; }

  /// <summary>
  /// Gets or sets the appointment identifier associated with the message.
  /// </summary>
  public Guid AppointmentId { get; set; }

  /// <summary>
  /// Gets or sets the type of visit for which the message is sent.
  /// </summary>
  public VisitType VisitType { get; set; }

  /// <summary>
  /// Gets or sets the type of questionnaire associated with the message.
  /// </summary>
  public QuestionnaireType QuestionnaireType { get; set; }

  /// <summary>
  /// Gets or sets the delivery method used to send the message (SMS or Email).
  /// </summary>
  public DeliveryMethod Method { get; set; } = DeliveryMethod.SMS;

  /// <summary>
  /// Gets or sets the scheduled time when the message should be sent.
  /// </summary>
  public DateTimeOffset ScheduledSendAt { get; set; }

  /// <summary>
  /// Gets or sets the time when a delivery attempt was made.
  /// </summary>
  public DateTimeOffset? AttemptedAt { get; set; }

  /// <summary>
  /// Gets or sets the time when the message was successfully sent.
  /// </summary>
  public DateTimeOffset? SentAt { get; set; }

  /// <summary>
  /// Gets or sets the current status of the message delivery.
  /// </summary>
  public string Status { get; set; } = "Pending";

  /// <summary>
  /// Gets or sets the reason for delivery failure, if applicable.
  /// </summary>
  public string? FailureReason { get; set; }

  /// <summary>
  /// Gets or sets the unique token hash used for secure questionnaire links.
  /// </summary>
  public string LinkTokenHash { get; set; } = Guid.NewGuid().ToString("N");

  /// <summary>
  /// Gets or sets the time when the questionnaire was completed by the patient.
  /// </summary>
  public DateTimeOffset? QuestionnaireCompletedAt { get; set; }
}
