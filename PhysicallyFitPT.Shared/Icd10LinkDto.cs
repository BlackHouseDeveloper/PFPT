// <copyright file="Icd10LinkDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  public class Icd10LinkDto
  {
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }
  }
}
