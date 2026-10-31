namespace Atc.Wpf.Controls.Tests.DataGrids;

using static DataGridFilterTestData;

public sealed class DataGridFilterTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Defaults_match_the_documented_api()
    {
        var grid = new DataGrid();
        var column = new DataGridTextColumn();

        Assert.False(DataGridFilter.GetIsEnabled(grid));
        Assert.False(DataGridFilter.GetShowColumnChooser(grid));
        Assert.Equal(string.Empty, DataGridFilter.GetSearchText(grid));
        Assert.Equal(TimeSpan.FromMilliseconds(300), DataGridFilter.GetSearchDelay(grid));
        Assert.True(DataGridFilter.GetIsColumnFilterEnabled(column));
        Assert.True(DataGridFilter.GetCanUserHideColumn(column));
        Assert.Null(DataGridFilter.GetColumnKey(column));
        Assert.False(DataGridFilter.GetIsColumnFilterActive(column));
        Assert.Empty(DataGridFilter.GetColumnFilters(grid));
    }

    [StaFact]
    public void Column_key_prefers_the_attached_key_then_sort_member_path_then_binding_then_header()
    {
        var keyed = new DataGridTextColumn { Header = "H", Binding = new Binding("A"), SortMemberPath = "S" };
        DataGridFilter.SetColumnKey(keyed, "K");
        var sorted = new DataGridTextColumn { Header = "H", Binding = new Binding("A"), SortMemberPath = "S" };
        var bound = new DataGridTextColumn { Header = "H", Binding = new Binding("A") };
        var template = new DataGridTemplateColumn { Header = "Header" };

        Assert.Equal("K", DataGridFilter.ResolveColumnKey(keyed));
        Assert.Equal("S", DataGridFilter.ResolveColumnKey(sorted));
        Assert.Equal("A", DataGridFilter.ResolveColumnKey(bound));
        Assert.Equal("Header", DataGridFilter.ResolveColumnKey(template));
    }

    [StaFact]
    public void Enabling_wraps_the_header_templates_and_disabling_restores_them()
    {
        var own = new DataTemplate();
        var grid = CreateGrid(isEnabled: false);
        var column = Column(grid, "Status");
        column.HeaderTemplate = own;

        DataGridFilter.SetIsEnabled(grid, value: true);

        Assert.NotSame(own, column.HeaderTemplate);
        Assert.NotNull(Column(grid, "Id").HeaderTemplate);

        DataGridFilter.SetIsEnabled(grid, value: false);

        Assert.Same(own, column.HeaderTemplate);
        Assert.Null(Column(grid, "Id").HeaderTemplate);
    }

    [StaFact]
    public void Distinct_values_are_formatted_sorted_by_raw_value_with_one_blanks_entry_last()
    {
        var grid = CreateGrid();

        var customers = DataGridFilter.GetDistinctValues(grid, Column(grid, "Customer"));
        var amounts = DataGridFilter.GetDistinctValues(grid, Column(grid, "Amount"));
        var dates = DataGridFilter.GetDistinctValues(grid, Column(grid, "Date"));
        var cities = DataGridFilter.GetDistinctValues(grid, Column(grid, "City"));

        Assert.Equal(["Alice", "alice", "Bob", "Carol", string.Empty], customers.Select(x => x.Text), StringComparer.Ordinal);
        Assert.Equal(Atc.Wpf.Resources.Miscellaneous.Blanks, customers[^1].DisplayText);
        Assert.Equal(["2.00", "10.00", "75.00", "300.00", "1,234.50"], amounts.Select(x => x.Text), StringComparer.Ordinal);
        Assert.Equal(["2024-01-05", "2024-01-06", "2024-02-01", "2024-03-15", "2024-04-01"], dates.Select(x => x.Text), StringComparer.Ordinal);
        Assert.Equal(["Aarhus", "Berlin", string.Empty], cities.Select(x => x.Text), StringComparer.Ordinal);
    }

    [StaFact]
    public void Values_filter_keeps_the_rows_with_a_listed_text_and_marks_the_column_active()
    {
        var grid = CreateGrid();

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open", "Pending"]));

        Assert.Equal([1, 3, 4, 5], VisibleIds(grid));
        Assert.True(DataGridFilter.GetIsColumnFilterActive(Column(grid, "Status")));
        Assert.False(DataGridFilter.GetIsColumnFilterActive(Column(grid, "Country")));
        var filter = Assert.Single(DataGridFilter.GetColumnFilters(grid));
        Assert.Equal("Status", filter.ColumnKey);
    }

    [StaFact]
    public void Blank_entry_matches_null_and_empty_cells()
    {
        var grid = CreateGrid();

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Customer", [string.Empty]));

        Assert.Equal([3, 4], VisibleIds(grid));
    }

    [StaFact]
    public void Formatted_values_filter_numbers_as_shown()
    {
        var grid = CreateGrid();

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Amount", ["1,234.50", "2.00"]));

        Assert.Equal([2, 3, 5], VisibleIds(grid));
    }

    [StaFact]
    public void Filters_of_several_columns_combine_with_and()
    {
        var grid = CreateGrid();

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Country", ["Denmark"]));

        Assert.Equal([1, 3], VisibleIds(grid));
        Assert.Equal(2, DataGridFilter.GetColumnFilters(grid).Count);
    }

    [StaFact]
    public void Consumer_filter_is_kept_and_anded_and_survives_clear_all()
    {
        var orders = Orders();
        var grid = CreateGrid(orders);
        var view = CollectionViewSource.GetDefaultView(orders);
        view.Filter = x => ((TestOrder)x).Amount < 100m;
        Assert.Equal([1, 2, 5, 6], VisibleIds(grid));

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));

        Assert.Equal([1, 5], VisibleIds(grid));

        DataGridFilter.ClearAllFilters(grid);

        Assert.Equal([1, 2, 5, 6], VisibleIds(grid));
        Assert.Empty(DataGridFilter.GetColumnFilters(grid));
        Assert.False(DataGridFilter.GetIsColumnFilterActive(Column(grid, "Status")));
    }

    [StaFact]
    public void Consumer_filter_set_after_the_behaviour_is_adopted_on_the_next_change()
    {
        var orders = Orders();
        var grid = CreateGrid(orders);
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open", "Closed"]));

        CollectionViewSource.GetDefaultView(orders).Filter = x => ((TestOrder)x).Country == "Germany";
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));

        Assert.Equal([5], VisibleIds(grid));
    }

    [StaFact]
    public void Disabling_gives_the_view_the_consumer_filter_back()
    {
        var orders = Orders();
        var grid = CreateGrid(orders);
        var view = CollectionViewSource.GetDefaultView(orders);
        Predicate<object> consumer = x => ((TestOrder)x).Amount < 100m;
        view.Filter = consumer;
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));

        DataGridFilter.SetIsEnabled(grid, value: false);

        Assert.Same(consumer, view.Filter);
        Assert.Equal([1, 2, 5, 6], VisibleIds(grid));
    }

    [StaFact]
    public void Clearing_one_column_keeps_the_other_columns()
    {
        var grid = CreateGrid();
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Country", ["Denmark"]));

        DataGridFilter.ClearColumnFilter(grid, Column(grid, "Country"));

        Assert.Equal([1, 3, 5], VisibleIds(grid));
    }

    [StaFact]
    public void Distinct_values_reflect_the_rows_that_pass_the_other_columns()
    {
        var grid = CreateGrid();
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Country", ["Germany"]));
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));

        var statuses = DataGridFilter.GetDistinctValues(grid, Column(grid, "Status"));
        var countries = DataGridFilter.GetDistinctValues(grid, Column(grid, "Country"));

        // Status ignores its own filter but honours Country; Country ignores its own filter but honours Status.
        Assert.Equal(["Closed", "Open"], statuses.Select(x => x.Text), StringComparer.Ordinal);
        Assert.Equal(["Denmark", "Germany"], countries.Select(x => x.Text), StringComparer.Ordinal);
    }

    [StaTheory]
    [InlineData(DataGridTextFilterOperator.Contains, "LIC", new[] { 1, 6 })]
    [InlineData(DataGridTextFilterOperator.IsEqualTo, "alice", new[] { 1, 6 })]
    [InlineData(DataGridTextFilterOperator.StartsWith, "c", new[] { 5 })]
    public void Text_filter_compares_ignoring_case(
        DataGridTextFilterOperator textOperator,
        string text,
        int[] expected)
    {
        var grid = CreateGrid();

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForText("Customer", textOperator, text));

        Assert.Equal(expected, VisibleIds(grid));
    }

    [StaFact]
    public void Number_and_date_ranges_are_inclusive()
    {
        var grid = CreateGrid();

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForNumberRange("Amount", 10, 300));
        Assert.Equal([1, 4, 6], VisibleIds(grid));

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForNumberRange("Amount", minimum: null, maximum: null));
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForDateRange("Date", new DateTime(2024, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Unspecified)));
        Assert.Equal([2, 3, 4, 5], VisibleIds(grid));
        Assert.Single(DataGridFilter.GetColumnFilters(grid));
    }

    [StaFact]
    public void Search_matches_any_visible_column_ignoring_case_and_ands_with_column_filters()
    {
        var grid = CreateGrid();

        DataGridFilter.SetSearchText(grid, "DENM");
        Assert.Equal([1, 3], VisibleIds(grid));

        DataGridFilter.SetSearchText(grid, "aarhus");
        Assert.Equal([1], VisibleIds(grid));

        DataGridFilter.SetSearchText(grid, "e");
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Closed"]));
        Assert.Equal([2, 6], VisibleIds(grid));

        DataGridFilter.SetSearchText(grid, string.Empty);
        Assert.Equal([2, 6], VisibleIds(grid));
    }

    [StaFact]
    public void Search_skips_hidden_columns()
    {
        var grid = CreateGrid();
        Column(grid, "Country").Visibility = Visibility.Collapsed;

        DataGridFilter.SetSearchText(grid, "Sweden");

        Assert.Empty(VisibleIds(grid));
    }

    [StaFact]
    public void Search_is_debounced()
    {
        var grid = CreateGrid();
        DataGridFilter.SetSearchDelay(grid, TimeSpan.FromSeconds(30));

        DataGridFilter.SetSearchText(grid, "Bob");

        Assert.True(DataGridFilter.IsSearchPending(grid));
        Assert.Equal(6, grid.Items.Count);

        DataGridFilter.ApplyPendingSearch(grid);

        Assert.False(DataGridFilter.IsSearchPending(grid));
        Assert.Equal([2], VisibleIds(grid));
    }

    [StaFact]
    public void Search_works_without_column_filters_enabled()
    {
        var grid = CreateGrid(isEnabled: false);

        DataGridFilter.SetSearchText(grid, "carol");

        Assert.Equal([5], VisibleIds(grid));
    }

    [StaFact]
    public void Each_apply_refreshes_the_view_once_and_raises_one_event()
    {
        var orders = Orders();
        var grid = CreateGrid(orders);
        var view = CollectionViewSource.GetDefaultView(orders);
        var resets = 0;
        ((INotifyCollectionChanged)view).CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                resets++;
            }
        };
        var events = new List<DataGridFilterChangedEventArgs>();
        DataGridFilter.AddFilterChangedHandler(grid, (_, e) => events.Add(e));

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));
        Assert.Equal(1, resets);

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Country", ["Denmark"]));
        Assert.Equal(2, resets);

        DataGridFilter.SetSearchText(grid, "a");
        Assert.Equal(3, resets);

        DataGridFilter.ClearAllFilters(grid);
        Assert.Equal(4, resets);

        Assert.Equal(4, events.Count);
        Assert.Equal(2, events[1].Filters.Count);
        Assert.Equal(2, events[1].ItemCount);
        Assert.Equal("a", events[2].SearchText);
        Assert.Empty(events[3].Filters);
    }

    [StaFact]
    public void Item_count_leaves_out_the_new_item_row()
    {
        var grid = CreateGrid();
        grid.CanUserAddRows = true;
        var events = new List<DataGridFilterChangedEventArgs>();
        DataGridFilter.AddFilterChangedHandler(grid, (_, e) => events.Add(e));

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));

        Assert.Equal(3, Assert.Single(events).ItemCount);
    }

    [StaFact]
    public void New_items_source_gets_the_filters_and_the_old_view_gets_its_filter_back()
    {
        var first = Orders();
        var grid = CreateGrid(first);
        var firstView = CollectionViewSource.GetDefaultView(first);
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));
        Assert.Equal(3, grid.Items.Count);

        var second = Orders();
        second.Add(new TestOrder { Id = 7, Status = "Open", Country = "Norway" });
        grid.ItemsSource = second;

        Assert.Null(firstView.Filter);
        Assert.Equal([1, 3, 5, 7], VisibleIds(grid));
        Assert.Single(DataGridFilter.GetColumnFilters(grid));
    }

    [StaFact]
    public void Column_chooser_lists_the_columns_and_hides_and_shows_them()
    {
        var grid = CreateGrid();
        DataGridFilter.SetCanUserHideColumn(Column(grid, "Id"), value: false);
        Column(grid, "City").Visibility = Visibility.Collapsed;

        var menu = DataGridFilter.CreateColumnChooserMenu(grid);
        var items = menu.Items.Cast<MenuItem>().ToList();

        Assert.Equal(["Id", "Customer", "Country", "Status", "Amount", "Date", "City"], items.Select(x => (string)x.Header), StringComparer.Ordinal);
        Assert.False(items[0].IsEnabled);
        Assert.False(items[6].IsChecked);

        items[2].IsChecked = false;
        items[6].IsChecked = true;

        Assert.Equal(Visibility.Collapsed, Column(grid, "Country").Visibility);
        Assert.Equal(Visibility.Visible, Column(grid, "City").Visibility);
    }

    [StaFact]
    public void Column_chooser_keeps_the_last_shown_column()
    {
        var grid = CreateGrid();
        foreach (var column in grid.Columns.Skip(1))
        {
            column.Visibility = Visibility.Collapsed;
        }

        var menu = DataGridFilter.CreateColumnChooserMenu(grid);
        var first = (MenuItem)menu.Items[0];
        first.IsChecked = false;

        Assert.True(first.IsChecked);
        Assert.Equal(Visibility.Visible, grid.Columns[0].Visibility);
    }

    [StaFact]
    public void Hiding_a_column_with_the_chooser_reapplies_the_search()
    {
        var grid = CreateGrid();
        DataGridFilter.SetSearchText(grid, "Sweden");
        Assert.Equal([4, 6], VisibleIds(grid));

        var menu = DataGridFilter.CreateColumnChooserMenu(grid);
        ((MenuItem)menu.Items[2]).IsChecked = false;

        Assert.Empty(VisibleIds(grid));
    }

    [StaFact]
    public void Template_column_without_a_path_cannot_be_filtered_but_one_with_sort_member_path_can()
    {
        var grid = CreateGrid();
        var noPath = new DataGridTemplateColumn { Header = "Actions" };
        var withPath = new DataGridTemplateColumn { Header = "Status (template)", SortMemberPath = nameof(TestOrder.Status) };
        grid.Columns.Add(noPath);
        grid.Columns.Add(withPath);

        Assert.False(DataGridFilter.CanFilterColumn(grid, noPath));
        Assert.True(DataGridFilter.CanFilterColumn(grid, withPath));
        Assert.Equal(["Closed", "Open", "Pending"], DataGridFilter.GetDistinctValues(grid, withPath).Select(x => x.Text), StringComparer.Ordinal);
    }

    [StaFact]
    public void Column_opt_out_disables_filtering_of_that_column()
    {
        var grid = CreateGrid();
        var column = Column(grid, "Country");

        DataGridFilter.SetIsColumnFilterEnabled(column, value: false);

        Assert.False(DataGridFilter.CanFilterColumn(grid, column));
    }
}