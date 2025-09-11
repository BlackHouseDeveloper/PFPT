// <copyright file="CptCode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

/// <summary>
/// Represents a Current Procedural Terminology (CPT) code used in healthcare billing.
/// </summary>
public class CptCode : Entity
{
  /// <summary>
  /// Gets or sets the CPT code value.
  /// </summary>
  public string Code { get; set; } = null!;

  /// <summary>
  /// Gets or sets the description of the procedure or service.
  /// </summary>
  public string Description { get; set; } = null!;
}
