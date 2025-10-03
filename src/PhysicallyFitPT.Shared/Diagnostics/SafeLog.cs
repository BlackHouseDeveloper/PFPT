// <copyright file="SafeLog.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared.Diagnostics;

/// <summary>
/// Provides small helpers to prevent logging sensitive payloads.
/// </summary>
public static class SafeLog
{
  /// <summary>
  /// Redacts a potentially sensitive value, replacing populated strings with an ellipsis.
  /// </summary>
  /// <param name="value">Potentially sensitive string value.</param>
  /// <returns>A redacted representation safe for logging.</returns>
  public static string Redact(string? value) => string.IsNullOrWhiteSpace(value) ? string.Empty : "***";
}
