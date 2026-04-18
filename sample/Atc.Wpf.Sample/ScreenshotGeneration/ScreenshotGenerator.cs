namespace Atc.Wpf.Sample.ScreenshotGeneration;

/// <summary>
/// Walks all sample TreeViews, instantiates controls, captures screenshots,
/// saves PNGs, and generates a markdown report.
/// </summary>
[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Screenshot generation should not throw.")]
[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
[SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Console logging only.")]
internal sealed class ScreenshotGenerator
{
    private static readonly Dictionary<string, string> TreeViewTypeToArea = new(StringComparer.Ordinal)
    {
        ["SamplesWpfTreeView"] = "Wpf",
        ["SamplesWpfControlsTreeView"] = "Wpf.Controls",
        ["SamplesWpfFormsTreeView"] = "Wpf.Forms",
        ["SamplesWpfComponentsTreeView"] = "Wpf.Components",
        ["SamplesWpfNetworkTreeView"] = "Wpf.Network",
        ["SamplesWpfUndoRedoTreeView"] = "Wpf.UndoRedo",
        ["SamplesWpfThemingTreeView"] = "Wpf.Theming",
        ["SamplesWpfSourceGeneratorsTreeView"] = "Wpf.SourceGenerators",
        ["SamplesWpfFontIconsTreeView"] = "Wpf.FontIcons",
    };

    private static readonly HashSet<string> SkipPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "DialogBoxes.StandardDialogBoxView",
        "Dialogs.DialogServiceView",
        "Window.NiceWindowView",
        "Window.NiceWindowWithFlyoutView",
        "Window.NiceDialogBoxView",
        "Window.NiceDialogBoxWithFlyoutView",
        "Network.NetworkScannerSampleView",
    };

    private readonly CaptureService captureService = new();
    private readonly string solutionRoot;

    public ScreenshotGenerator(string solutionRoot)
    {
        this.solutionRoot = solutionRoot;
    }

    public void GenerateAll(TreeView[] treeViews)
    {
        var items = CollectScreenshotItems(treeViews);
        System.Console.WriteLine($"[Screenshots] Found {items.Count} controls to capture.");

        var results = new List<(ScreenshotItem Item, CaptureResult? Result, string? SkipReason)>();
        var entryAssembly = Assembly.GetEntryAssembly()!;

        foreach (var item in items)
        {
            System.Console.Write($"  [{item.AssemblyArea}] {item.Category}/{item.ControlName} ... ");

            if (SkipPaths.Contains(item.SamplePath))
            {
                System.Console.WriteLine("SKIPPED (in skip list)");
                results.Add((item, null, "Skipped (in skip list)"));
                continue;
            }

            try
            {
                var result = CaptureControl(entryAssembly, item);
                results.Add(result);
                if (result.Result is { IsSuccess: true })
                {
                    System.Console.WriteLine($"OK ({result.Result.Width}x{result.Result.Height})");
                }
                else
                {
                    System.Console.WriteLine($"FAILED: {result.SkipReason ?? result.Result?.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"ERROR: {ex.Message}");
                results.Add((item, null, ex.Message));
            }
        }

        var reportPath = Path.Combine(solutionRoot, "docs", "controls-screenshots.md");
        ScreenshotReport.Generate(reportPath, results);

        var succeeded = results.Count(r => r.Result is { IsSuccess: true });
        var failed = results.Count - succeeded;
        System.Console.WriteLine();
        System.Console.WriteLine($"[Screenshots] Done. {succeeded} captured, {failed} failed/skipped.");
        System.Console.WriteLine($"[Screenshots] Report: {reportPath}");
    }

    private (ScreenshotItem Item, CaptureResult? Result, string? SkipReason) CaptureControl(
        Assembly entryAssembly,
        ScreenshotItem item)
    {
        var sampleType = GetTypeBySamplePath(entryAssembly, item.SamplePath);
        if (sampleType is null)
        {
            return (item, null, $"Type not found for path '{item.SamplePath}'");
        }

        if (Activator.CreateInstance(sampleType) is not UIElement control)
        {
            return (item, null, $"Could not create instance of '{sampleType.FullName}'");
        }

        // Host in a container with theme background
        var container = new Border
        {
            Background = Application.Current.TryFindResource("MahApps.Brushes.ThemeBackground") as Brush
                          ?? Brushes.White,
            Child = control,
            Width = 1000,
        };

        // Measure and arrange
        container.Measure(new Size(1000, 2000));
        var desiredHeight = container.DesiredSize.Height;

        if (desiredHeight < 1)
        {
            desiredHeight = 600;
        }

        container.Arrange(new Rect(0, 0, 1000, desiredHeight));
        container.UpdateLayout();

        // Flush dispatcher to apply templates
        FlushDispatcher();

        // Prefer capturing just the UsageArea element if present
        var captureTarget = LogicalTreeHelper.FindLogicalNode(control, "UsageArea") as Visual
                            ?? (Visual)container;

        // Check for empty bounds
        var bounds = VisualTreeHelper.GetDescendantBounds(captureTarget);
        if (bounds.IsEmpty || bounds.Width < 1 || bounds.Height < 1)
        {
            return (item, null, "Empty layout bounds");
        }

        var bitmap = captureService.CaptureVisual(captureTarget);

        var outputPath = Path.Combine(
            solutionRoot,
            "docs",
            "images",
            "screenshots",
            item.AssemblyArea,
            item.Category,
            $"{item.ControlName}.png");

        var result = captureService.SaveToFile(bitmap, outputPath);
        return (item, result, null);
    }

    private static List<ScreenshotItem> CollectScreenshotItems(
        TreeView[] treeViews)
    {
        var items = new List<ScreenshotItem>();

        foreach (var treeView in treeViews)
        {
            var typeName = treeView.GetType().Name;
            if (!TreeViewTypeToArea.TryGetValue(typeName, out var assemblyArea))
            {
                assemblyArea = typeName;
            }

            CollectFromItems(treeView.Items, assemblyArea, category: string.Empty, items);
        }

        return items;
    }

    private static void CollectFromItems(
        ItemCollection itemCollection,
        string assemblyArea,
        string category,
        List<ScreenshotItem> items)
    {
        foreach (var item in itemCollection)
        {
            if (item is SampleTreeViewItem sampleItem && !string.IsNullOrEmpty(sampleItem.SamplePath))
            {
                var controlName = sampleItem.Header?.ToString() ?? sampleItem.SamplePath;
                items.Add(new ScreenshotItem(
                    assemblyArea,
                    category,
                    controlName,
                    sampleItem.SamplePath));
            }
            else if (item is TreeViewItem treeViewItem)
            {
                var subCategory = treeViewItem.Header?.ToString() ?? category;
                CollectFromItems(treeViewItem.Items, assemblyArea, subCategory, items);
            }
        }
    }

    private static Type? GetTypeBySamplePath(
        Assembly entryAssembly,
        string samplePath)
    {
        var sampleType = entryAssembly
            .GetExportedTypes()
            .FirstOrDefault(x => x.FullName is not null && x.FullName.EndsWith(
                samplePath,
                StringComparison.Ordinal));

        if (sampleType is not null)
        {
            return sampleType;
        }

        var assemblyStartName = entryAssembly
            .GetName()
            .Name!
            .Split('.')[0];

        foreach (var assembly in AppDomain.CurrentDomain
                     .GetAssemblies()
                     .Where(x => !x.IsDynamic &&
                                 x.FullName!.StartsWith(
                                     assemblyStartName,
                                     StringComparison.Ordinal)))
        {
            sampleType = assembly
                .GetExportedTypes()
                .FirstOrDefault(x => x.FullName is not null && x.FullName.EndsWith(
                    samplePath,
                    StringComparison.Ordinal));

            if (sampleType is not null)
            {
                return sampleType;
            }
        }

        return null;
    }

    private static void FlushDispatcher()
    {
        var frame = new DispatcherFrame();
        _ = Dispatcher.CurrentDispatcher.BeginInvoke(
            DispatcherPriority.ContextIdle,
            () => frame.Continue = false);
        Dispatcher.PushFrame(frame);
    }
}