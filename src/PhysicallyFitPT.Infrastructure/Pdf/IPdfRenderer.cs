// <copyright file="IPdfRenderer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Pdf;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Interface for rendering PDF documents from note data.
/// </summary>
public interface IPdfRenderer
{
  /// <summary>
  /// Renders a note as a PDF document.
  /// </summary>
  /// <param name="noteId">The unique identifier of the note to render.</param>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>A byte array containing the PDF document.</returns>
  Task<byte[]> RenderNoteAsync(Guid noteId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Renders a demo PDF document with clinic letterhead.
  /// </summary>
  /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
  /// <returns>A byte array containing the PDF document.</returns>
  Task<byte[]> RenderDemoAsync(CancellationToken cancellationToken = default);
}
