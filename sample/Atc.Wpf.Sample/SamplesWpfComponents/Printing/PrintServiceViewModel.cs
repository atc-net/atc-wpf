namespace Atc.Wpf.Sample.SamplesWpfComponents.Printing;

public sealed partial class PrintServiceViewModel : ViewModelBase
{
    private readonly IPrintService printService = new PrintService();

    [ObservableProperty]
    private string resultLog = string.Empty;

    [ObservableProperty]
    private bool isLandscape;

    [ObservableProperty]
    private PrintScaleMode selectedScaleMode = PrintScaleMode.ShrinkToFit;

    public IReadOnlyList<PrintScaleMode> ScaleModes { get; } = Enum.GetValues<PrintScaleMode>();

    [RelayCommand]
    private void PrintVisual(Visual? visual)
    {
        if (visual is null)
        {
            return;
        }

        var settings = CreateSettings();
        var result = printService.Print(visual, settings);
        AppendLog($"Print: {result}");
    }

    [RelayCommand]
    private void PrintPreviewVisual(Visual? visual)
    {
        if (visual is null)
        {
            return;
        }

        var settings = CreateSettings();
        var result = printService.PrintWithPreview(visual, settings);
        AppendLog($"Preview: {result}");
    }

    [RelayCommand]
    private void PrintWithHeaders(Visual? visual)
    {
        if (visual is null)
        {
            return;
        }

        var settings = CreateSettings();
        settings.HeaderFooter = HeaderFooterSettings.CreateDefault("Sample Document");
        var result = printService.Print(visual, settings);
        AppendLog($"Print+Headers: {result}");
    }

    [RelayCommand]
    private void PreviewWithHeaders(Visual? visual)
    {
        if (visual is null)
        {
            return;
        }

        var settings = CreateSettings();
        settings.HeaderFooter = HeaderFooterSettings.CreateDefault("Sample Document");
        var result = printService.PrintWithPreview(visual, settings);
        AppendLog($"Preview+Headers: {result}");
    }

    [RelayCommand]
    private void PrintFlowDocument()
    {
        var document = CreateSampleFlowDocument();
        var settings = CreateSettings();
        var result = printService.PrintDocument(document, settings);
        AppendLog($"FlowDoc Print: {result}");
    }

    [RelayCommand]
    private void PreviewFlowDocument()
    {
        var document = CreateSampleFlowDocument();
        var settings = CreateSettings();
        settings.HeaderFooter = HeaderFooterSettings.CreateDefault("Flow Document Sample");
        var result = printService.PrintDocumentWithPreview(document, settings);
        AppendLog($"FlowDoc Preview: {result}");
    }

    private PrintSettings CreateSettings()
        => new()
        {
            Title = "Sample Print Job",
            Orientation = IsLandscape ? PrintOrientation.Landscape : PrintOrientation.Portrait,
            ScaleMode = SelectedScaleMode,
        };

    private static FlowDocument CreateSampleFlowDocument()
    {
        var document = new FlowDocument
        {
            FontFamily = new FontFamily("Segoe UI"),
            FontSize = 14,
        };

        document.Blocks.Add(new Paragraph(new Run("Sample Flow Document"))
        {
            FontSize = 24,
            FontWeight = FontWeights.Bold,
        });

        document.Blocks.Add(new Paragraph(new Run(
            "This is a sample FlowDocument created for demonstrating the print service. " +
            "FlowDocuments support rich text formatting, inline images, lists, tables, and more.")));

        var list = new List();
        list.ListItems.Add(new ListItem(new Paragraph(new Run("Multi-page pagination"))));
        list.ListItems.Add(new ListItem(new Paragraph(new Run("Headers and footers with page numbers"))));
        list.ListItems.Add(new ListItem(new Paragraph(new Run("Portrait and landscape orientation"))));
        list.ListItems.Add(new ListItem(new Paragraph(new Run("Configurable margins and scaling"))));
        document.Blocks.Add(list);

        document.Blocks.Add(new Paragraph(new Run(
            "The print framework clones the FlowDocument before printing to avoid modifying the original. " +
            "Page size, margins, and column width are applied to the clone, and a DocumentPaginator is " +
            "obtained for rendering.")));

        return document;
    }

    private void AppendLog(string entry)
        => ResultLog = $"[{DateTime.Now:T}] {entry}{Environment.NewLine}{ResultLog}";
}