// <copyright file="DiagnosticsEventIds.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared.Diagnostics;

using Microsoft.Extensions.Logging;

/// <summary>
/// Central registry for diagnostics-related event identifiers to keep log semantics consistent.
/// </summary>
public static class DiagnosticsEventIds
{
  /// <summary>
  /// Event identifier emitted when developer diagnostics become enabled in a production context.
  /// </summary>
  public static readonly EventId DeveloperDiagnosticsEnabled = new(7001, nameof(DeveloperDiagnosticsEnabled));

  /// <summary>
  /// Event identifier emitted when developer diagnostics are explicitly disabled in production.
  /// </summary>
  public static readonly EventId DeveloperDiagnosticsDisabled = new(7002, nameof(DeveloperDiagnosticsDisabled));

  /// <summary>
  /// Event identifier emitted when environment overrides are applied in a debug context.
  /// </summary>
  public static readonly EventId DeveloperDiagnosticsOverrideNotice = new(7003, nameof(DeveloperDiagnosticsOverrideNotice));
}
