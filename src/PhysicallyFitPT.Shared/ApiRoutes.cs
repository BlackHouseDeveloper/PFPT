// <copyright file="ApiRoutes.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Centralizes API route generation to keep client paths consistent.
/// </summary>
public static class ApiRoutes
{
  private static readonly ConcurrentDictionary<string, string> Cache = new();
  private static string? basePath;

  /// <summary>
  /// Configures the base path (e.g. <c>/pfpt</c>) that should prefix all API routes.
  /// </summary>
  /// <param name="prefix">The base path prefix (with or without leading slash).</param>
  public static void ConfigureBasePath(string? prefix)
  {
    basePath = string.IsNullOrWhiteSpace(prefix)
      ? null
      : $"/{prefix!.Trim('/')}";
    Cache.Clear();
  }

  /// <summary>
  /// Builds a route anchored at <c>/api</c>.
  /// </summary>
  /// <param name="segments">Additional path segments to append.</param>
  /// <returns>Normalized API route.</returns>
  public static string Combine(params string[] segments) => Compose(BaseSegments(), segments);

  /// <summary>
  /// Builds a route anchored at <c>/api/v1</c>.
  /// </summary>
  /// <param name="segments">Additional path segments to append.</param>
  /// <returns>Normalized API v1 route.</returns>
  public static string V1(params string[] segments) => Compose(BaseSegments("v1"), segments);

  /// <summary>
  /// Resets static state (for tests only).
  /// </summary>
  internal static void ResetForTests()
  {
#if DEBUG
    basePath = null;
    Cache.Clear();
#endif
  }

  private static string[] BaseSegments(params string[] additional)
  {
    var list = new List<string> { "api" };
    if (additional?.Length > 0)
    {
      list.AddRange(additional);
    }

    return list.ToArray();
  }

  private static string Compose(string[] baseSegments, params string[] more)
  {
    var key = string.Join('|', baseSegments.Concat(more ?? Array.Empty<string>()));
    return Cache.GetOrAdd(key, _ => Normalize(baseSegments, more ?? Array.Empty<string>()));
  }

  private static string Normalize(IEnumerable<string> first, IEnumerable<string> second)
  {
    var segments = first.Concat(second)
      .Where(segment => !string.IsNullOrWhiteSpace(segment))
      .Select(segment => segment.Trim('/'))
      .Where(segment => segment.Length > 0);

    var core = string.Join('/', segments);
    var prefix = basePath?.TrimEnd('/') ?? string.Empty;

    return string.IsNullOrEmpty(prefix) ? $"/{core}" : $"{prefix}/{core}";
  }
}
