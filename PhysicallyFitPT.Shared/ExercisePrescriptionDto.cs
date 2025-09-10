// <copyright file="ExercisePrescriptionDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  public class ExercisePrescriptionDto
  {
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Dosage { get; set; }

    public string? Notes { get; set; }
  }
}
