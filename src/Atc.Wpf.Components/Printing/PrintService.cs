namespace Atc.Wpf.Components.Printing;

/// <summary>
/// Default implementation of <see cref="IPrintService"/> that wraps
/// WPF printing APIs with pagination, preview, and header/footer support.
/// </summary>
[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Print operations should not throw.")]
public class PrintService : IPrintService
{
    private readonly Func<Window?>? ownerResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintService"/> class.
    /// </summary>
    public PrintService()
        : this(ownerResolver: null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintService"/> class.
    /// </summary>
    /// <param name="ownerResolver">Function to resolve the owner window for preview dialogs.</param>
    public PrintService(Func<Window?>? ownerResolver)
        => this.ownerResolver = ownerResolver;

    /// <inheritdoc />
    public PrintResult Print(
        Visual visual,
        string? description = null)
        => Print(visual, new PrintSettings { Title = description ?? "Print Document" });

    /// <inheritdoc />
    public PrintResult Print(
        Visual visual,
        PrintSettings settings)
    {
        ArgumentNullException.ThrowIfNull(visual);
        ArgumentNullException.ThrowIfNull(settings);

        try
        {
            var printDialog = new PrintDialog();
            ApplySettings(printDialog, settings);

            if (settings.ShowPrintDialog && printDialog.ShowDialog() != true)
            {
                return PrintResult.Cancelled();
            }

            var pageSize = GetPageSize(printDialog, settings);
            var paginator = new VisualPaginator(visual, pageSize, settings);

            printDialog.PrintDocument(paginator, settings.Title);

            return PrintResult.Success(paginator.PageCount);
        }
        catch (Exception ex)
        {
            return PrintResult.Failed(ex.Message);
        }
    }

    /// <inheritdoc />
    public PrintResult PrintWithPreview(
        Visual visual,
        PrintSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(visual);

        settings ??= new PrintSettings();

        try
        {
            var printDialog = new PrintDialog();
            ApplySettings(printDialog, settings);

            var pageSize = GetPageSize(printDialog, settings);
            var paginator = new VisualPaginator(visual, pageSize, settings);
            var fixedDocument = CreateFixedDocument(paginator, pageSize);

            return ShowPreviewAndPrint(fixedDocument, paginator, settings);
        }
        catch (Exception ex)
        {
            return PrintResult.Failed(ex.Message);
        }
    }

    /// <inheritdoc />
    public PrintResult PrintDocument(
        FlowDocument document,
        PrintSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(document);

        settings ??= new PrintSettings();

        try
        {
            var printDialog = new PrintDialog();
            ApplySettings(printDialog, settings);

            if (settings.ShowPrintDialog && printDialog.ShowDialog() != true)
            {
                return PrintResult.Cancelled();
            }

            var pageSize = GetPageSize(printDialog, settings);
            var paginator = FlowDocumentPrintHelper.CreatePaginator(document, pageSize, settings);

            printDialog.PrintDocument(paginator, settings.Title);

            return PrintResult.Success(paginator.PageCount);
        }
        catch (Exception ex)
        {
            return PrintResult.Failed(ex.Message);
        }
    }

    /// <inheritdoc />
    public PrintResult PrintDocumentWithPreview(
        FlowDocument document,
        PrintSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(document);

        settings ??= new PrintSettings();

        try
        {
            var printDialog = new PrintDialog();
            ApplySettings(printDialog, settings);

            var pageSize = GetPageSize(printDialog, settings);
            var paginator = FlowDocumentPrintHelper.CreatePaginator(document, pageSize, settings);
            var fixedDocument = CreateFixedDocument(paginator, pageSize);

            return ShowPreviewAndPrint(fixedDocument, paginator, settings);
        }
        catch (Exception ex)
        {
            return PrintResult.Failed(ex.Message);
        }
    }

    private static void ApplySettings(
        PrintDialog dialog,
        PrintSettings settings)
    {
        dialog.PrintTicket.PageOrientation = settings.Orientation == PrintOrientation.Landscape
            ? global::System.Printing.PageOrientation.Landscape
            : global::System.Printing.PageOrientation.Portrait;

        if (settings.Copies > 1)
        {
            dialog.PrintTicket.CopyCount = settings.Copies;
        }
    }

    private static Size GetPageSize(
        PrintDialog dialog,
        PrintSettings settings)
    {
        if (settings.Orientation == PrintOrientation.Landscape)
        {
            return new Size(dialog.PrintableAreaHeight, dialog.PrintableAreaWidth);
        }

        return new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight);
    }

    private static FixedDocument CreateFixedDocument(
        DocumentPaginator paginator,
        Size pageSize)
    {
        var fixedDocument = new FixedDocument();
        fixedDocument.DocumentPaginator.PageSize = pageSize;

        for (var i = 0; i < paginator.PageCount; i++)
        {
            var page = paginator.GetPage(i);
            if (page == DocumentPage.Missing)
            {
                continue;
            }

            var fixedPage = new FixedPage
            {
                Width = pageSize.Width,
                Height = pageSize.Height,
            };

            // Wrap the page visual in a container
            var canvas = new Canvas
            {
                Width = pageSize.Width,
                Height = pageSize.Height,
            };

            var visualBrush = new VisualBrush(page.Visual) { Stretch = Stretch.None };
            canvas.Background = visualBrush;

            fixedPage.Children.Add(canvas);
            fixedPage.Measure(pageSize);
            fixedPage.Arrange(new Rect(pageSize));
            fixedPage.UpdateLayout();

            var pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(fixedPage);
            fixedDocument.Pages.Add(pageContent);
        }

        return fixedDocument;
    }

    private PrintResult ShowPreviewAndPrint(
        FixedDocument fixedDocument,
        DocumentPaginator paginator,
        PrintSettings settings)
    {
        var previewWindow = new PrintPreviewWindow
        {
            Owner = GetOwnerWindow(),
        };

        previewWindow.SetDocument(fixedDocument);

        if (previewWindow.ShowDialog() != true || !previewWindow.UserWantsToPrint)
        {
            return PrintResult.Cancelled();
        }

        var printDialog = new PrintDialog();
        ApplySettings(printDialog, settings);

        if (printDialog.ShowDialog() != true)
        {
            return PrintResult.Cancelled();
        }

        printDialog.PrintDocument(paginator, settings.Title);

        return PrintResult.Success(paginator.PageCount);
    }

    private Window GetOwnerWindow()
    {
        var owner = ownerResolver?.Invoke();
        return owner ?? Application.Current.MainWindow ?? throw new InvalidOperationException("No owner window available.");
    }
}