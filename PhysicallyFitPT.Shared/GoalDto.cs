// <copyright file="GoalDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  public class GoalDto
    {
        public Guid Id { get; set; }

        public bool IsLongTerm { get; set; }

        public string Description { get; set; } = null!;

        public string? MeasureType { get; set; }

        public string? BaselineValue { get; set; }

        public string? TargetValue { get; set; }

        public DateTime? TargetDate { get; set; }

        public string Status { get; set; } = null!;
    }
}
