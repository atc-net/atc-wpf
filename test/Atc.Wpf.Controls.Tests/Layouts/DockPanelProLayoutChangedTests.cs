namespace Atc.Wpf.Controls.Tests.Layouts;

[Collection(nameof(PackResourceTestGroup))]
public sealed class DockPanelProLayoutChangedTests : IDisposable
{
    public DockPanelProLayoutChangedTests()
    {
        // pack://application URIs need the Application type initialized.
        _ = typeof(Atc.Wpf.MarkupExtensions.StaticResourceExtension).Assembly;
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
    }

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Floating_a_pane_and_collapsing_a_region_raise_one_coalesced_LayoutChanged()
    {
        var (panel, bottom, _) = DockPanelProPaneTests.CreatePanel();
        var host = DockPanelProFloatingWindowTests.Show(panel);
        try
        {
            var count = 0;
            panel.LayoutChanged += (_, _) => count++;
            Assert.False(panel.IsLayoutChangePending);

            panel.FloatPane(panel.FindPane("output")!);
            bottom.IsCollapsible = true;
            bottom.IsExpanded = false;

            Assert.Equal(0, count);
            Assert.True(panel.IsLayoutChangePending);

            panel.FlushLayoutChanged();

            Assert.Equal(1, count);
            Assert.False(panel.IsLayoutChangePending);
        }
        finally
        {
            panel.RedockAllPanes();
            host.Close();
        }
    }

    [StaFact]
    public void Dock_close_restore_tab_and_splitter_changes_each_raise_LayoutChanged()
    {
        var (panel, bottom, right) = DockPanelProPaneTests.CreatePanel();
        var host = DockPanelProFloatingWindowTests.Show(panel);
        try
        {
            var count = 0;
            panel.LayoutChanged += (_, _) => count++;
            var errors = panel.FindPane("errors")!;
            var splitter = DockPanelProFloatingWindowTests
                .FindVisualChildren<Thumb>(panel)
                .First(x => ReferenceEquals(x.Tag, bottom));

            void AssertRaised(Action action)
            {
                var before = count;
                action();
                panel.FlushLayoutChanged();
                Assert.Equal(before + 1, count);
            }

            AssertRaised(() => panel.ClosePane(errors));
            AssertRaised(() => panel.RestorePane(errors));
            AssertRaised(() => panel.ActivatePane(panel.FindPane("find")!));
            AssertRaised(() => panel.MovePane(errors, right));
            AssertRaised(() => panel.FloatPane(errors));
            AssertRaised(() => panel.RedockPane(errors));
            AssertRaised(() => splitter.RaiseEvent(new DragCompletedEventArgs(0, -10, canceled: false)));
        }
        finally
        {
            panel.RedockAllPanes();
            host.Close();
        }
    }

    [StaFact]
    public void Moving_a_floating_window_raises_LayoutChanged()
    {
        var (panel, _, _) = DockPanelProPaneTests.CreatePanel();
        var host = DockPanelProFloatingWindowTests.Show(panel);
        try
        {
            var output = panel.FindPane("output")!;
            output.FloatingBounds = new Rect(100, 100, 300, 200);
            panel.FloatPane(output);
            panel.FlushLayoutChanged();
            var count = 0;
            panel.LayoutChanged += (_, _) => count++;

            panel.GetFloatingWindow(output)!.Left = 160;
            panel.FlushLayoutChanged();

            Assert.Equal(1, count);
        }
        finally
        {
            panel.RedockAllPanes();
            host.Close();
        }
    }

    [StaFact]
    public void Changes_before_the_panel_is_loaded_do_not_raise_LayoutChanged()
    {
        var (panel, bottom, _) = DockPanelProPaneTests.CreatePanel();
        var count = 0;
        panel.LayoutChanged += (_, _) => count++;

        bottom.IsCollapsible = true;
        bottom.IsExpanded = false;
        panel.ClosePane(panel.FindPane("errors")!);
        panel.FlushLayoutChanged();

        Assert.Equal(0, count);
    }

    [StaFact]
    public void AutoSaveLayout_saves_to_the_store_and_a_new_panel_with_the_same_LayoutId_loads_it()
    {
        var store = new MemoryLayoutStore();
        var (panel, _, _) = DockPanelProPaneTests.CreatePanel();
        panel.LayoutId = "ide";
        panel.AutoSaveLayout = true;
        panel.LayoutStore = store;
        var host = DockPanelProFloatingWindowTests.Show(panel);
        try
        {
            Assert.Equal(0, store.SaveCount);

            panel.ClosePane(panel.FindPane("errors")!);
            panel.FlushLayoutChanged();

            Assert.Equal(1, store.SaveCount);
            Assert.Contains("\"errors\"", store.Layouts["ide"], StringComparison.Ordinal);
        }
        finally
        {
            host.Close();
        }

        var (restored, bottom, _) = DockPanelProPaneTests.CreatePanel();
        restored.LayoutId = "ide";
        restored.AutoSaveLayout = true;
        restored.LayoutStore = store;
        var restoredHost = DockPanelProFloatingWindowTests.Show(restored);
        try
        {
            Assert.Equal(DockPaneState.Closed, restored.FindPane("errors")!.State);
            Assert.Equal("output,find", string.Join(",", bottom.DockedPanes.Select(x => x.PaneId)));
            Assert.False(restored.IsLayoutChangePending);
        }
        finally
        {
            restoredHost.Close();
        }
    }

    [StaFact]
    public void Closing_the_owner_saves_a_pending_change()
    {
        var store = new MemoryLayoutStore();
        var (panel, _, _) = DockPanelProPaneTests.CreatePanel();
        panel.LayoutId = "ide";
        panel.AutoSaveLayout = true;
        panel.LayoutStore = store;
        var host = DockPanelProFloatingWindowTests.Show(panel);

        panel.FloatPane(panel.FindPane("output")!);
        host.Close();

        Assert.Equal(1, store.SaveCount);
        Assert.Contains("\"state\": 1", store.Layouts["ide"], StringComparison.Ordinal);
    }

    [StaFact]
    public void Without_AutoSaveLayout_nothing_is_saved()
    {
        var store = new MemoryLayoutStore();
        var (panel, _, _) = DockPanelProPaneTests.CreatePanel();
        panel.LayoutId = "ide";
        panel.LayoutStore = store;
        var host = DockPanelProFloatingWindowTests.Show(panel);
        try
        {
            panel.ClosePane(panel.FindPane("errors")!);
            panel.FlushLayoutChanged();

            Assert.Equal(0, store.SaveCount);
        }
        finally
        {
            host.Close();
        }
    }

    [Fact]
    public void FileDockLayoutStore_round_trips_and_returns_null_for_an_unknown_id()
    {
        var directory = Path.Combine(Path.GetTempPath(), "atc-dock-" + Guid.NewGuid().ToString("N"));
        try
        {
            var store = new FileDockLayoutStore(directory);

            Assert.Null(store.Load("ide"));

            store.Save("ide:main/1", "{\"version\":2}");

            Assert.Equal("{\"version\":2}", store.Load("ide:main/1"));
            Assert.Single(Directory.GetFiles(directory));
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    private sealed class MemoryLayoutStore : IDockLayoutStore
    {
        public Dictionary<string, string> Layouts { get; } = new(StringComparer.Ordinal);

        public int SaveCount { get; private set; }

        public string? Load(string layoutId)
            => Layouts.GetValueOrDefault(layoutId);

        public void Save(
            string layoutId,
            string layout)
        {
            SaveCount++;
            Layouts[layoutId] = layout;
        }
    }
}