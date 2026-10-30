namespace Atc.Wpf.Components.Tests.Printing;

/// <summary>
/// The paginators take the text DPI from the visual they print, so they work in a host without an
/// <see cref="Application"/> or a main window (tray-only, startup, or hosted in another UI framework).
/// </summary>
public sealed class PaginatorWithoutMainWindowTests
{
    [StaFact]
    public void VisualPaginator_GetPage_WithHeaderAndFooter_DoesNotNeedAMainWindow()
    {
        Assert.Null(Application.Current);

        var paginator = new VisualPaginator(
            CreateArrangedVisual(),
            new Size(600, 800),
            CreateSettings());

        var page = paginator.GetPage(0);

        Assert.NotSame(DocumentPage.Missing, page);
        Assert.NotNull(page.Visual);
    }

    [StaFact]
    public void HeaderFooterDocumentPaginator_GetPage_DoesNotNeedAMainWindow()
    {
        Assert.Null(Application.Current);

        var settings = CreateSettings();
        var inner = new VisualPaginator(
            CreateArrangedVisual(),
            new Size(600, 800),
            new PrintSettings());
        var paginator = new HeaderFooterDocumentPaginator(
            inner,
            settings.HeaderFooter!,
            "Title",
            settings.Margins);

        var page = paginator.GetPage(0);

        Assert.NotSame(DocumentPage.Missing, page);
        Assert.NotNull(page.Visual);
    }

    [StaFact]
    public void GetPixelsPerDip_WithoutAVisualOrAMainWindow_FallsBackTo96Dpi()
    {
        Assert.Null(Application.Current);

        Assert.Equal(1.0, PrintDpiHelper.GetPixelsPerDip(visual: null));
    }

    private static PrintSettings CreateSettings()
        => new()
        {
            HeaderFooter = new HeaderFooterSettings
            {
                HeaderLeft = "{Title}",
                HeaderCenter = "Header",
                FooterRight = "Page {PageNumber} of {TotalPages}",
            },
        };

    private static Border CreateArrangedVisual()
    {
        var border = new Border
        {
            Width = 200,
            Height = 300,
            Background = Brushes.SteelBlue,
        };

        border.Measure(new Size(200, 300));
        border.Arrange(new Rect(0, 0, 200, 300));
        return border;
    }
}