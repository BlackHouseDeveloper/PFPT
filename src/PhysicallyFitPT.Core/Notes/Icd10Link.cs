// <copyright file="Icd10Link.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain.Notes;

/// <summary>
/// Represents an ICD-10 diagnosis code link.
/// </summary>
public sealed class Icd10Link
{
  /// <summary>
  /// Gets or sets the unique identifier for this ICD-10 code link.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the ICD-10 diagnosis code.
  /// </summary>
  public string Code { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the description of the ICD-10 diagnosis code.
  /// </summary>
  public string? Description { get; set; }
}
