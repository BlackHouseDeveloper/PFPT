// <copyright file="NoteDtoSummary.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;

  public class NoteDtoSummary
  {
    public Guid Id { get; set; }

    public string VisitType { get; set; } = null!;

    public bool IsSigned { get; set; }

    public DateTimeOffset? SignedAt { get; set; }

    public string? SignedBy { get; set; }

    public DateTimeOffset Date { get; set; } // Date of the encounter (appointment scheduled start or note creation)
  }
}
