// <copyright file="NoteDtoDetail.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

using System;

/// <summary>
/// Data transfer object representing a detailed clinical note with all SOAP sections.
/// </summary>
public class NoteDtoDetail
{
    /// <summary>
    /// Gets or sets the unique identifier of the note.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the patient.
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the appointment.
    /// </summary>
    public Guid AppointmentId { get; set; }

    /// <summary>
    /// Gets or sets the type of visit.
    /// </summary>
    public string VisitType { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether the note is signed.
    /// </summary>
    public bool IsSigned { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the note was signed.
    /// </summary>
    public DateTimeOffset? SignedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the person who signed the note.
    /// </summary>
    public string? SignedBy { get; set; }

    /// <summary>
    /// Gets or sets the subjective portion of the note.
    /// </summary>
    public SubjectiveDto Subjective { get; set; } = new SubjectiveDto();

    /// <summary>
    /// Gets or sets the objective portion of the note.
    /// </summary>
    public ObjectiveDto Objective { get; set; } = new ObjectiveDto();

    /// <summary>
    /// Gets or sets the assessment portion of the note.
    /// </summary>
    public AssessmentDto Assessment { get; set; } = new AssessmentDto();

    /// <summary>
    /// Gets or sets the plan portion of the note.
    /// </summary>
    public PlanDto Plan { get; set; } = new PlanDto();
}
