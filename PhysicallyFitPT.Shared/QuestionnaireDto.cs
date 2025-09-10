// <copyright file="QuestionnaireDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  public class QuestionnaireDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public int Type { get; set; }

        public string? BodyRegion { get; set; }

        public int Version { get; set; }

        public string JsonSchema { get; set; } = "{}";
    }
}
