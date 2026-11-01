namespace Atc.Wpf.Controls.Tests.DataGrids;

using static DataGridFilterTestData;

/// <summary>
/// A collection view refuses to filter, sort or refresh while an item is being edited or added. Filters applied
/// during such a transaction commit the grid's row edit first, or wait until the transaction ends.
/// </summary>
[Collection(nameof(PackResourceTestGroup))]
public sealed class DataGridFilterEditTransactionTests : IDisposable
{
    private readonly List<Window> windows = [];

    public void Dispose()
    {
        foreach (var window in windows)
        {
            window.Close();
        }

        Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();
    }

    [StaFact]
    public void Filter_set_during_an_edit_transaction_is_applied_when_the_edit_ends()
    {
        var orders = Orders();
        var grid = CreateGrid(orders);
        var view = (IEditableCollectionView)CollectionViewSource.GetDefaultView(orders);
        var events = new List<DataGridFilterChangedEventArgs>();
        DataGridFilter.AddFilterChangedHandler(grid, (_, e) => events.Add(e));
        view.EditItem(orders[1]);

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));

        Assert.True(DataGridFilter.GetIsColumnFilterActive(Column(grid, "Status")));
        Assert.Empty(events);

        view.CommitEdit();
        WaitUntil(() => events.Count > 0);

        Assert.Equal([1, 3, 5], VisibleIds(grid));
        Assert.Equal(3, Assert.Single(events).ItemCount);
    }

    [StaFact]
    public void Filter_set_during_an_add_transaction_is_applied_when_the_new_item_is_committed()
    {
        var orders = Orders();
        var grid = CreateGrid(orders);
        var view = (IEditableCollectionView)CollectionViewSource.GetDefaultView(orders);
        var added = (TestOrder)view.AddNew();
        added.Id = 7;
        added.Status = "Open";

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));

        view.CommitNew();
        WaitUntil(() => !DataGridFilter.IsApplyPending(grid));

        Assert.Equal([1, 3, 5, 7], VisibleIds(grid).Order());
    }

    [StaFact]
    public void Debounced_search_that_fires_during_an_add_transaction_waits_for_it()
    {
        var orders = Orders();
        var grid = CreateGrid(orders);
        DataGridFilter.SetSearchDelay(grid, TimeSpan.FromMilliseconds(20));
        var view = (IEditableCollectionView)CollectionViewSource.GetDefaultView(orders);
        var added = (TestOrder)view.AddNew();

        DataGridFilter.SetSearchText(grid, "Germany");
        WaitUntil(() => !DataGridFilter.IsSearchPending(grid));

        Assert.True(view.IsAddingNew);

        added.Id = 7;
        added.Country = "Germany";
        view.CommitNew();
        WaitUntil(() => !DataGridFilter.IsApplyPending(grid));

        Assert.Equal([2, 5, 7], VisibleIds(grid).Order());
    }

    [StaFact]
    public void Layout_loaded_during_an_edit_transaction_sorts_and_filters_when_the_edit_ends()
    {
        var source = CreateGrid();
        source.Items.SortDescriptions.Add(new SortDescription("Amount", ListSortDirection.Descending));
        DataGridFilter.SetColumnFilter(source, DataGridColumnFilter.ForValues("Status", ["Open"]));
        var json = DataGridFilter.SaveLayout(source);

        var orders = Orders();
        var target = CreateGrid(orders);
        var view = (IEditableCollectionView)CollectionViewSource.GetDefaultView(orders);
        view.EditItem(orders[0]);

        Assert.True(DataGridFilter.LoadLayout(target, json));

        view.CommitEdit();
        WaitUntil(() => !DataGridFilter.IsApplyPending(target));

        Assert.Equal([3, 1, 5], VisibleIds(target));
        Assert.Equal(ListSortDirection.Descending, Column(target, "Amount").SortDirection);
    }

    [StaFact]
    public void Disabling_during_an_edit_transaction_gives_the_consumer_filter_back_when_the_edit_ends()
    {
        var orders = Orders();
        var grid = CreateGrid(orders);
        var view = CollectionViewSource.GetDefaultView(orders);
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));
        ((IEditableCollectionView)view).EditItem(orders[0]);

        DataGridFilter.SetIsEnabled(grid, value: false);

        ((IEditableCollectionView)view).CommitEdit();
        WaitUntil(() => !DataGridFilter.IsApplyPending(grid));

        Assert.Null(view.Filter);
        Assert.Equal(6, grid.Items.Count);
    }

    [StaFact]
    public void Filter_set_while_the_grid_edits_a_cell_commits_the_row_and_applies_at_once()
    {
        var grid = CreateGrid();
        grid.CanUserAddRows = true;
        Show(grid);
        var view = (IEditableCollectionView)grid.Items;
        grid.CurrentCell = new DataGridCellInfo(grid.Items[1], Column(grid, "Customer"));
        Assert.True(grid.BeginEdit());
        Assert.True(view.IsEditingItem);

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));

        Assert.False(view.IsEditingItem);
        Assert.False(DataGridFilter.IsApplyPending(grid));
        Assert.Equal([1, 3, 5], VisibleIds(grid));
    }

    private static void WaitUntil(Func<bool> condition)
    {
        var stopwatch = Stopwatch.StartNew();
        while (!condition())
        {
            Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(5), "Timed out.");
            DispatcherHelper.DoEvents();
            Thread.Sleep(10);
        }

        DispatcherHelper.DoEvents();
    }

    private void Show(DataGrid grid)
    {
        var window = new Window
        {
            Content = grid,
            Width = 900,
            Height = 300,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };
        windows.Add(window);
        window.Show();
        window.UpdateLayout();
        DispatcherHelper.DoEvents();
    }
}