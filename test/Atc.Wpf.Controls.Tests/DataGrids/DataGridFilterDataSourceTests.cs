namespace Atc.Wpf.Controls.Tests.DataGrids;

using static DataGridFilterTestData;

/// <summary>
/// Filtering grids whose rows are not plain objects: a <see cref="DataTable"/> (a <see cref="BindingListCollectionView"/>
/// over a <see cref="DataView"/>), <see cref="ExpandoObject"/> rows, and a source that cannot be filtered.
/// </summary>
[Collection(nameof(PackResourceTestGroup))]
public sealed class DataGridFilterDataSourceTests : IDisposable
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
    public void A_data_table_grid_lists_the_real_distinct_values()
    {
        using var table = CreateTable();
        var grid = CreateGrid(table.DefaultView);

        Assert.Equal(["Closed", "Open", "Pending"], DataGridFilter.GetDistinctValues(grid, Column(grid, "Status")).Select(x => x.Text), StringComparer.Ordinal);
        Assert.Equal(["2.00", "10.00", "75.00", "300.00", "1,234.50"], DataGridFilter.GetDistinctValues(grid, Column(grid, "Amount")).Select(x => x.Text), StringComparer.Ordinal);
        Assert.Equal(["Alice", "alice", "Carol", "O'Brien", string.Empty], DataGridFilter.GetDistinctValues(grid, Column(grid, "Customer")).Select(x => x.Text), StringComparer.Ordinal);
    }

    [StaFact]
    public void A_data_table_grid_filters_its_rows_and_clearing_restores_them()
    {
        using var table = CreateTable();
        var grid = CreateGrid(table.DefaultView);

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));
        Assert.Equal([1, 3, 5], Ids(grid));
        Assert.True(DataGridFilter.GetIsColumnFilterActive(Column(grid, "Status")));

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Customer", ["Alice", string.Empty]));
        Assert.Equal([1, 3], Ids(grid));

        DataGridFilter.ClearAllFilters(grid);
        Assert.Equal([1, 2, 3, 4, 5, 6], Ids(grid));
        Assert.True(string.IsNullOrEmpty(table.DefaultView.RowFilter));
    }

    [StaFact]
    public void A_data_table_grid_filters_formatted_dates_with_a_time_of_day()
    {
        using var table = CreateTable();
        var grid = CreateGrid(table.DefaultView);

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Date", ["2024-01-05", "2024-03-15"]));

        Assert.Equal([1, 4, 5], Ids(grid));
    }

    [StaFact]
    public void A_data_table_grid_applies_text_and_range_filters_and_the_search()
    {
        using var table = CreateTable();
        var grid = CreateGrid(table.DefaultView);

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForText("Customer", DataGridTextFilterOperator.Contains, "AL"));
        Assert.Equal([1, 6], Ids(grid));

        DataGridFilter.ClearAllFilters(grid);
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForNumberRange("Amount", 10, 1000));
        Assert.Equal([1, 4, 6], Ids(grid));

        DataGridFilter.ClearAllFilters(grid);
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForDateRange("Date", new DateTime(2024, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Unspecified)));
        Assert.Equal([2, 3, 4, 5], Ids(grid));

        DataGridFilter.ClearAllFilters(grid);
        DataGridFilter.SetSearchText(grid, "o'b");
        Assert.Equal([2], Ids(grid));
        DataGridFilter.SetSearchText(grid, "denm");
        Assert.Equal([1, 3], Ids(grid));
    }

    [StaFact]
    public void A_data_table_grid_keeps_the_consumer_row_filter_and_gives_it_back()
    {
        using var table = CreateTable();
        table.DefaultView.RowFilter = "Country <> 'Sweden'";
        var grid = CreateGrid(table.DefaultView);

        Assert.Equal(["Closed", "Open"], DataGridFilter.GetDistinctValues(grid, Column(grid, "Status")).Select(x => x.Text), StringComparer.Ordinal);

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Closed"]));
        Assert.Equal([2], Ids(grid));

        DataGridFilter.ClearAllFilters(grid);
        Assert.Equal("Country <> 'Sweden'", table.DefaultView.RowFilter);
        Assert.Equal([1, 2, 3, 5], Ids(grid));
    }

    [StaFact]
    public void A_data_table_grid_lists_the_values_that_pass_the_other_columns_only()
    {
        using var table = CreateTable();
        var grid = CreateGrid(table.DefaultView);
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Country", ["Germany"]));

        Assert.Equal(["Closed", "Open"], DataGridFilter.GetDistinctValues(grid, Column(grid, "Status")).Select(x => x.Text), StringComparer.Ordinal);
        Assert.Equal(["Denmark", "Germany"], DataGridFilter.GetDistinctValues(grid, Column(grid, "Country")).Select(x => x.Text), StringComparer.Ordinal);
        Assert.Equal([5], Ids(grid));
    }

    [StaFact]
    public void A_data_table_grid_restores_its_filters_from_a_saved_layout()
    {
        using var table = CreateTable();
        var grid = CreateGrid(table.DefaultView);
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));
        var layout = DataGridFilter.SaveLayout(grid);
        DataGridFilter.ClearAllFilters(grid);
        Assert.Equal(6, Ids(grid).Count);

        DataGridFilter.LoadLayout(grid, layout);

        Assert.Equal([1, 3, 5], Ids(grid));
        Assert.True(DataGridFilter.GetIsColumnFilterActive(Column(grid, "Status")));
    }

    [StaFact]
    public void Expando_rows_are_read_and_filtered()
    {
        var rows = Orders()
            .Select(order =>
            {
                IDictionary<string, object?> row = new ExpandoObject();
                row["Id"] = order.Id;
                row["Customer"] = order.Customer;
                row["Country"] = order.Country;
                row["Status"] = order.Status;
                row["Amount"] = order.Amount;
                row["Date"] = order.Date;
                return (ExpandoObject)row;
            })
            .ToList();
        var grid = CreateGrid(rows);

        Assert.Equal(["Closed", "Open", "Pending"], DataGridFilter.GetDistinctValues(grid, Column(grid, "Status")).Select(x => x.Text), StringComparer.Ordinal);

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Pending"]));

        Assert.Equal([4], grid.Items.OfType<IDictionary<string, object?>>().Select(x => (int)x["Id"]!).ToList());
    }

    [StaFact]
    public void A_source_that_cannot_be_filtered_disables_the_buttons_and_shows_no_active_filter()
    {
        var grid = CreateGrid(new System.ComponentModel.BindingList<TestOrder>(Orders()));
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

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));
        DataGridFilter.OpenColumnFilter(grid, Column(grid, "Status"));

        Assert.False(DataGridFilter.GetIsColumnFilterActive(Column(grid, "Status")));
        Assert.Null(DataGridFilter.GetOpenPopup(grid));
        Assert.False(DataGridFilter.HandleKey(grid, Key.System, Key.Down, grid));
        Assert.Equal(6, grid.Items.Count);

        var button = FindButton(grid, Column(grid, "Status"));
        Assert.True(button.IsVisible);
        Assert.False(button.IsEnabled);
        Assert.Equal(Atc.Wpf.Resources.Miscellaneous.FilterNotSupported, button.ToolTip);
        Assert.True(ToolTipService.GetShowOnDisabled(button));
    }

    private static DataGrid CreateGrid(IEnumerable itemsSource)
    {
        var grid = new DataGrid
        {
            AutoGenerateColumns = false,
            CanUserAddRows = false,
            Language = XmlLanguage.GetLanguage("en-US"),
            ItemsSource = itemsSource,
        };

        grid.Columns.Add(new DataGridTextColumn { Header = "Id", Binding = new Binding("Id") });
        grid.Columns.Add(new DataGridTextColumn { Header = "Customer", Binding = new Binding("Customer") });
        grid.Columns.Add(new DataGridTextColumn { Header = "Country", Binding = new Binding("Country") });
        grid.Columns.Add(new DataGridTextColumn { Header = "Status", Binding = new Binding("Status") });
        grid.Columns.Add(new DataGridTextColumn { Header = "Amount", Binding = new Binding("Amount") { StringFormat = "N2" } });
        grid.Columns.Add(new DataGridTextColumn { Header = "Date", Binding = new Binding("Date") { StringFormat = "{0:yyyy-MM-dd}" } });

        DataGridFilter.SetSearchDelay(grid, TimeSpan.Zero);
        DataGridFilter.SetIsEnabled(grid, value: true);
        return grid;
    }

    private static DataTable CreateTable()
    {
        var table = new DataTable("Orders");
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Customer", typeof(string));
        table.Columns.Add("Country", typeof(string));
        table.Columns.Add("Status", typeof(string));
        table.Columns.Add("Amount", typeof(decimal));
        table.Columns.Add("Date", typeof(DateTime));
        foreach (var order in Orders())
        {
            table.Rows.Add(order.Id, (object?)order.Customer ?? DBNull.Value, order.Country, order.Status, order.Amount, order.Date);
        }

        // A customer named with an apostrophe checks the quoting of the row filter.
        table.Rows[1]["Customer"] = "O'Brien";
        table.AcceptChanges();
        return table;
    }

    private static List<int> Ids(DataGrid grid)
        => grid.Items.OfType<DataRowView>().Select(x => (int)x["Id"]).ToList();

    private static DataGridFilterButton FindButton(
        DependencyObject root,
        DataGridColumn column)
    {
        var stack = new Stack<DependencyObject>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current is DataGridFilterHeader header &&
                header.Button.Column is { } buttonColumn &&
                ReferenceEquals(buttonColumn, column))
            {
                return header.Button;
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++)
            {
                stack.Push(VisualTreeHelper.GetChild(current, i));
            }
        }

        throw new InvalidOperationException("No filter button for the column.");
    }
}