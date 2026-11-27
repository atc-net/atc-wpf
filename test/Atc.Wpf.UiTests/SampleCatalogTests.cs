namespace Atc.Wpf.UiTests;

/// <summary>
/// Pure-logic tests for <see cref="SampleCatalog"/>; they read the sample app's XAML and do not launch it.
/// </summary>
public sealed class SampleCatalogTests
{
    [Fact]
    public void Load_returns_the_trees_in_the_order_of_the_sample_tabs()
    {
        var treeNames = SampleCatalog.Load()
            .Select(x => x.TreeName)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        Assert.Equal(
            [
                "StvSamplesWpf",
                "StvSamplesWpfComponents",
                "StvSamplesWpfControls",
                "StvSamplesWpfDisplays",
                "StvSamplesWpfFontIcons",
                "StvSamplesWpfForms",
                "StvSamplesWpfHardware",
                "StvSamplesWpfNetwork",
                "StvSamplesWpfSourceGenerators",
                "StvSamplesWpfTheming",
                "StvSamplesWpfUndoRedo",
            ],
            treeNames);
    }

    [Fact]
    public void Load_returns_the_samples_of_a_tree_from_top_to_bottom()
    {
        var networkHeaders = SampleCatalog.Load()
            .Where(x => string.Equals(x.TreeName, "StvSamplesWpfNetwork", StringComparison.Ordinal))
            .Select(x => x.Header)
            .ToList();

        Assert.Equal("NetworkScanner", networkHeaders[0]);
        Assert.True(
            networkHeaders.IndexOf("TcpClient") < networkHeaders.IndexOf("UdpClient"),
            "TcpClient is declared above UdpClient.");
    }
}