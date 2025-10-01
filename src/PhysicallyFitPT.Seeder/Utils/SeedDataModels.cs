// <copyright file="SeedDataModels.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Seeder.Utils;

/// <summary>
/// Data model for CPT code JSON data.
/// </summary>
public class CptCodeSeedData
{
  /// <summary>
  /// Gets or sets the CPT code.
  /// </summary>
  public string Code { get; set; } = null!;

  /// <summary>
  /// Gets or sets the description.
  /// </summary>
  public string Description { get; set; } = null!;
}

/// <summary>
/// Data model for ICD-10 code JSON data.
/// </summary>
public class Icd10CodeSeedData
{
  /// <summary>
  /// Gets or sets the ICD-10 code.
  /// </summary>
  public string Code { get; set; } = null!;

  /// <summary>
  /// Gets or sets the description.
  /// </summary>
  public string Description { get; set; } = null!;
}

/// <summary>
/// Data model for patient JSON data.
/// </summary>
public class PatientSeedData
{
  /// <summary>
  /// Gets or sets the Medical Record Number (MRN).
  /// </summary>
  public string MRN { get; set; } = null!;

  /// <summary>
  /// Gets or sets the first name.
  /// </summary>
  public string FirstName { get; set; } = null!;

  /// <summary>
  /// Gets or sets the last name.
  /// </summary>
  public string LastName { get; set; } = null!;

  /// <summary>
  /// Gets or sets the email address.
  /// </summary>
  public string? Email { get; set; }
}
