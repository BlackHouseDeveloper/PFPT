// <copyright file="PdfRenderer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Services;

using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

/// <summary>
/// PDF rendering service implementation using QuestPDF.
/// </summary>
public class PdfRenderer : IPdfRenderer
{
  /// <inheritdoc/>
  public byte[] RenderSimple(string title, string body)
  {
    return Document.Create(container =>
    {
      container.Page(page =>
      {
        page.Size(PageSizes.A4);
        page.Margin(36);
        page.DefaultTextStyle(t => t.FontSize(11));
        page.Content().Column(col =>
        {
          col.Item().Text(title).FontSize(18).SemiBold();
          col.Item().Text(body);
        });
      });
    }).GeneratePdf();
  }
}




