// <copyright file="PatientDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  public class PatientDto
  {
    public Guid Id { get; set; }

    public string? MRN { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }

    public string? Sex { get; set; }

    public string? Email { get; set; }

    public string? MobilePhone { get; set; }

    public string? MedicationsCsv { get; set; }

    public string? ComorbiditiesCsv { get; set; }

    public string? AssistiveDevicesCsv { get; set; }

    public string? LivingSituation { get; set; }
  }
}
