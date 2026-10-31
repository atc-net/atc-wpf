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
    /// <returns>The samples, in tree order.</returns>
    public static IReadOnlyList<SampleEntry> Load()
    {
        var sampleFolder = Path.Combine(FindRepositoryRoot(), SampleProjectFolder);
        var treeNames = LoadTreeViewNames(Path.Combine(sampleFolder, "MainWindow.xaml"));

        var samples = new List<SampleEntry>();
        foreach (var file in Directory.EnumerateFiles(sampleFolder, "Samples*TreeView.xaml").Order(StringComparer.Ordinal))
        {
            var treeType = Path.GetFileNameWithoutExtension(file);
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