// <copyright file="BodyPartData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Core;

/// <summary>
/// Represents clinical data specific to a body part for SOAP note documentation.
/// </summary>
public class BodyPartData
{
  /// <summary>
  /// Gets or sets the display name of the body part.
  /// </summary>
  public required string Name { get; set; }

  /// <summary>
  /// Gets or sets specific anatomical pain locations for this body part.
  /// </summary>
  public required List<string> PainLocations { get; set; }

  /// <summary>
  /// Gets or sets common functional limitations associated with this body part.
  /// </summary>
  public required List<string> FunctionalLimitations { get; set; }

  /// <summary>
  /// Gets or sets range of motion measurements specific to this body part.
  /// </summary>
  public required List<string> RomMeasurements { get; set; }

  /// <summary>
  /// Gets or sets muscles tested during manual muscle testing for this body part.
  /// </summary>
  public required List<string> MmtMuscles { get; set; }

  /// <summary>
  /// Gets or sets special orthopedic tests for this body part.
  /// </summary>
  public required List<string> SpecialTests { get; set; }
}
