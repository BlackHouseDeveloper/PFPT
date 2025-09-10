// <copyright file="RomMeasureDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  public class RomMeasureDto
    {
        public Guid Id { get; set; }

        public string Joint { get; set; } = null!;

        public string Movement { get; set; } = null!;

        public int Side { get; set; }

        public int? MeasuredDegrees { get; set; }

        public int? NormalDegrees { get; set; }

        public bool WithPain { get; set; }

        public string? Notes { get; set; }
    }
}
