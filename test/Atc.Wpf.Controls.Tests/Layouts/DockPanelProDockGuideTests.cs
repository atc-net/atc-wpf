namespace Atc.Wpf.Controls.Tests.Layouts;

[Collection(nameof(PackResourceTestGroup))]
public sealed class DockPanelProDockGuideTests : IDisposable
{
    public DockPanelProDockGuideTests()
    {
        // pack://application URIs need the Application type initialized.
        _ = typeof(Atc.Wpf.MarkupExtensions.StaticResourceExtension).Assembly;
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
    }

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Only_expanded_regions_with_panes_are_targets_in_a_compass_in_the_middle()
    {
        var (panel, _, _) = DockPanelProPaneTests.CreatePanel();
        var host = DockPanelProFloatingWindowTests.Show(panel);
        try
        {
            var targets = panel.GetDockGuideTargets();

            Assert.Equal([DockPosition.Right, DockPosition.Bottom], targets.Keys.OrderBy(x => x));
            var middle = new Point(panel.ActualWidth / 2, panel.ActualHeight / 2);
            Assert.True(targets[DockPosition.Right].Left > middle.X);
            Assert.True(targets[DockPosition.Bottom].Top > middle.Y);
            Assert.Null(panel.HitTestDockGuide(middle));
            Assert.Equal(DockPosition.Right, panel.HitTestDockGuide(Center(targets[DockPosition.Right])));
        }
        finally
        {
            host.Close();
        }
    }

    [StaFact]
    public void Releasing_a_floating_pane_over_a_target_docks_it_in_that_region()
    {
        var (panel, _, right) = DockPanelProPaneTests.CreatePanel();
        var host = DockPanelProFloatingWindowTests.Show(panel);
        try
        {
            var output = panel.FindPane("output")!;
            panel.FloatPane(output);
            var window = panel.GetFloatingWindow(output)!;
            var target = Center(panel.GetDockGuideTargets()[DockPosition.Right]);

            panel.UpdateDockGuides(output, target);

            Assert.True(panel.IsShowingDockGuides);
            Assert.Equal(DockPosition.Right, panel.HighlightedDockGuide);

            Assert.True(panel.CompleteDockDrag(output, target));

            Assert.False(panel.IsShowingDockGuides);
            Assert.Equal(DockPaneState.Docked, output.State);
            Assert.Same(right, output.Region);
            Assert.Same(output, right.ActivePane);
            Assert.Null(panel.GetFloatingWindow(output));
            Assert.False(window.IsVisible);
        }
        finally
        {
            panel.RedockAllPanes();
            host.Close();
        }
    }

    [StaFact]
    public void Releasing_a_floating_pane_away_from_the_targets_keeps_it_floating()
    {
        var (panel, _, _) = DockPanelProPaneTests.CreatePanel();
        var host = DockPanelProFloatingWindowTests.Show(panel);
        try
        {
            var output = panel.FindPane("output")!;
            panel.FloatPane(output);

            panel.UpdateDockGuides(output, new Point(5, 5));

            Assert.True(panel.IsShowingDockGuides);
            Assert.Null(panel.HighlightedDockGuide);
            Assert.False(panel.CompleteDockDrag(output, new Point(5, 5)));
            Assert.False(panel.IsShowingDockGuides);
            Assert.Equal(DockPaneState.Floating, output.State);
            Assert.NotNull(panel.GetFloatingWindow(output));
        }
        finally
        {
            panel.RedockAllPanes();
            host.Close();
        }
    }

    private static Point Center(Rect bounds)
        => new(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height / 2));
}