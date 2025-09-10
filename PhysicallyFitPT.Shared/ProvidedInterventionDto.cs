// <copyright file="ProvidedInterventionDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  public class ProvidedInterventionDto
    {
        public Guid Id { get; set; }

        public string CptCode { get; set; } = null!;

        public string? Description { get; set; }

        public int Units { get; set; }

        public int? Minutes { get; set; }
    }
}
