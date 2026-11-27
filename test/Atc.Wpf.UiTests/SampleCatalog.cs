namespace Atc.Wpf.UiTests;

/// <summary>
/// Reads every sample from the sample app's tree views (<c>Samples*TreeView.xaml</c>), so a new sample is
/// visited by <see cref="SampleSmokeTests"/> without any list to keep up to date.
/// </summary>
internal static class SampleCatalog
{
    private const string SampleProjectFolder = "sample/Atc.Wpf.Sample";

    /// <summary>
    /// Gets every sample as (tree view automation id, header, sample path).
    /// </summary>
    /// <returns>
    /// The samples in the order of the sample categories in <c>MainWindow.xaml</c> (Wpf first, Wpf.UndoRedo last),
    /// and within a tab from the top of its tree to the bottom.
    /// </returns>
    public static IReadOnlyList<SampleEntry> Load()
    {
        var sampleFolder = Path.Combine(FindRepositoryRoot(), SampleProjectFolder);
        var categories = LoadCategories(Path.Combine(sampleFolder, "MainWindow.xaml"));

        var treeFiles = Directory
            .EnumerateFiles(sampleFolder, "Samples*TreeView.xaml")
            .Select(Path.GetFileNameWithoutExtension)
            .ToHashSet(StringComparer.Ordinal);
        var treeFilesWithoutCategory = treeFiles
            .Except(categories.Select(x => x.TreeType), StringComparer.Ordinal)
            .ToList();
        if (treeFilesWithoutCategory.Count > 0)
        {
            throw new InvalidOperationException($"MainWindow.xaml has no SampleCategory for {string.Join(", ", treeFilesWithoutCategory)}, so its samples cannot be reached.");
        }

        var samples = new List<SampleEntry>();
        foreach (var (treeType, treeName) in categories.Where(x => treeFiles.Contains(x.TreeType)))
        {
            var document = System.Xml.Linq.XDocument.Load(Path.Combine(sampleFolder, treeType + ".xaml"));
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

    /// <summary>
    /// Reads the sample categories of <c>MainWindow.xaml</c> in their order:
    /// <c>&lt;atcSample:SampleCategory Key="WpfControls" TreeType="{x:Type sample:SamplesWpfControlsTreeView}" /&gt;</c>
    /// gives the tree type <c>SamplesWpfControlsTreeView</c> and the tree automation id <c>StvSamplesWpfControls</c>.
    /// </summary>
    private static List<(string TreeType, string TreeName)> LoadCategories(
        string mainWindowFile)
    {
        var categories = System.Xml.Linq.XDocument
            .Load(mainWindowFile)
            .Descendants()
            .Where(x => string.Equals(x.Name.LocalName, "SampleCategory", StringComparison.Ordinal))
            .Select(x => (
                TreeType: TreeTypeName((string?)x.Attribute("TreeType")),
                TreeName: "StvSamples" + (string?)x.Attribute("Key")))
            .ToList();

        return categories.Count > 0
            ? categories
            : throw new InvalidOperationException("MainWindow.xaml has no SampleCategory.");
    }

    private static string TreeTypeName(string? treeType)
    {
        // "{x:Type sample:SamplesWpfControlsTreeView}" => "SamplesWpfControlsTreeView"
        var name = (treeType ?? string.Empty).Trim('{', '}').Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? string.Empty;
        return name[(name.IndexOf(':', StringComparison.Ordinal) + 1)..];
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