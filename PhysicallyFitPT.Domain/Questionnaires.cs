// <copyright file="Questionnaires.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

public class QuestionnaireDefinition : Entity
{
  public string Name { get; set; } = null!;

  public QuestionnaireType Type { get; set; }

  public string? BodyRegion { get; set; }

  public int Version { get; set; } = 1;

  public string JsonSchema { get; set; } = "{}";
}

public class QuestionnaireResponse : Entity
{
  public Guid PatientId { get; set; }

  public Guid AppointmentId { get; set; }

  public Guid QuestionnaireDefinitionId { get; set; }

  public DateTimeOffset SubmittedAt { get; set; } = DateTimeOffset.UtcNow;

  public string AnswersJson { get; set; } = "{}";
}
