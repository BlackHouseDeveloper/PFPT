// <copyright file="SeedHashCalculator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Security.Cryptography;
using System.Text;

namespace PhysicallyFitPT.Seeder.Seeding.Hashing;

/// <summary>
/// Calculates SHA256 hashes for seed task content descriptors.
/// </summary>
public class SeedHashCalculator
{
  /// <summary>
  /// Computes a SHA256 hash for the given inputs.
  /// </summary>
  /// <param name="inputs">String inputs to hash.</param>
  /// <returns>A SHA256 hash string in lowercase hex format.</returns>
  public string ComputeHash(params string[] inputs)
  {
    if (inputs == null || inputs.Length == 0)
    {
      throw new ArgumentException("At least one input is required", nameof(inputs));
    }

    var combined = string.Join("|", inputs.Where(i => !string.IsNullOrEmpty(i)));
    var bytes = Encoding.UTF8.GetBytes(combined);

    using var sha256 = SHA256.Create();
    var hashBytes = sha256.ComputeHash(bytes);
    return Convert.ToHexString(hashBytes).ToLowerInvariant();
  }

  /// <summary>
  /// Computes a hash for a file's content.
  /// </summary>
  /// <param name="filePath">Path to the file.</param>
  /// <returns>A SHA256 hash of the file content, or null if file doesn't exist.</returns>
  public async Task<string?> ComputeFileHashAsync(string filePath)
  {
    if (!File.Exists(filePath))
    {
      return null;
    }

    var content = await File.ReadAllTextAsync(filePath);
    return ComputeHash(content);
  }

  /// <summary>
  /// Computes a fallback signature hash for inline data.
  /// </summary>
  /// <param name="taskId">The task identifier.</param>
  /// <param name="fallbackSignature">A signature representing the inline data.</param>
  /// <param name="versionToken">Optional version token for breaking changes.</param>
  /// <returns>A SHA256 hash string.</returns>
  public string ComputeFallbackHash(string taskId, string fallbackSignature, string? versionToken = null)
  {
    var inputs = new List<string> { taskId, fallbackSignature };
    if (!string.IsNullOrEmpty(versionToken))
    {
      inputs.Add(versionToken);
    }

    return ComputeHash(inputs.ToArray());
  }
}