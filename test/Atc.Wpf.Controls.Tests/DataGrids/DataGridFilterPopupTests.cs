namespace Atc.Wpf.Controls.Tests.DataGrids;

using static DataGridFilterTestData;

/// <summary>
/// The header filter buttons and the filter pop-up of a grid shown in a window.
/// </summary>
[Collection(nameof(PackResourceTestGroup))]
public sealed class DataGridFilterPopupTests : IDisposable
{
    private readonly List<Window> windows = [];
    private readonly List<DataGrid> grids = [];

    public void Dispose()
    {
        foreach (var grid in grids)
        {
            DataGridFilter.CloseColumnFilter(grid);
        }

        foreach (var window in windows)
        {
            window.Close();
        }

        Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();
    }

    [StaFact]
    public void Each_filterable_column_header_gets_an_invoke_button_named_after_the_column()
    {
        var grid = Show(CreateGrid());

        var button = FindButton(grid, "Status");
        var peer = UIElementAutomationPeer.CreatePeerForElement(button);

        Assert.True(button.IsVisible);
        Assert.Equal(string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.FilterColumn1, "Status"), peer.GetName());
        Assert.Equal(AutomationControlType.Button, peer.GetAutomationControlType());
        Assert.IsAssignableFrom<IInvokeProvider>(peer.GetPattern(PatternInterface.Invoke));
    }

    [StaFact]
    public void Column_opt_out_and_template_columns_without_a_path_show_no_button()
    {
        var grid = CreateGrid();
        DataGridFilter.SetIsColumnFilterEnabled(Column(grid, "Country"), value: false);
        grid.Columns.Add(new DataGridTemplateColumn { Header = "Actions" });
        Show(grid);

        Assert.False(FindButton(grid, "Country").IsVisible);
        Assert.False(FindButton(grid, "Actions").IsVisible);
        Assert.True(FindButton(grid, "Customer").IsVisible);
    }

    [StaFact]
    public void Pressing_the_button_does_not_press_or_sort_the_header()
    {
        var grid = Show(CreateGrid());
        var button = FindButton(grid, "Status");
        var header = FindAncestor<DataGridColumnHeader>(button)!;

        var args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
        {
            RoutedEvent = UIElement.MouseLeftButtonDownEvent,
        };
        button.RaiseEvent(args);

        Assert.True(args.Handled);
        Assert.False(header.IsPressed);
        Assert.Null(Column(grid, "Status").SortDirection);
        button.ReleaseMouseCapture();
    }

    [StaFact]
    public void Invoking_the_button_opens_the_pop_up_under_it_with_the_content_reachable_through_the_peer()
    {
        var grid = Show(CreateGrid());
        var button = FindButton(grid, "Status");
        var peer = UIElementAutomationPeer.CreatePeerForElement(button);

        ((IInvokeProvider)peer.GetPattern(PatternInterface.Invoke)).Invoke();
        DispatcherHelper.DoEvents();

        var content = DataGridFilter.GetOpenPopupContent(grid, Column(grid, "Status"));
        Assert.NotNull(content);
        Assert.Same(button, DataGridFilter.GetOpenPopup(grid)!.PlacementTarget);
        Assert.Equal(["#Closed", "#Open", "#Pending"], content.ValueList.Items.Keys, StringComparer.Ordinal);
        Assert.Equal(3, content.ValueList.SelectedKeys.Count);

        peer.ResetChildrenCache();
        var children = peer.GetChildren();
        Assert.Contains(children, x => x.GetType().Name.Contains("UserControl", StringComparison.Ordinal));
        Assert.Contains(Descendants(peer), x => x.GetName() == Atc.Wpf.Resources.Word.Ok);
        Assert.Contains(Descendants(peer), x => x.GetName() == "Closed");
    }

    [StaFact]
    public void Unticking_a_value_and_ok_applies_the_filter_and_closes_the_pop_up()
    {
        var grid = Show(CreateGrid());
        var column = Column(grid, "Status");
        DataGridFilter.OpenColumnFilter(grid, column);
        var content = DataGridFilter.GetOpenPopupContent(grid)!;

        content.ValueList.GetItem("#Closed")!.IsChecked = false;
        content.OkButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

        Assert.Null(DataGridFilter.GetOpenPopup(grid));
        Assert.Equal([1, 3, 4, 5], VisibleIds(grid));
        Assert.True(FindButton(grid, "Status").IsFilterActive);
        var filter = Assert.Single(DataGridFilter.GetColumnFilters(grid));
        Assert.Equal(["Open", "Pending"], filter.Values);
    }

    [StaFact]
    public void Reopening_shows_the_current_filter_and_clear_filter_removes_it()
    {
        var grid = Show(CreateGrid());
        var column = Column(grid, "Status");
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));

        DataGridFilter.OpenColumnFilter(grid, column);
        var content = DataGridFilter.GetOpenPopupContent(grid)!;
        Assert.Equal(["#Open"], content.ValueList.SelectedKeys);

        content.ClearButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

        Assert.Null(DataGridFilter.GetOpenPopup(grid));
        Assert.Empty(DataGridFilter.GetColumnFilters(grid));
        Assert.Equal(6, grid.Items.Count);
        Assert.False(FindButton(grid, "Status").IsFilterActive);
    }

    [StaFact]
    public void Ticking_everything_again_removes_the_filter()
    {
        var grid = Show(CreateGrid());
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));
        DataGridFilter.OpenColumnFilter(grid, Column(grid, "Status"));
        var content = DataGridFilter.GetOpenPopupContent(grid)!;

        content.ValueList.SelectAll();
        Assert.True(content.HandleKey(Key.Enter, originalSource: null));

        Assert.Empty(DataGridFilter.GetColumnFilters(grid));
        Assert.Equal(6, grid.Items.Count);
    }

    [StaFact]
    public void Ok_on_a_values_filter_reopened_while_another_column_is_filtered_keeps_it()
    {
        var grid = Show(CreateGrid());
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Country", ["Denmark"]));

        // Only "Open" is shown (both Danish rows are open) and it is ticked.
        DataGridFilter.OpenColumnFilter(grid, Column(grid, "Status"));
        var content = DataGridFilter.GetOpenPopupContent(grid)!;
        Assert.Equal(["#Open"], content.ValueList.Items.Keys, StringComparer.Ordinal);
        content.OkButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        DataGridFilter.ClearColumnFilter(grid, Column(grid, "Country"));

        Assert.Equal([1, 3, 5], VisibleIds(grid));
        var filter = Assert.Single(DataGridFilter.GetColumnFilters(grid));
        Assert.Equal(["Open"], filter.Values);
    }

    [StaFact]
    public void Ticking_every_shown_value_with_the_hidden_values_allowed_removes_the_filter()
    {
        var grid = Show(CreateGrid());
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open", "Pending"]));
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Country", ["Sweden"]));

        // Shown: Closed, Pending (the Swedish rows); Open is hidden and allowed.
        DataGridFilter.OpenColumnFilter(grid, Column(grid, "Status"));
        var content = DataGridFilter.GetOpenPopupContent(grid)!;
        Assert.Equal(["#Closed", "#Pending"], content.ValueList.Items.Keys, StringComparer.Ordinal);
        content.ValueList.SelectAll();
        content.OkButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        DataGridFilter.ClearColumnFilter(grid, Column(grid, "Country"));

        Assert.Empty(DataGridFilter.GetColumnFilters(grid));
        Assert.Equal(6, grid.Items.Count);
    }

    [StaFact]
    public void Ticking_every_shown_value_with_a_hidden_excluded_value_keeps_a_filter()
    {
        var grid = Show(CreateGrid());
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open", "Closed"]));
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Country", ["Germany"]));

        // Shown: Closed, Open (the German rows), both ticked; Pending is hidden and excluded.
        DataGridFilter.OpenColumnFilter(grid, Column(grid, "Status"));
        var content = DataGridFilter.GetOpenPopupContent(grid)!;
        content.OkButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        DataGridFilter.ClearColumnFilter(grid, Column(grid, "Country"));

        Assert.Equal([1, 2, 3, 5, 6], VisibleIds(grid));
    }

    [StaFact]
    public void Unticking_a_shown_value_keeps_the_hidden_values_allowed()
    {
        var grid = Show(CreateGrid());
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Country", ["Germany"]));

        // Shown: Closed, Open (the German rows); Pending is hidden and, without a Status filter, allowed.
        DataGridFilter.OpenColumnFilter(grid, Column(grid, "Status"));
        var content = DataGridFilter.GetOpenPopupContent(grid)!;
        content.ValueList.GetItem("#Closed")!.IsChecked = false;
        content.OkButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Assert.Equal([5], VisibleIds(grid));
        DataGridFilter.ClearColumnFilter(grid, Column(grid, "Country"));

        Assert.Equal([1, 3, 4, 5], VisibleIds(grid));
    }

    [StaFact]
    public void Escape_cancels_and_keeps_the_filters()
    {
        var grid = Show(CreateGrid());
        DataGridFilter.OpenColumnFilter(grid, Column(grid, "Status"));
        var content = DataGridFilter.GetOpenPopupContent(grid)!;
        content.ValueList.GetItem("#Open")!.IsChecked = false;

        Assert.True(content.HandleKey(Key.Escape, originalSource: null));

        Assert.Null(DataGridFilter.GetOpenPopup(grid));
        Assert.Empty(DataGridFilter.GetColumnFilters(grid));
        Assert.Equal(6, grid.Items.Count);
    }

    [StaFact]
    public void Enter_on_a_button_is_left_to_the_button()
    {
        var grid = Show(CreateGrid());
        DataGridFilter.OpenColumnFilter(grid, Column(grid, "Status"));
        var content = DataGridFilter.GetOpenPopupContent(grid)!;

        Assert.False(content.HandleKey(Key.Enter, content.CancelButton));
        Assert.NotNull(DataGridFilter.GetOpenPopup(grid));
    }

    [StaFact]
    public void Alt_down_opens_the_filter_of_the_focused_button_or_the_current_column()
    {
        var grid = Show(CreateGrid());
        var button = FindButton(grid, "Country");

        Assert.True(DataGridFilter.HandleKey(grid, Key.System, Key.Down, button));
        Assert.NotNull(DataGridFilter.GetOpenPopupContent(grid, Column(grid, "Country")));
        DataGridFilter.CloseColumnFilter(grid);

        grid.CurrentCell = new DataGridCellInfo(grid.Items[0], Column(grid, "Customer"));
        Assert.True(DataGridFilter.HandleKey(grid, Key.System, Key.Down, grid));
        Assert.NotNull(DataGridFilter.GetOpenPopupContent(grid, Column(grid, "Customer")));
        DataGridFilter.CloseColumnFilter(grid);

        Assert.False(DataGridFilter.HandleKey(grid, Key.Down, Key.None, grid));
        Assert.Null(DataGridFilter.GetOpenPopup(grid));
    }

    [StaFact]
    public void Text_mode_builds_a_text_filter()
    {
        var grid = Show(CreateGrid());
        DataGridFilter.OpenColumnFilter(grid, Column(grid, "Customer"));
        var content = DataGridFilter.GetOpenPopupContent(grid)!;

        content.Mode = DataGridColumnFilterKind.Text;
        content.TextFilterBox.Text = "AL";
        content.OkButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

        var filter = Assert.Single(DataGridFilter.GetColumnFilters(grid));
        Assert.Equal(DataGridColumnFilterKind.Text, filter.Kind);
        Assert.Equal([1, 6], VisibleIds(grid));
    }

    [StaFact]
    public void Range_mode_is_offered_for_number_and_date_columns_only()
    {
        var grid = Show(CreateGrid());

        DataGridFilter.OpenColumnFilter(grid, Column(grid, "Amount"));
        var amount = DataGridFilter.GetOpenPopupContent(grid)!;
        amount.Mode = DataGridColumnFilterKind.NumberRange;
        Assert.Equal(DataGridColumnFilterKind.NumberRange, amount.Mode);
        amount.FromBox.Text = "10";
        amount.ToBox.Text = "1,000";
        amount.OkButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Assert.Equal([1, 4, 6], VisibleIds(grid));

        DataGridFilter.OpenColumnFilter(grid, Column(grid, "Date"));
        var date = DataGridFilter.GetOpenPopupContent(grid)!;
        date.Mode = DataGridColumnFilterKind.DateRange;
        Assert.Equal(DataGridColumnFilterKind.DateRange, date.Mode);
        DataGridFilter.CloseColumnFilter(grid);

        DataGridFilter.OpenColumnFilter(grid, Column(grid, "Country"));
        var country = DataGridFilter.GetOpenPopupContent(grid)!;
        country.Mode = DataGridColumnFilterKind.NumberRange;
        Assert.Equal(DataGridColumnFilterKind.Values, country.Mode);
    }

    [StaFact]
    public void Header_keeps_the_column_title_with_the_consumer_header_template()
    {
        var grid = CreateGrid(isEnabled: false);
        var template = new DataTemplate();
        var factory = new FrameworkElementFactory(typeof(TextBlock));
        factory.SetBinding(TextBlock.TextProperty, new Binding { StringFormat = "[{0}]" });
        template.VisualTree = factory;
        Column(grid, "Status").HeaderTemplate = template;
        DataGridFilter.SetIsEnabled(grid, value: true);
        Show(grid);

        var header = FindAncestor<DataGridColumnHeader>(FindButton(grid, "Status"))!;
        var texts = FindVisuals<TextBlock>(header).Select(x => x.Text).ToList();

        Assert.Contains("[Status]", texts, StringComparer.Ordinal);
    }

    [StaFact]
    public void A_header_template_set_after_the_filter_wraps_the_header_keeps_the_button()
    {
        var grid = Show(CreateGrid());
        var column = Column(grid, "Status");
        var template = new DataTemplate();
        var factory = new FrameworkElementFactory(typeof(TextBlock));
        factory.SetBinding(TextBlock.TextProperty, new Binding { StringFormat = "[{0}]" });
        template.VisualTree = factory;

        column.HeaderTemplate = template;
        grid.UpdateLayout();
        DispatcherHelper.DoEvents();

        Assert.True(DataGridFilter.IsHeaderWrapped(column));
        var header = FindAncestor<DataGridColumnHeader>(FindButton(grid, "Status"))!;
        Assert.Contains("[Status]", FindVisuals<TextBlock>(header).Select(x => x.Text), StringComparer.Ordinal);

        DataGridFilter.SetIsEnabled(grid, value: false);
        Assert.Same(template, column.HeaderTemplate);
    }

    [StaFact]
    public void A_header_template_selector_set_after_the_filter_wraps_the_header_keeps_the_button()
    {
        var grid = Show(CreateGrid());
        var column = Column(grid, "Status");
        var template = new DataTemplate();
        var factory = new FrameworkElementFactory(typeof(TextBlock));
        factory.SetBinding(TextBlock.TextProperty, new Binding { StringFormat = "<{0}>" });
        template.VisualTree = factory;

        column.HeaderTemplateSelector = new FixedTemplateSelector(template);
        grid.UpdateLayout();
        DispatcherHelper.DoEvents();

        Assert.True(DataGridFilter.IsHeaderWrapped(column));
        var header = FindAncestor<DataGridColumnHeader>(FindButton(grid, "Status"))!;
        Assert.Contains("<Status>", FindVisuals<TextBlock>(header).Select(x => x.Text), StringComparer.Ordinal);
    }

    [StaFact]
    public void Funnel_follows_the_header_foreground_and_turns_accent_when_active()
    {
        var headerForeground = new SolidColorBrush(Colors.WhiteSmoke);
        var accent = new SolidColorBrush(Colors.Orange);
        var grid = CreateGrid();
        var headerStyle = new Style(typeof(DataGridColumnHeader));
        headerStyle.Setters.Add(new Setter(Control.ForegroundProperty, headerForeground));
        grid.ColumnHeaderStyle = headerStyle;
        grid.Resources["AtcApps.Brushes.Accent"] = accent;
        Show(grid);
        var funnel = (System.Windows.Shapes.Path)FindButton(grid, "Status").Content;

        Assert.Same(headerForeground, funnel.Stroke);
        Assert.Null(funnel.Fill);

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open"]));

        Assert.Same(accent, funnel.Fill);
        Assert.Same(accent, funnel.Stroke);
    }

    [StaFact]
    public void Filter_buttons_are_reachable_from_the_grid_in_the_automation_tree()
    {
        var grid = Show(CreateGrid());
        var gridPeer = UIElementAutomationPeer.CreatePeerForElement(grid);
        var expected = string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.FilterColumn1, "Status");

        var button = Descendants(gridPeer).FirstOrDefault(x =>
            string.Equals(x.GetClassName(), "DataGridFilterButton", StringComparison.Ordinal) &&
            string.Equals(x.GetName(), expected, StringComparison.Ordinal));

        Assert.NotNull(button);
        Assert.Equal(AutomationControlType.Button, button.GetAutomationControlType());
    }

    [StaFact]
    public void Alt_down_in_an_editing_combo_box_is_left_to_the_combo_box()
    {
        var grid = Show(CreateGrid());
        grid.CurrentCell = new DataGridCellInfo(grid.Items[0], Column(grid, "Customer"));

        Assert.False(DataGridFilter.HandleKey(grid, Key.System, Key.Down, new ComboBox()));
        Assert.Null(DataGridFilter.GetOpenPopup(grid));
    }

    [StaFact]
    public void Opening_the_filter_of_a_hundred_thousand_distinct_values_is_measured()
    {
        var orders = Enumerable
            .Range(0, 100_000)
            .Select(i => new TestOrder { Id = i, Status = (i % 5).ToString(CultureInfo.InvariantCulture), Country = "Denmark" })
            .ToList();
        var grid = Show(CreateGrid(orders));

        var stopwatch = Stopwatch.StartNew();
        DataGridFilter.OpenColumnFilter(grid, Column(grid, "Id"));
        DataGridFilter.GetOpenPopup(grid)!.Child.UpdateLayout();
        var idMs = stopwatch.ElapsedMilliseconds;
        var content = DataGridFilter.GetOpenPopupContent(grid)!;
        Assert.Equal(100_000, content.ValueList.Items.Count);

        stopwatch.Restart();
        content.ValueList.GetItem("#7")!.IsChecked = false;
        content.OkButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        var okMs = stopwatch.ElapsedMilliseconds;
        Assert.Equal(99_999, grid.Items.Count);

        stopwatch.Restart();
        DataGridFilter.OpenColumnFilter(grid, Column(grid, "Status"));
        DataGridFilter.GetOpenPopup(grid)!.Child.UpdateLayout();
        var statusMs = stopwatch.ElapsedMilliseconds;
        DataGridFilter.CloseColumnFilter(grid);

        TestContext.Current.TestOutputHelper?.WriteLine(
            $"100k rows: open Id filter (100,000 values) {idMs} ms, OK {okMs} ms, open Status filter (5 values) {statusMs} ms");
    }

    private sealed class FixedTemplateSelector(DataTemplate template) : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(
            object item,
            DependencyObject container)
            => template;
    }

    private DataGrid Show(DataGrid grid)
    {
        grids.Add(grid);
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
        return grid;
    }

    private static DataGridFilterButton FindButton(
        DataGrid grid,
        string header)
    {
        var column = Column(grid, header);
        return FindVisuals<DataGridFilterButton>(grid).Single(x =>
            ReferenceEquals(x.Column, column) ||
            ReferenceEquals(FindAncestor<DataGridColumnHeader>(x)?.Column, column));
    }

    private static IEnumerable<AutomationPeer> Descendants(AutomationPeer peer)
    {
        foreach (var child in peer.GetChildren() ?? [])
        {
            yield return child;
            foreach (var descendant in Descendants(child))
            {
                yield return descendant;
            }
        }
    }

    private static List<T> FindVisuals<T>(DependencyObject root)
        where T : DependencyObject
    {
        var result = new List<T>();
        var stack = new Stack<DependencyObject>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current is T match)
            {
                result.Add(match);
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++)
            {
                stack.Push(VisualTreeHelper.GetChild(current, i));
            }
        }

        return result;
    }

    private static T? FindAncestor<T>(DependencyObject? element)
        where T : DependencyObject
    {
        while (element is not null)
        {
            element = VisualTreeHelper.GetParent(element);
            if (element is T match)
            {
                return match;
            }
        }

        return null;
    }
}