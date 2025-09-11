// <copyright file="Icd10LinkDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  /// <summary>
  /// Represents an ICD-10 diagnosis code data transfer object.
  /// </summary>
  public class Icd10LinkDto
  {
    /// <summary>
    /// Gets or sets the unique identifier for this ICD-10 code link.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the ICD-10 diagnosis code.
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Gets or sets the description of the ICD-10 diagnosis code.
    /// </summary>
    public string? Description { get; set; }
  }
}
