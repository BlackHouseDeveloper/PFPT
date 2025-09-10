// <copyright file="OutcomeMeasureScoreDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  public class OutcomeMeasureScoreDto
    {
        public Guid Id { get; set; }

        public string Instrument { get; set; } = null!;

        public int? RawScore { get; set; }

        public double? Percent { get; set; }

        public DateTime? CollectedOn { get; set; }
    }
}
