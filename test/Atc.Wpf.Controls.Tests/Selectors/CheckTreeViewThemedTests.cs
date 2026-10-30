namespace Atc.Wpf.Controls.Tests.Selectors;

[Collection(nameof(PackResourceTestGroup))]
public sealed class CheckTreeViewThemedTests : IDisposable
{
    public CheckTreeViewThemedTests()
    {
        // pack://application URIs need the Application type initialized.
        _ = typeof(Atc.Wpf.MarkupExtensions.StaticResourceExtension).Assembly;
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
    }

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void With_the_themed_tree_styles_10000_nodes_stay_virtualized_also_with_a_filter_matching_thousands()
    {
        var sut = new CheckTreeView
        {
            Height = 300,
            Nodes = CheckTreeStateTests.CreateLargeTree(10, 30, 33),
        };

        var window = new Window
        {
            Resources = CreateThemedTreeResources(),
            Content = sut,
            Width = 300,
            Height = 400,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };
        window.Show();
        try
        {
            window.UpdateLayout();
            var firstRow = (TreeViewItem)sut.PartTree.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.NotNull(firstRow.Style);
            Assert.InRange(CountVisualChildren<TreeViewItem>(sut.PartTree), 1, 100);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            sut.FilterMode = TreeFilterMode.FlatResults;
            sut.FilterText = "Leaf";
            sut.FlushPendingFilter();
            window.UpdateLayout();
            stopwatch.Stop();

            Assert.Equal(9900, sut.ShownRoots.Count);
            Assert.InRange(CountVisualChildren<TreeViewItem>(sut.PartTree), 1, 100);
            Assert.InRange(stopwatch.ElapsedMilliseconds, 0, 3000);

            sut.FilterMode = TreeFilterMode.KeepAncestors;
            sut.FilterText = "Branch";
            sut.FlushPendingFilter();
            window.UpdateLayout();

            Assert.InRange(CountVisualChildren<TreeViewItem>(sut.PartTree), 1, 100);
        }
        finally
        {
            window.Close();
        }
    }

    private static ResourceDictionary CreateThemedTreeResources()
    {
        var treeStyles = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Theming;component/Styles/BaseControls/TreeView.xaml", UriKind.Absolute),
        };
        var resources = new ResourceDictionary();
        resources.MergedDictionaries.Add(treeStyles);
        resources[typeof(TreeView)] = treeStyles["AtcApps.Styles.TreeView"];
        resources[typeof(TreeViewItem)] = treeStyles["AtcApps.Styles.TreeViewItem"];
        return resources;
    }

    private static int CountVisualChildren<T>(DependencyObject parent)
        where T : DependencyObject
    {
        var count = 0;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T)
            {
                count++;
            }

            count += CountVisualChildren<T>(child);
        }

        return count;
    }
}