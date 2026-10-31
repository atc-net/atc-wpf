namespace Atc.Wpf.Controls.Tests.DataGrids;

using static DataGridFilterTestData;

public sealed class DataGridFilterLayoutTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Save_and_load_round_trip_order_widths_visibility_sorting_and_filters()
    {
        var source = CreateGrid();
        Column(source, "Status").DisplayIndex = 0;
        Column(source, "Customer").Width = new DataGridLength(140);
        Column(source, "Country").Width = new DataGridLength(2, DataGridLengthUnitType.Star);
        Column(source, "City").Visibility = Visibility.Collapsed;
        source.Items.SortDescriptions.Add(new SortDescription("Amount", ListSortDirection.Descending));
        DataGridFilter.SetColumnFilter(source, DataGridColumnFilter.ForValues("Status", ["Open", "Closed"]));
        DataGridFilter.SetColumnFilter(source, DataGridColumnFilter.ForNumberRange("Amount", 2, 100));

        var json = DataGridFilter.SaveLayout(source);

        var target = CreateGrid();
        Assert.True(DataGridFilter.LoadLayout(target, json));

        Assert.Equal(0, Column(target, "Status").DisplayIndex);
        Assert.Equal(1, Column(target, "Id").DisplayIndex);
        Assert.Equal(new DataGridLength(140), Column(target, "Customer").Width);
        Assert.Equal(new DataGridLength(2, DataGridLengthUnitType.Star), Column(target, "Country").Width);
        Assert.Equal(Visibility.Collapsed, Column(target, "City").Visibility);
        var sort = Assert.Single(target.Items.SortDescriptions);
        Assert.Equal("Amount", sort.PropertyName);
        Assert.Equal(ListSortDirection.Descending, sort.Direction);
        Assert.Equal(ListSortDirection.Descending, Column(target, "Amount").SortDirection);
        Assert.Null(Column(target, "Id").SortDirection);
        var ids = VisibleIds(target);
        Assert.Equal([6, 1], ids.Take(2));
        Assert.Equal([2, 5], ids.Skip(2).Order());
        Assert.True(DataGridFilter.GetIsColumnFilterActive(Column(target, "Amount")));
        Assert.Equal(2, DataGridFilter.GetColumnFilters(target).Count);
    }

    [StaFact]
    public void Saved_layout_is_versioned_json()
    {
        var json = DataGridFilter.SaveLayout(CreateGrid());

        using var document = JsonDocument.Parse(json);
        Assert.Equal(DataGridLayout.CurrentVersion, document.RootElement.GetProperty("version").GetInt32());
        Assert.Equal(7, document.RootElement.GetProperty("columns").GetArrayLength());
    }

    [StaFact]
    public void Unknown_columns_and_filters_are_ignored_and_missing_columns_keep_their_state()
    {
        const string json = """
            {
              "version": 1,
              "columns": [
                { "key": "Removed", "displayIndex": 0, "width": 50, "widthUnit": "Pixel", "isVisible": false },
                { "key": "Country", "displayIndex": 6, "width": 90, "widthUnit": "Pixel", "isVisible": true }
              ],
              "sorts": [ { "propertyName": "Customer", "direction": "Ascending" } ],
              "filters": [
                { "columnKey": "Removed", "kind": "Values", "values": [ "x" ] },
                { "columnKey": "Country", "kind": "Values", "values": [ "Sweden" ] }
              ]
            }
            """;
        var grid = CreateGrid();

        Assert.True(DataGridFilter.LoadLayout(grid, json));

        Assert.Equal(6, Column(grid, "Country").DisplayIndex);
        Assert.Equal(new DataGridLength(90), Column(grid, "Country").Width);
        Assert.All(grid.Columns.Where(x => (string)x.Header != "Country"), x => Assert.Equal(Visibility.Visible, x.Visibility));
        var filter = Assert.Single(DataGridFilter.GetColumnFilters(grid));
        Assert.Equal("Country", filter.ColumnKey);
        Assert.Equal([4, 6], VisibleIds(grid));
    }

    [StaFact]
    public void Older_layout_without_sorts_or_filters_loads()
    {
        const string json = """{ "version": 1, "columns": [ { "key": "Id", "displayIndex": 3, "width": 1, "widthUnit": "Auto", "isVisible": true } ] }""";
        var grid = CreateGrid();
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));

        Assert.True(DataGridFilter.LoadLayout(grid, json));

        Assert.Equal(3, Column(grid, "Id").DisplayIndex);
        Assert.Empty(DataGridFilter.GetColumnFilters(grid));
        Assert.Equal(6, grid.Items.Count);
    }

    [StaFact]
    public void Invalid_json_changes_nothing()
    {
        var grid = CreateGrid();
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));

        Assert.False(DataGridFilter.LoadLayout(grid, "{ not json"));
        Assert.False(DataGridFilter.LoadLayout(grid, string.Empty));

        Assert.Single(DataGridFilter.GetColumnFilters(grid));
    }

    [StaFact]
    public void Load_sorts_and_filters_in_one_refresh_and_raises_one_event()
    {
        var source = CreateGrid();
        source.Items.SortDescriptions.Add(new SortDescription("Amount", ListSortDirection.Ascending));
        DataGridFilter.SetColumnFilter(source, DataGridColumnFilter.ForValues("Status", ["Open"]));
        var json = DataGridFilter.SaveLayout(source);

        var orders = Orders();
        var target = CreateGrid(orders);
        var resets = 0;
        ((INotifyCollectionChanged)CollectionViewSource.GetDefaultView(orders)).CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                resets++;
            }
        };
        var events = 0;
        DataGridFilter.AddFilterChangedHandler(target, (_, _) => events++);

        DataGridFilter.LoadLayout(target, json);

        Assert.Equal(1, resets);
        Assert.Equal(1, events);
        Assert.Equal([5, 1, 3], VisibleIds(target));
    }
}