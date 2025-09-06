using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace PhysicallyFitPT.Infrastructure.Services;

public interface IPdfRenderer
{
  byte[] RenderSimple(string title, string body);
}

public class PdfRenderer : IPdfRenderer
{
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
