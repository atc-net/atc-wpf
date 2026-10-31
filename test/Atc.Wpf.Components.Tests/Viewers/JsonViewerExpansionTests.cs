namespace Atc.Wpf.Components.Tests.Viewers;

[Collection(nameof(PackResourceTestGroup))]
public sealed class JsonViewerExpansionTests
{
    [StaFact]
    public void Load_LargeDocument_RealizesOnlyTheVisibleRows()
    {
        var json = "[" + string.Join(",", Enumerable.Range(0, 50_000).Select(i => $"{{\"id\":{i},\"name\":\"n{i}\"}}")) + "]";
        var sut = new JsonViewer { SuppressErrorMessages = true };
        var window = CreateWindow(sut);
        window.Show();
        try
        {
            sut.Data = json;
            window.UpdateLayout();

            var realized = CountTreeViewItems(sut.JsonTreeView);
            Assert.InRange(realized, 2, 300);

            var root = (TreeViewItem)sut.JsonTreeView.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.True(root.IsExpanded);
            var firstChild = (TreeViewItem)root.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.False(firstChild.IsExpanded);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void ThemeModeChanged_KeepsTheParsedDocumentAndTheExpandState()
    {
        var sut = new JsonViewer
        {
            SuppressErrorMessages = true,
            ThemeMode = ThemeMode.Light,
        };
        var window = CreateWindow(sut);
        window.Show();
        try
        {
            sut.Data = """{"a":{"x":1},"b":{"y":2}}""";
            window.UpdateLayout();
            var rootNode = sut.JsonTreeView.Items[0];
            var root = (TreeViewItem)sut.JsonTreeView.ItemContainerGenerator.ContainerFromIndex(0);
            ((TreeViewItem)root.ItemContainerGenerator.ContainerFromIndex(0)).IsExpanded = true;
            root.IsExpanded = true;

            sut.ThemeMode = ThemeMode.Dark;
            window.UpdateLayout();

            Assert.Same(rootNode, sut.JsonTreeView.Items[0]);
            root = (TreeViewItem)sut.JsonTreeView.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.True(root.IsExpanded);
            Assert.True(((TreeViewItem)root.ItemContainerGenerator.ContainerFromIndex(0)).IsExpanded);
            Assert.False(((TreeViewItem)root.ItemContainerGenerator.ContainerFromIndex(1)).IsExpanded);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void ThemeModeChanged_CollapsedRootStaysCollapsed()
    {
        var sut = new JsonViewer
        {
            SuppressErrorMessages = true,
            ThemeMode = ThemeMode.Light,
        };
        var window = CreateWindow(sut);
        window.Show();
        try
        {
            sut.Data = """{"a":1}""";
            window.UpdateLayout();
            ((TreeViewItem)sut.JsonTreeView.ItemContainerGenerator.ContainerFromIndex(0)).IsExpanded = false;

            sut.ThemeMode = ThemeMode.Dark;
            window.UpdateLayout();

            Assert.False(((TreeViewItem)sut.JsonTreeView.ItemContainerGenerator.ContainerFromIndex(0)).IsExpanded);
        }
        finally
        {
            window.Close();
        }
    }

    private static Window CreateWindow(JsonViewer sut)
        => new()
        {
            Content = sut,
            Width = 800,
            Height = 600,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };

    private static int CountTreeViewItems(DependencyObject parent)
    {
        var count = 0;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is TreeViewItem)
            {
                count++;
            }

            count += CountTreeViewItems(child);
        }

        return count;
    }
}