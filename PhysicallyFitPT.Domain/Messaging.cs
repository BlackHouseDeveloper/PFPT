// <copyright file="Messaging.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

public class CheckInMessageLog : Entity
{
  public Guid PatientId { get; set; }

  public Guid AppointmentId { get; set; }

  public VisitType VisitType { get; set; }

  public QuestionnaireType QuestionnaireType { get; set; }

  public DeliveryMethod Method { get; set; } = DeliveryMethod.SMS;

  public DateTimeOffset ScheduledSendAt { get; set; }

  public DateTimeOffset? AttemptedAt { get; set; }

  public DateTimeOffset? SentAt { get; set; }

  public string Status { get; set; } = "Pending";

  public string? FailureReason { get; set; }

  public string LinkTokenHash { get; set; } = Guid.NewGuid().ToString("N");

  public DateTimeOffset? QuestionnaireCompletedAt { get; set; }
}
