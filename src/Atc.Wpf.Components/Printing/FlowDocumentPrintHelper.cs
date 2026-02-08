namespace Atc.Wpf.Components.Printing;

/// <summary>
/// Helper for preparing a <see cref="FlowDocument"/> for printing.
/// Clones the document and returns a paginator, optionally wrapped
/// with header/footer support.
/// </summary>
internal static class FlowDocumentPrintHelper
{
    public static DocumentPaginator CreatePaginator(
        FlowDocument document,
        Size pageSize,
        PrintSettings settings)
    {
        var clone = CloneDocument(document);

        clone.PageWidth = pageSize.Width;
        clone.PageHeight = pageSize.Height;
        clone.PagePadding = settings.Margins;
        clone.ColumnWidth = pageSize.Width;

        var paginator = ((IDocumentPaginatorSource)clone).DocumentPaginator;
        paginator.PageSize = pageSize;

        if (settings.HeaderFooter is null)
        {
            return paginator;
        }

        // Adjust padding to leave room for headers/footers
        var hfHeight = settings.HeaderFooter.FontSize * 2;
        clone.PagePadding = new Thickness(
            settings.Margins.Left,
            settings.Margins.Top + hfHeight,
            settings.Margins.Right,
            settings.Margins.Bottom + hfHeight);

        paginator = ((IDocumentPaginatorSource)clone).DocumentPaginator;
        paginator.PageSize = pageSize;

        return new HeaderFooterDocumentPaginator(
            paginator,
            settings.HeaderFooter,
            settings.Title,
            settings.Margins);
    }

    private static FlowDocument CloneDocument(FlowDocument source)
    {
        var sourceRange = new TextRange(source.ContentStart, source.ContentEnd);
        using var stream = new MemoryStream();
        sourceRange.Save(stream, DataFormats.XamlPackage);

        var clone = new FlowDocument();
        var cloneRange = new TextRange(clone.ContentStart, clone.ContentEnd);
        stream.Position = 0;
        cloneRange.Load(stream, DataFormats.XamlPackage);

        clone.FontFamily = source.FontFamily;
        clone.FontSize = source.FontSize;

        return clone;
    }
}