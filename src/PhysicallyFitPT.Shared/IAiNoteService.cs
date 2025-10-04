// <copyright file="IAiNoteService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// Interface for AI note generation service.
  /// </summary>
  public interface IAiNoteService
  {
    /// <summary>
    /// Generates an AI-powered clinical note summary.
    /// </summary>
    /// <param name="noteInput">The raw note input text.</param>
    /// <param name="patientContext">Optional patient context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated note summary result.</returns>
    Task<AiNoteSummaryResult> GenerateNoteSummaryAsync(string noteInput, string? patientContext = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the AI service is healthy.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if healthy, false otherwise.</returns>
    Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
  }

  /// <summary>
  /// Result from AI note generation.
  /// </summary>
  public class AiNoteSummaryResult
  {
    /// <summary>
    /// Gets or sets the generated summary text.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the generation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the processing time in milliseconds.
    /// </summary>
    public int ProcessingTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of generation.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the word count of the input.
    /// </summary>
    public int WordCount { get; set; }
  }
}
