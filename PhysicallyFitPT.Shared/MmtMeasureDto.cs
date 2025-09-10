// <copyright file="MmtMeasureDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  public class MmtMeasureDto
    {
        public Guid Id { get; set; }

        public string MuscleGroup { get; set; } = null!;

        public int Side { get; set; }

        public string Grade { get; set; } = null!;

        public bool WithPain { get; set; }

        public string? Notes { get; set; }
    }
}
