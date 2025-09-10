// <copyright file="Entity.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Domain;

public abstract class Entity
{
  public Guid Id { get; set; } = Guid.NewGuid();

  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

  public string? CreatedBy { get; set; }

  public DateTimeOffset? UpdatedAt { get; set; }

  public string? UpdatedBy { get; set; }

  public bool IsDeleted { get; set; }
}
