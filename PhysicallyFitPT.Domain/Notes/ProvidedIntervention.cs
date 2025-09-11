// <copyright file="ProvidedIntervention.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain.Notes;

/// <summary>
/// Represents an intervention provided during the treatment session.
/// </summary>
public sealed class ProvidedIntervention
{
  /// <summary>
  /// Gets or sets the unique identifier for this intervention.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the CPT code for billing purposes.
  /// </summary>
  public string CptCode { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the description of the intervention provided.
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Gets or sets the number of units provided.
  /// </summary>
  public int Units { get; set; }

  /// <summary>
  /// Gets or sets the duration of the intervention in minutes.
  /// </summary>
  public int? Minutes { get; set; }
}
