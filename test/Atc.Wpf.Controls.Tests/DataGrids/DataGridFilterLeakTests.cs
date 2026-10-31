namespace Atc.Wpf.Controls.Tests.DataGrids;

using static DataGridFilterTestData;

/// <summary>
/// The filter predicate lives on the default collection view of the ItemsSource, which lives as long as the source.
/// A long-lived source (a cached or singleton view model) must not keep a dropped grid alive, and the grid must not
/// keep its source alive.
/// </summary>
public sealed class DataGridFilterLeakTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Filtered_grid_on_a_long_lived_source_can_be_collected()
    {
        var orders = Orders();

        var reference = CreateFilteredGrid(orders, search: false);

        Assert.True(ControlLeakProbe.IsCollected(reference));
        GC.KeepAlive(orders);
    }

    [StaFact]
    public void Searched_grid_with_a_pending_search_on_a_long_lived_source_can_be_collected_after_unload()
    {
        var orders = Orders();

        var reference = CreateFilteredGrid(orders, search: true);

        Assert.True(ControlLeakProbe.IsCollected(reference));
        GC.KeepAlive(orders);
    }

    [StaFact]
    public void Grid_whose_long_lived_sources_were_replaced_can_be_collected()
    {
        var first = Orders();
        var second = Orders();

        var reference = CreateGridWithReplacedSource(first, second);

        Assert.True(ControlLeakProbe.IsCollected(reference));
        GC.KeepAlive(first);
        GC.KeepAlive(second);
    }

    [StaFact]
    public void Dropped_grid_leaves_only_the_consumer_filter_on_the_shared_view()
    {
        var orders = Orders();
        var view = CollectionViewSource.GetDefaultView(orders);
        view.Filter = x => ((TestOrder)x).Amount < 100m;

        var reference = CreateFilteredGrid(orders, search: false);
        Assert.True(ControlLeakProbe.IsCollected(reference));

        view.Refresh();
        Assert.Equal([1, 2, 5, 6], view.Cast<TestOrder>().Select(x => x.Id));
    }

    [StaFact]
    public void Grid_still_follows_its_long_lived_source()
    {
        // Guards against "fixing" the leak by not filtering the shared view at all.
        var orders = Orders();
        var grid = CreateGrid(orders);

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));

        Assert.Equal(3, CollectionViewSource.GetDefaultView(orders).Cast<object>().Count());
        GC.KeepAlive(grid);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateFilteredGrid(
        List<TestOrder> orders,
        bool search)
    {
        var grid = CreateGrid(orders);
        DataGridFilter.SetShowColumnChooser(grid, value: true);
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));
        if (search)
        {
            DataGridFilter.SetSearchDelay(grid, TimeSpan.FromSeconds(30));
            DataGridFilter.SetSearchText(grid, "a");
        }

        grid.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
        grid.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));
        return new WeakReference(grid);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateGridWithReplacedSource(
        List<TestOrder> first,
        List<TestOrder> second)
    {
        var grid = CreateGrid(first);
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));
        DataGridFilter.SetSearchText(grid, "a");
        _ = DataGridFilter.GetDistinctValues(grid, Column(grid, "Country"));
        grid.ItemsSource = second;
        return new WeakReference(grid);
    }
}