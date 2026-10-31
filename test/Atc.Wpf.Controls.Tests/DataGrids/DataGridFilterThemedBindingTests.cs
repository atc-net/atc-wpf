namespace Atc.Wpf.Controls.Tests.DataGrids;

using static DataGridFilterTestData;

/// <summary>
/// Filters a grid that carries the ATC DataGrid styles: refreshing the view regenerates and recycles rows, and the
/// themed row template must not log a data binding error while doing so (the sample app shows every one in a dialog).
/// </summary>
[Collection(nameof(PackResourceTestGroup))]
public sealed class DataGridFilterThemedBindingTests : IDisposable
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
    public void Filtering_clearing_scrolling_and_refreshing_a_themed_grid_logs_no_binding_errors()
    {
        using var trace = BindingTraceCapture.Start();
        var grid = Show(CreateGrid(ManyOrders(500)));
        var customer = Column(grid, "Customer");
        var status = Column(grid, "Status");

        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForText(DataGridFilter.ResolveColumnKey(customer), DataGridTextFilterOperator.Contains, "1"));
        Pump();
        ScrollToEnd(grid);
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues(DataGridFilter.ResolveColumnKey(status), ["Open"]));
        Pump();
        DataGridFilter.SetSearchText(grid, "7");
        Pump();
        DataGridFilter.SetSearchText(grid, string.Empty);
        Pump();
        DataGridFilter.ClearColumnFilter(grid, customer);
        Pump();
        ScrollToEnd(grid);
        DataGridFilter.ClearAllFilters(grid);
        Pump();
        grid.Items.Refresh();
        Pump();
        grid.ScrollIntoView(grid.Items[0]);
        Pump();

        AssertNoBindingErrors(trace);
    }

    [StaFact]
    public void Text_contains_filter_through_the_pop_up_logs_no_binding_errors()
    {
        using var trace = BindingTraceCapture.Start();
        var grid = Show(CreateGrid(ManyOrders(500)));
        ScrollToEnd(grid);

        DataGridFilter.OpenColumnFilter(grid, Column(grid, "Customer"));
        Pump();
        var content = DataGridFilter.GetOpenPopupContent(grid)!;
        content.Mode = DataGridColumnFilterKind.Text;
        content.TextFilterBox.Text = "12";
        Pump();
        content.OkButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Equal(DataGridColumnFilterKind.Text, Assert.Single(DataGridFilter.GetColumnFilters(grid)).Kind);
        Assert.All(grid.Items.OfType<TestOrder>(), x => Assert.Contains("12", x.Customer, StringComparison.Ordinal));
        AssertNoBindingErrors(trace);
    }

    [StaFact]
    public void Row_headers_follow_the_grid_headers_visibility_and_details_follow_are_row_details_frozen()
    {
        using var trace = BindingTraceCapture.Start();
        var grid = CreateGrid(ManyOrders(50));
        grid.RowDetailsTemplate = new DataTemplate { VisualTree = new FrameworkElementFactory(typeof(TextBlock)) };
        grid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible;
        Show(grid);

        // The themed grid shows column headers only.
        Assert.All(RowParts<DataGridRowHeader>(grid), x => Assert.Equal(Visibility.Collapsed, x.Visibility));
        Assert.All(RowParts<DataGridDetailsPresenter>(grid), x => Assert.Equal(SelectiveScrollingOrientation.Both, SelectiveScrollingGrid.GetSelectiveScrollingOrientation(x)));

        grid.HeadersVisibility = DataGridHeadersVisibility.All;
        grid.AreRowDetailsFrozen = true;
        Pump();
        Assert.All(RowParts<DataGridRowHeader>(grid), x => Assert.Equal(Visibility.Visible, x.Visibility));
        Assert.All(RowParts<DataGridDetailsPresenter>(grid), x => Assert.Equal(SelectiveScrollingOrientation.Vertical, SelectiveScrollingGrid.GetSelectiveScrollingOrientation(x)));

        grid.HeadersVisibility = DataGridHeadersVisibility.Column;
        grid.AreRowDetailsFrozen = false;
        Pump();
        Assert.All(RowParts<DataGridRowHeader>(grid), x => Assert.Equal(Visibility.Collapsed, x.Visibility));
        Assert.All(RowParts<DataGridDetailsPresenter>(grid), x => Assert.Equal(SelectiveScrollingOrientation.Both, SelectiveScrollingGrid.GetSelectiveScrollingOrientation(x)));
        AssertNoBindingErrors(trace);
    }

    private static List<T> RowParts<T>(DataGrid grid)
        where T : DependencyObject
    {
        var rows = FindVisuals<DataGridRow>(grid);
        Assert.NotEmpty(rows);
        var parts = rows.SelectMany(FindVisuals<T>).ToList();
        Assert.Equal(rows.Count, parts.Count);
        return parts;
    }

    private static List<T> FindVisuals<T>(DependencyObject root)
        where T : DependencyObject
    {
        var result = new List<T>();
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is T match)
            {
                result.Add(match);
            }
            else
            {
                result.AddRange(FindVisuals<T>(child));
            }
        }

        return result;
    }

    private static List<TestOrder> ManyOrders(int count)
        => Enumerable.Range(1, count)
            .Select(i => new TestOrder
            {
                Id = i,
                Customer = $"Customer {i}",
                Country = (i % 3) switch { 0 => "Denmark", 1 => "Germany", _ => "Sweden" },
                Status = (i % 2) == 0 ? "Open" : "Closed",
                Amount = i,
                Date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).AddDays(i),
            })
            .ToList();

    private static ResourceDictionary CreateThemedResources()
    {
        // The theme dictionaries use the StaticResource markup type from Atc.Wpf, and pack://application
        // URIs need the Application type initialized (not an instance).
        _ = typeof(Atc.Wpf.MarkupExtensions.StaticResourceExtension).Assembly;
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);

        var styles = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Theming;component/Styles/BaseControls/DataGrid.xaml", UriKind.Absolute),
        };
        var resources = new ResourceDictionary();
        resources.MergedDictionaries.Add(styles);
        resources.MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Theming;component/Styles/Themes/Light.Blue.xaml", UriKind.Absolute),
        });
        resources[typeof(DataGrid)] = styles["AtcApps.Styles.DataGrid"];
        return resources;
    }

    private static void AssertNoBindingErrors(BindingTraceCapture trace)
        => Assert.True(
            trace.Events.Count == 0,
            string.Join(Environment.NewLine, trace.Events.Select(x => $"{x.Id}: {x.Message}")));

    private static void Pump()
        => Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

    private static void ScrollToEnd(DataGrid grid)
    {
        if (grid.Items.Count > 0)
        {
            grid.ScrollIntoView(grid.Items[grid.Items.Count - 1]);
        }

        Pump();
    }

    private DataGrid Show(DataGrid grid)
    {
        grids.Add(grid);
        grid.EnableRowVirtualization = true;
        var window = new Window
        {
            Resources = CreateThemedResources(),
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
        Pump();
        return grid;
    }
}