// <copyright file="Icd10Code.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

public class Icd10Code : Entity
{
  public string Code { get; set; } = null!;

  public string Description { get; set; } = null!;
}