namespace Atc.Wpf.Components.Printing;

/// <summary>
/// A <see cref="DocumentPaginator"/> decorator that draws header and footer text
/// on each page of an inner paginator.
/// </summary>
internal sealed class HeaderFooterDocumentPaginator : DocumentPaginator
{
    private readonly DocumentPaginator innerPaginator;
    private readonly HeaderFooterSettings headerFooter;
    private readonly string title;
    private readonly Thickness margins;

    public HeaderFooterDocumentPaginator(
        DocumentPaginator innerPaginator,
        HeaderFooterSettings headerFooter,
        string title,
        Thickness margins)
    {
        this.innerPaginator = innerPaginator;
        this.headerFooter = headerFooter;
        this.title = title;
        this.margins = margins;
    }

    public override bool IsPageCountValid => innerPaginator.IsPageCountValid;

    public override int PageCount => innerPaginator.PageCount;

    public override Size PageSize
    {
        get => innerPaginator.PageSize;
        set => innerPaginator.PageSize = value;
    }

    public override IDocumentPaginatorSource Source => innerPaginator.Source;

    public override DocumentPage GetPage(int pageNumber)
    {
        var innerPage = innerPaginator.GetPage(pageNumber);
        if (innerPage == DocumentPage.Missing)
        {
            return DocumentPage.Missing;
        }

        var pageSize2 = PageSize;
        var drawingVisual = new DrawingVisual();
        using (var dc = drawingVisual.RenderOpen())
        {
            // Draw the inner page content
            var pageVisual = innerPage.Visual;
            dc.DrawRectangle(
                new VisualBrush(pageVisual) { Stretch = Stretch.None },
                pen: null,
                new Rect(pageSize2));

            // Draw header and footer
            var typeface = new Typeface(headerFooter.FontFamily);
            var brush = Brushes.Black;
            var fontSize = headerFooter.FontSize;
            var printableWidth = pageSize2.Width - margins.Left - margins.Right;
            var totalPages = PageCount;

            var pixelsPerDip = VisualTreeHelper.GetDpi(Application.Current.MainWindow!).PixelsPerDip;

            DrawLine(dc, headerFooter.HeaderLeft, headerFooter.HeaderCenter, headerFooter.HeaderRight, margins.Left, margins.Top / 2, printableWidth, typeface, fontSize, brush, pageNumber + 1, totalPages, pixelsPerDip);
            DrawLine(dc, headerFooter.FooterLeft, headerFooter.FooterCenter, headerFooter.FooterRight, margins.Left, pageSize2.Height - (margins.Bottom / 2) - fontSize, printableWidth, typeface, fontSize, brush, pageNumber + 1, totalPages, pixelsPerDip);
        }

        return new DocumentPage(drawingVisual, pageSize2, new Rect(pageSize2), new Rect(pageSize2));
    }

    [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Drawing context helper requires all parameters.")]
    private void DrawLine(
        DrawingContext dc,
        string leftTemplate,
        string centerTemplate,
        string rightTemplate,
        double x,
        double y,
        double width,
        Typeface typeface,
        double fontSize,
        Brush brush,
        int pageNumber,
        int totalPages,
        double pixelsPerDip)
    {
        if (!string.IsNullOrEmpty(leftTemplate))
        {
            var text = HeaderFooterFormatter.Format(leftTemplate, pageNumber, totalPages, title);
            var ft = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, fontSize, brush, pixelsPerDip);
            dc.DrawText(ft, new Point(x, y));
        }

        if (!string.IsNullOrEmpty(centerTemplate))
        {
            var text = HeaderFooterFormatter.Format(centerTemplate, pageNumber, totalPages, title);
            var ft = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, fontSize, brush, pixelsPerDip);
            dc.DrawText(ft, new Point(x + ((width - ft.Width) / 2), y));
        }

        if (!string.IsNullOrEmpty(rightTemplate))
        {
            var text = HeaderFooterFormatter.Format(rightTemplate, pageNumber, totalPages, title);
            var ft = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, fontSize, brush, pixelsPerDip);
            dc.DrawText(ft, new Point(x + width - ft.Width, y));
        }
    }
}