// <copyright file="Patient.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

public sealed class Patient
{
    public Guid Id { get; set; }

    public string? MRN { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public string? Sex { get; set; }

    public string? Email { get; set; }

    public string? MobilePhone { get; set; }

    public string? MedicationsCsv { get; set; }

    public string? ComorbiditiesCsv { get; set; }

    public string? AssistiveDevicesCsv { get; set; }

    public string? LivingSituation { get; set; }

    // Auditing / soft-delete (kept annotation-free)
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public string? CreatedBy { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public List<Appointment> Appointments { get; set; } = new();
}
