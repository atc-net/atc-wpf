namespace Atc.Wpf.Components.Printing;

/// <summary>
/// A <see cref="DocumentPaginator"/> that renders a <see cref="Visual"/> to a bitmap,
/// slices it vertically into pages, and applies scaling and margins.
/// </summary>
[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "DrawingVisual objects are managed by WPF.")]
internal sealed class VisualPaginator : DocumentPaginator
{
    private readonly Visual sourceVisual;
    private readonly PrintSettings settings;
    private readonly double headerFooterHeight;
    private RenderTargetBitmap? renderedBitmap;
    private int pageCount;
    private double scaleFactor = 1.0;
    private double scaledContentWidth;
    private double printableHeight;

    public VisualPaginator(
        Visual sourceVisual,
        Size pageSize,
        PrintSettings settings)
    {
        this.sourceVisual = sourceVisual;
        PageSize = pageSize;
        this.settings = settings;
        headerFooterHeight = settings.HeaderFooter is not null ? settings.HeaderFooter.FontSize * 2 : 0;

        RenderAndPaginate();
    }

    public override bool IsPageCountValid => true;

    public override int PageCount => pageCount;

    public override Size PageSize { get; set; }

    public override IDocumentPaginatorSource Source => null!;

    public override DocumentPage GetPage(int pageNumber)
    {
        if (renderedBitmap is null || pageNumber < 0 || pageNumber >= pageCount)
        {
            return DocumentPage.Missing;
        }

        var margins = settings.Margins;
        var printableWidth = PageSize.Width - margins.Left - margins.Right;

        var sourceY = pageNumber * printableHeight / scaleFactor;
        var sourceHeight = System.Math.Min(
            printableHeight / scaleFactor,
            renderedBitmap.PixelHeight - sourceY);

        if (sourceHeight <= 0)
        {
            return DocumentPage.Missing;
        }

        var croppedBitmap = new CroppedBitmap(
            renderedBitmap,
            new Int32Rect(
                0,
                (int)sourceY,
                renderedBitmap.PixelWidth,
                (int)System.Math.Ceiling(sourceHeight)));

        var drawingVisual = new DrawingVisual();
        using (var dc = drawingVisual.RenderOpen())
        {
            var contentX = margins.Left + ((printableWidth - scaledContentWidth) / 2);
            var contentY = margins.Top + headerFooterHeight;

            dc.DrawImage(
                croppedBitmap,
                new Rect(
                    contentX,
                    contentY,
                    scaledContentWidth,
                    sourceHeight * scaleFactor));

            if (settings.HeaderFooter is not null)
            {
                DrawHeaderFooter(dc, pageNumber + 1, pageCount, margins);
            }
        }

        return new DocumentPage(
            drawingVisual,
            PageSize,
            new Rect(PageSize),
            new Rect(PageSize));
    }

    private void RenderAndPaginate()
    {
        var element = sourceVisual as FrameworkElement;
        var sourceWidth = element?.ActualWidth ?? 0;
        var sourceHeight2 = element?.ActualHeight ?? 0;

        if (sourceWidth <= 0 || sourceHeight2 <= 0)
        {
            pageCount = 0;
            return;
        }

        var dpi = 96;
        renderedBitmap = new RenderTargetBitmap(
            (int)System.Math.Ceiling(sourceWidth),
            (int)System.Math.Ceiling(sourceHeight2),
            dpi,
            dpi,
            PixelFormats.Pbgra32);
        renderedBitmap.Render(sourceVisual);

        var margins = settings.Margins;
        var printableWidth = PageSize.Width - margins.Left - margins.Right;
        printableHeight = PageSize.Height - margins.Top - margins.Bottom - (headerFooterHeight * 2);

        scaleFactor = settings.ScaleMode switch
        {
            PrintScaleMode.FitToPage => printableWidth / sourceWidth,
            PrintScaleMode.ShrinkToFit => sourceWidth > printableWidth
                ? printableWidth / sourceWidth
                : 1.0,
            _ => 1.0,
        };

        scaledContentWidth = sourceWidth * scaleFactor;
        var scaledContentHeight = sourceHeight2 * scaleFactor;

        pageCount = System.Math.Max(
            1,
            (int)System.Math.Ceiling(scaledContentHeight / printableHeight));
    }

    private void DrawHeaderFooter(
        DrawingContext dc,
        int pageNumber,
        int totalPages,
        Thickness margins)
    {
        var hf = settings.HeaderFooter!;
        var title = settings.Title;
        var typeface = new Typeface(hf.FontFamily);
        var brush = Brushes.Black;
        var printableWidth = PageSize.Width - margins.Left - margins.Right;

        DrawTextLine(
            dc,
            hf.HeaderLeft,
            hf.HeaderCenter,
            hf.HeaderRight,
            margins.Left,
            margins.Top,
            printableWidth,
            typeface,
            hf.FontSize,
            brush,
            pageNumber,
            totalPages,
            title);

        DrawTextLine(
            dc,
            hf.FooterLeft,
            hf.FooterCenter,
            hf.FooterRight,
            margins.Left,
            PageSize.Height - margins.Bottom - hf.FontSize - 4,
            printableWidth,
            typeface,
            hf.FontSize,
            brush,
            pageNumber,
            totalPages,
            title);
    }

    [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Drawing context helper requires all parameters.")]
    private static void DrawTextLine(
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
        string title)
    {
        var pixelsPerDip = VisualTreeHelper.GetDpi(Application.Current.MainWindow!).PixelsPerDip;

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