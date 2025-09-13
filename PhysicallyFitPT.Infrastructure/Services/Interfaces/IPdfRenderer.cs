// <copyright file="IPdfRenderer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

/// <summary>
/// Interface for PDF rendering services.
/// </summary>
public interface IPdfRenderer
{
    /// <summary>
    /// Renders a simple PDF document with title and body content.
    /// </summary>
    /// <param name="title">The title of the PDF document.</param>
    /// <param name="body">The main content body of the PDF document.</param>
    /// <returns>A byte array containing the generated PDF data.</returns>
    byte[] RenderSimple(string title, string body);
}
