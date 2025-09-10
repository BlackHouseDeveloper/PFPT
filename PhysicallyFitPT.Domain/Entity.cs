// <copyright file="Entity.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

/// <summary>
/// Base entity class providing common properties for domain entities.
/// </summary>
public abstract class Entity
{
  /// <summary>
  /// Gets or sets the unique identifier for the entity.
  /// </summary>
  public Guid Id { get; set; } = Guid.NewGuid();

  /// <summary>
  /// Gets or sets the timestamp when the entity was created.
  /// </summary>
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// Gets or sets the identifier of the user who created the entity.
  /// </summary>
  public string? CreatedBy { get; set; }

  /// <summary>
  /// Gets or sets the timestamp when the entity was last updated.
  /// </summary>
  public DateTimeOffset? UpdatedAt { get; set; }

  /// <summary>
  /// Gets or sets the identifier of the user who last updated the entity.
  /// </summary>
  public string? UpdatedBy { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether the entity is soft-deleted.
  /// </summary>
  public bool IsDeleted { get; set; }
}
