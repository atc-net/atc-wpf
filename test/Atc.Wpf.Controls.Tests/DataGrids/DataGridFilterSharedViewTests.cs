namespace Atc.Wpf.Controls.Tests.DataGrids;

using static DataGridFilterTestData;

/// <summary>
/// Grids that share one <c>ItemsSource</c> share its default collection view: their filters combine (AND) with each
/// other and with the consumer's own view filter.
/// </summary>
public sealed class DataGridFilterSharedViewTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Two_grids_on_one_collection_combine_their_filters_and_clear_one_by_one()
    {
        var orders = Orders();
        var view = CollectionViewSource.GetDefaultView(orders);
        Predicate<object> consumer = x => ((TestOrder)x).Id != 6;
        view.Filter = consumer;
        var a = CreateGrid(orders);
        var b = CreateGrid(orders);

        DataGridFilter.SetColumnFilter(a, DataGridColumnFilter.ForValues("Status", ["Open", "Closed"]));
        Assert.Equal([1, 2, 3, 5], ViewIds(view));

        DataGridFilter.SetColumnFilter(b, DataGridColumnFilter.ForValues("Country", ["Germany", "Sweden"]));
        Assert.Equal([2, 5], ViewIds(view));

        // Filtering A again used to adopt B's predicate as the consumer filter: A -> B -> A ... a stack overflow.
        DataGridFilter.SetColumnFilter(a, DataGridColumnFilter.ForValues("Status", ["Open"]));
        Assert.Equal([5], ViewIds(view));
        Assert.Equal([5], VisibleIds(a));
        Assert.Equal([5], VisibleIds(b));

        DataGridFilter.ClearAllFilters(a);
        Assert.Equal([2, 4, 5], ViewIds(view));

        DataGridFilter.ClearAllFilters(b);
        Assert.Equal([1, 2, 3, 4, 5], ViewIds(view));
        Assert.Same(consumer, view.Filter);
    }

    [StaFact]
    public void Three_grids_on_one_collection_combine_their_filters_in_any_order()
    {
        var orders = Orders();
        var view = CollectionViewSource.GetDefaultView(orders);
        var a = CreateGrid(orders);
        var b = CreateGrid(orders);
        var c = CreateGrid(orders);

        DataGridFilter.SetColumnFilter(a, DataGridColumnFilter.ForValues("Status", ["Open", "Closed"]));
        DataGridFilter.SetColumnFilter(b, DataGridColumnFilter.ForValues("Country", ["Denmark", "Germany", "Sweden"]));
        DataGridFilter.SetColumnFilter(a, DataGridColumnFilter.ForValues("Status", ["Open", "Closed", "Pending"]));
        DataGridFilter.SetColumnFilter(c, DataGridColumnFilter.ForNumberRange("Amount", 2, 100));
        DataGridFilter.SetColumnFilter(b, DataGridColumnFilter.ForValues("Country", ["Germany", "Sweden"]));
        Assert.Equal([2, 5, 6], ViewIds(view));

        DataGridFilter.SetSearchText(c, "er");
        Assert.Equal([2, 5], ViewIds(view));

        DataGridFilter.ClearAllFilters(b);
        Assert.Equal([2, 5], ViewIds(view));

        DataGridFilter.SetSearchText(c, "e");
        Assert.Equal([1, 2, 5, 6], ViewIds(view));

        DataGridFilter.SetSearchText(c, string.Empty);
        DataGridFilter.ClearAllFilters(c);
        Assert.Equal([1, 2, 3, 4, 5, 6], ViewIds(view));

        DataGridFilter.SetColumnFilter(c, DataGridColumnFilter.ForValues("Status", ["Pending"]));
        Assert.Equal([4], ViewIds(view));

        DataGridFilter.ClearAllFilters(c);
        DataGridFilter.ClearAllFilters(a);
        Assert.Equal([1, 2, 3, 4, 5, 6], ViewIds(view));
        Assert.Null(view.Filter);
    }

    [StaFact]
    public void Distinct_values_of_one_grid_honour_the_filters_of_the_other_grids_on_the_view()
    {
        var orders = Orders();
        var a = CreateGrid(orders);
        var b = CreateGrid(orders);

        DataGridFilter.SetColumnFilter(a, DataGridColumnFilter.ForValues("Country", ["Germany"]));

        var statuses = DataGridFilter.GetDistinctValues(b, Column(b, "Status"));

        Assert.Equal(["Closed", "Open"], statuses.Select(x => x.Text), StringComparer.Ordinal);
    }

    [StaFact]
    public void Disabling_one_grid_keeps_the_filter_of_the_other()
    {
        var orders = Orders();
        var view = CollectionViewSource.GetDefaultView(orders);
        var a = CreateGrid(orders);
        var b = CreateGrid(orders);
        DataGridFilter.SetColumnFilter(a, DataGridColumnFilter.ForValues("Status", ["Open"]));
        DataGridFilter.SetColumnFilter(b, DataGridColumnFilter.ForValues("Country", ["Germany"]));
        Assert.Equal([5], ViewIds(view));

        DataGridFilter.SetIsEnabled(a, value: false);
        Assert.Equal([2, 5], ViewIds(view));

        b.ItemsSource = null;
        Assert.Equal([1, 2, 3, 4, 5, 6], ViewIds(view));
        Assert.Null(view.Filter);
    }

    private static List<int> ViewIds(ICollectionView view)
        => view.Cast<TestOrder>().Select(x => x.Id).ToList();
}