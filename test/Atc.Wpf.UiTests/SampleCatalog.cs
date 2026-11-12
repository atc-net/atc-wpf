namespace Atc.Wpf.UiTests;

/// <summary>
/// Reads every sample from the sample app's tree views (<c>Samples*TreeView.xaml</c>), so a new sample is
/// visited by <see cref="SampleSmokeTests"/> without any list to keep up to date.
/// </summary>
internal static class SampleCatalog
{
    private const string SampleProjectFolder = "sample/Atc.Wpf.Sample";
    private const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

    /// <summary>
    /// Gets every sample as (tree view automation id, header, sample path).
    /// </summary>
    /// <returns>
    /// The samples in the order of the tabs in <c>SamplesTabControl</c> (Wpf first, Wpf.UndoRedo last),
    /// and within a tab from the top of its tree to the bottom.
    /// </returns>
    public static IReadOnlyList<SampleEntry> Load()
    {
        var sampleFolder = Path.Combine(FindRepositoryRoot(), SampleProjectFolder);
        var mainWindowFile = Path.Combine(sampleFolder, "MainWindow.xaml");
        var treeNames = LoadTreeViewNames(mainWindowFile);
        var treeTypes = LoadTreeViewTypesInTabOrder(mainWindowFile);

        var treeFiles = Directory
            .EnumerateFiles(sampleFolder, "Samples*TreeView.xaml")
            .Select(Path.GetFileNameWithoutExtension)
            .ToHashSet(StringComparer.Ordinal);
        var treeFilesWithoutTab = treeFiles.Except(treeTypes, StringComparer.Ordinal).ToList();
        if (treeFilesWithoutTab.Count > 0)
        {
            throw new InvalidOperationException($"SamplesTabControl in MainWindow.xaml has no tab for {string.Join(", ", treeFilesWithoutTab)}, so its samples cannot be reached.");
        }

        var samples = new List<SampleEntry>();
        foreach (var treeType in treeTypes.Where(treeFiles.Contains))
        {
            var file = Path.Combine(sampleFolder, treeType + ".xaml");
            if (!treeNames.TryGetValue(treeType, out var treeName))
            {
                throw new InvalidOperationException($"MainWindow.xaml has no named {treeType}, so its samples cannot be reached.");
            }

            var document = System.Xml.Linq.XDocument.Load(file);
            samples.AddRange(document
                .Descendants()
                .Where(x => string.Equals(x.Name.LocalName, "SampleTreeViewItem", StringComparison.Ordinal))
                .Select(x => new SampleEntry(
                    treeName,
                    (string?)x.Attribute("Header") ?? string.Empty,
                    (string?)x.Attribute("SamplePath") ?? string.Empty))
                .Where(x => x.SamplePath.Length > 0));
        }

        return samples;
    }

    private static List<string> LoadTreeViewTypesInTabOrder(
        string mainWindowFile)
    {
        // <TabControl x:Name="SamplesTabControl"> <TabItem x:Name="TabWpfControls" Tag="WpfControls"> => SamplesWpfControlsTreeView
        var document = System.Xml.Linq.XDocument.Load(mainWindowFile);
        var tabControl = document
            .Descendants()
            .FirstOrDefault(x => string.Equals(x.Name.LocalName, "TabControl", StringComparison.Ordinal) &&
                                 string.Equals((string?)x.Attribute(System.Xml.Linq.XName.Get("Name", XamlNamespace)), "SamplesTabControl", StringComparison.Ordinal))
            ?? throw new InvalidOperationException("MainWindow.xaml has no TabControl named SamplesTabControl.");

        return tabControl
            .Elements()
            .Where(x => string.Equals(x.Name.LocalName, "TabItem", StringComparison.Ordinal))
            .Select(x => (string?)x.Attribute("Tag"))
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => $"Samples{x}TreeView")
            .ToList();
    }

    private static Dictionary<string, string> LoadTreeViewNames(
        string mainWindowFile)
    {
        // <sample:SamplesWpfControlsTreeView x:Name="StvSampleWpfControls" ... />
        var document = System.Xml.Linq.XDocument.Load(mainWindowFile);
        return document
            .Descendants()
            .Where(x => x.Name.LocalName.StartsWith("Samples", StringComparison.Ordinal) &&
                        x.Name.LocalName.EndsWith("TreeView", StringComparison.Ordinal))
            .Select(x => (Type: x.Name.LocalName, Name: (string?)x.Attribute(System.Xml.Linq.XName.Get("Name", XamlNamespace))))
            .Where(x => x.Name is not null)
            .ToDictionary(x => x.Type, x => x.Name!, StringComparer.Ordinal);
    }

    private static string FindRepositoryRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && dir.GetFiles("Atc.Wpf.slnx").Length == 0)
        {
            dir = dir.Parent;
        }

        return dir?.FullName
               ?? throw new DirectoryNotFoundException("Could not find the repository root (Atc.Wpf.slnx).");
    }
}