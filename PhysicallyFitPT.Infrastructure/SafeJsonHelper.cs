// <copyright file="SafeJsonHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Utilities;

using System.Text.Json;
using Microsoft.Extensions.Logging;

/// <summary>
/// Safe JSON operations to prevent runtime errors.
/// </summary>
public static class SafeJsonHelper
{
  private static readonly JsonSerializerOptions DefaultOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true,
  };

  /// <summary>
  /// Safely deserializes JSON string, returning default value on error.
  /// </summary>
  /// <typeparam name="T">The type to deserialize to.</typeparam>
  /// <param name="json">The JSON string to deserialize.</param>
  /// <param name="logger">Logger instance for logging errors.</param>
  /// <param name="defaultValue">Default value to return on error.</param>
  /// <returns>The deserialized object or default value on error.</returns>
  public static T? SafeDeserialize<T>(string json, ILogger? logger = null, T? defaultValue = default)
  {
    if (string.IsNullOrWhiteSpace(json))
    {
      return defaultValue;
    }

    try
    {
      return JsonSerializer.Deserialize<T>(json, DefaultOptions);
    }
    catch (JsonException ex)
    {
      logger?.LogWarning(ex, "Failed to deserialize JSON: {Json}", json);
      return defaultValue;
    }
    catch (Exception ex)
    {
      logger?.LogError(ex, "Unexpected error deserializing JSON: {Json}", json);
      return defaultValue;
    }
  }

  /// <summary>
  /// Safely serializes object to JSON, returning empty object on error.
  /// </summary>
  /// <typeparam name="T">The type of the object to serialize.</typeparam>
  /// <param name="obj">The object to serialize.</param>
  /// <param name="logger">Logger instance for logging errors.</param>
  /// <param name="defaultValue">Default JSON string to return on error.</param>
  /// <returns>The JSON string representation or default value on error.</returns>
  public static string SafeSerialize<T>(T obj, ILogger? logger = null, string defaultValue = "{}")
  {
    if (obj is null)
    {
      return defaultValue;
    }

    try
    {
      return JsonSerializer.Serialize(obj, DefaultOptions);
    }
    catch (JsonException ex)
    {
      logger?.LogWarning(ex, "Failed to serialize object of type {Type}", typeof(T).Name);
      return defaultValue;
    }
    catch (Exception ex)
    {
      logger?.LogError(ex, "Unexpected error serializing object of type {Type}", typeof(T).Name);
      return defaultValue;
    }
  }

  /// <summary>
  /// Validates if string is valid JSON.
  /// </summary>
  /// <param name="json">The JSON string to validate.</param>
  /// <returns>True if the JSON is valid; otherwise, false.</returns>
  public static bool IsValidJson(string json)
  {
    if (string.IsNullOrWhiteSpace(json))
    {
      return false;
    }

    try
    {
      JsonDocument.Parse(json);
      return true;
    }
    catch (JsonException)
    {
      return false;
    }
  }
}
