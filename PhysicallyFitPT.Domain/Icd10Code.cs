// <copyright file="Icd10Code.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

/// <summary>
/// Represents an International Classification of Diseases, 10th Revision (ICD-10) diagnostic code.
/// </summary>
public class Icd10Code : Entity
{
  /// <summary>
  /// Gets or sets the ICD-10 diagnostic code value.
  /// </summary>
  public string Code { get; set; } = null!;

  /// <summary>
  /// Gets or sets the description of the diagnosis or condition.
  /// </summary>
  public string Description { get; set; } = null!;
}
