namespace Atc.Wpf.Components.Tests.Printing;

/// <summary>
/// <see cref="PrintService"/> paginates with the printable area that <see cref="PrintDialog"/> reports for the
/// orientation on its print ticket, without swapping width and height again for landscape.
/// Runs against the "Microsoft Print to PDF" queue and is skipped where that queue is not installed.
/// </summary>
public sealed class PrintServicePageSizeTests
{
    private const string PdfQueueName = "Microsoft Print to PDF";

    private static readonly MethodInfo ApplySettingsMethod = GetPrivateStaticMethod("ApplySettings");
    private static readonly MethodInfo GetPageSizeMethod = GetPrivateStaticMethod("GetPageSize");

    [StaFact]
    public void Landscape_PageSize_IsWiderThanTall_AndMatchesPrintDialog()
    {
        using var server = new LocalPrintServer();
        using var queue = GetPdfQueueOrSkip(server);

        var pageSize = GetPageSize(queue, PrintOrientation.Landscape);
        var reference = GetPrintableArea(queue, PageOrientation.Landscape);

        Assert.True(
            pageSize.Width > pageSize.Height,
            $"Landscape page size {pageSize.Width} x {pageSize.Height} is not wider than tall.");
        Assert.Equal(reference, pageSize);
    }

    [StaFact]
    public void Portrait_PageSize_IsTallerThanWide_AndMatchesPrintDialog()
    {
        using var server = new LocalPrintServer();
        using var queue = GetPdfQueueOrSkip(server);

        var pageSize = GetPageSize(queue, PrintOrientation.Portrait);
        var reference = GetPrintableArea(queue, PageOrientation.Portrait);

        Assert.True(
            pageSize.Height > pageSize.Width,
            $"Portrait page size {pageSize.Width} x {pageSize.Height} is not taller than wide.");
        Assert.Equal(reference, pageSize);
    }

    [StaFact]
    public void Landscape_PageSize_IsThePortraitPageSizeTurned()
    {
        using var server = new LocalPrintServer();
        using var queue = GetPdfQueueOrSkip(server);

        var portrait = GetPageSize(queue, PrintOrientation.Portrait);
        var landscape = GetPageSize(queue, PrintOrientation.Landscape);

        Assert.Equal(portrait.Width, landscape.Height, precision: 6);
        Assert.Equal(portrait.Height, landscape.Width, precision: 6);
    }

    /// <summary>
    /// The page size the service paginates with: the settings are applied to a fresh dialog the way
    /// <see cref="PrintService"/> does, then the size is read through the service's own helper.
    /// </summary>
    private static Size GetPageSize(
        PrintQueue queue,
        PrintOrientation orientation)
    {
        var dialog = new PrintDialog { PrintQueue = queue };
        ApplySettingsMethod.Invoke(null, [dialog, new PrintSettings { Orientation = orientation }]);
        return (Size)GetPageSizeMethod.Invoke(null, [dialog])!;
    }

    /// <summary>
    /// What <see cref="PrintDialog"/> itself reports for a print ticket with the given orientation.
    /// </summary>
    private static Size GetPrintableArea(
        PrintQueue queue,
        PageOrientation orientation)
    {
        var dialog = new PrintDialog
        {
            PrintQueue = queue,
            PrintTicket = new PrintTicket { PageOrientation = orientation },
        };

        return new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight);
    }

    private static PrintQueue GetPdfQueueOrSkip(LocalPrintServer server)
    {
        try
        {
            return server.GetPrintQueue(PdfQueueName);
        }
        catch (PrintQueueException)
        {
            Assert.Skip($"The '{PdfQueueName}' print queue is not installed on this machine.");
            throw;
        }
    }

    private static MethodInfo GetPrivateStaticMethod(string name)
        => typeof(PrintService).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)
           ?? throw new InvalidOperationException($"PrintService.{name} was not found; update this test to the renamed helper.");
}