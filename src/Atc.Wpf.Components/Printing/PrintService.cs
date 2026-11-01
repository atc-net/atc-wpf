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
    {
        this.ownerResolver = ownerResolver;
        DialogFactory = settings => new PrintDialogAdapter(settings);
        PreviewPresenter = ShowPreviewWindow;
    }

    /// <summary>
    /// Gets or sets the factory that creates the print dialog for the given settings
    /// (a test seam; defaults to a <see cref="PrintDialog"/>).
    /// </summary>
    internal Func<PrintSettings, IPrintDialogAdapter> DialogFactory { get; set; }

    /// <summary>
    /// Gets or sets the function that shows the preview and returns whether the user chose to print
    /// (a test seam; defaults to <see cref="PrintPreviewWindow"/>).
    /// </summary>
    internal Func<FixedDocument, bool> PreviewPresenter { get; set; }

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
            var printDialog = DialogFactory(settings);

            if (settings.ShowPrintDialog && printDialog.ShowDialog() != true)
            {
                return PrintResult.Cancelled();
            }

            var paginator = new VisualPaginator(visual, printDialog.PageSize, settings);

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
            return ShowPreviewAndPrint(
                pageSize => new VisualPaginator(visual, pageSize, settings),
                settings);
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
            var printDialog = DialogFactory(settings);

            if (settings.ShowPrintDialog && printDialog.ShowDialog() != true)
            {
                return PrintResult.Cancelled();
            }

            var paginator = FlowDocumentPrintHelper.CreatePaginator(document, printDialog.PageSize, settings);

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
            return ShowPreviewAndPrint(
                pageSize => FlowDocumentPrintHelper.CreatePaginator(document, pageSize, settings),
                settings);
        }
        catch (Exception ex)
        {
            return PrintResult.Failed(ex.Message);
        }
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

    /// <summary>
    /// Previews the document laid out for the default printer, then prints it laid out again for the printer,
    /// paper and orientation chosen in the print dialog shown after the preview.
    /// </summary>
    private PrintResult ShowPreviewAndPrint(
        Func<Size, DocumentPaginator> createPaginator,
        PrintSettings settings)
    {
        var previewPageSize = DialogFactory(settings).PageSize;
        var paginator = createPaginator(previewPageSize);
        var fixedDocument = CreateFixedDocument(paginator, previewPageSize);

        if (!PreviewPresenter(fixedDocument))
        {
            return PrintResult.Cancelled();
        }

        var printDialog = DialogFactory(settings);

        if (printDialog.ShowDialog() != true)
        {
            return PrintResult.Cancelled();
        }

        var printPageSize = printDialog.PageSize;
        if (printPageSize != previewPageSize)
        {
            paginator = createPaginator(printPageSize);
        }

        printDialog.PrintDocument(paginator, settings.Title);

        return PrintResult.Success(paginator.PageCount);
    }

    private bool ShowPreviewWindow(FixedDocument fixedDocument)
    {
        var owner = GetOwnerWindow();
        var previewWindow = new PrintPreviewWindow
        {
            Owner = owner,
            FlowDirection = owner.FlowDirection,
        };

        previewWindow.SetDocument(fixedDocument);

        return previewWindow.ShowDialog() == true && previewWindow.UserWantsToPrint;
    }

    private Window GetOwnerWindow()
    {
        var owner = ownerResolver?.Invoke();
        return owner ?? Application.Current?.MainWindow ?? throw new InvalidOperationException("No owner window available.");
    }
}