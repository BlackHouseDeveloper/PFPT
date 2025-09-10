// <copyright file="SpecialTestDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  public class SpecialTestDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public int Side { get; set; }

        public int Result { get; set; }

        public string? Notes { get; set; }
    }
}
