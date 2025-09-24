// <copyright file="JsonDataLoader.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PhysicallyFitPT.Seeder.Utils;

/// <summary>
/// Utility for loading JSON data files with fallback support.
/// </summary>
public class JsonDataLoader
{
  private readonly ILogger<JsonDataLoader> logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="JsonDataLoader"/> class.
  /// </summary>
  /// <param name="logger">Logger instance.</param>
  public JsonDataLoader(ILogger<JsonDataLoader> logger)
  {
    this.logger = logger;
  }

  /// <summary>
  /// Loads data from a JSON file, with fallback to provided default data.
  /// </summary>
  /// <typeparam name="T">Type of data to deserialize.</typeparam>
  /// <param name="filePath">Path to the JSON file.</param>
  /// <param name="fallbackData">Default data to use if file doesn't exist or fails to load.</param>
  /// <returns>Deserialized data or fallback data.</returns>
  public async Task<T[]> LoadDataAsync<T>(string filePath, T[] fallbackData)
  {
    try
    {
      if (!File.Exists(filePath))
      {
        logger.LogDebug("JSON file not found at {FilePath}, using fallback data", filePath);
        return fallbackData;
      }

      var jsonContent = await File.ReadAllTextAsync(filePath);
      if (string.IsNullOrWhiteSpace(jsonContent))
      {
        logger.LogDebug("JSON file at {FilePath} is empty, using fallback data", filePath);
        return fallbackData;
      }

      var data = JsonSerializer.Deserialize<T[]>(jsonContent, new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
      });

      if (data == null || data.Length == 0)
      {
        logger.LogDebug("JSON file at {FilePath} deserialized to empty array, using fallback data", filePath);
        return fallbackData;
      }

      logger.LogDebug("Successfully loaded {Count} items from {FilePath}", data.Length, filePath);
      return data;
    }
    catch (JsonException ex)
    {
      logger.LogWarning(ex, "Failed to parse JSON file at {FilePath}, using fallback data", filePath);
      return fallbackData;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Unexpected error loading JSON file at {FilePath}, using fallback data", filePath);
      return fallbackData;
    }
  }

  /// <summary>
  /// Gets the path for a data file relative to the Data directory.
  /// </summary>
  /// <param name="fileName">Name of the data file.</param>
  /// <returns>Full path to the data file.</returns>
  public static string GetDataFilePath(string fileName)
  {
    var baseDir = AppContext.BaseDirectory;
    return Path.Combine(baseDir, "Data", fileName);
  }
}