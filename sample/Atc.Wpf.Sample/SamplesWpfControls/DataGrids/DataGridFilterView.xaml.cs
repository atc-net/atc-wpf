namespace Atc.Wpf.Sample.SamplesWpfControls.DataGrids;

public partial class DataGridFilterView
{
    private string? savedLayout;

    public DataGridFilterView()
    {
        InitializeComponent();

        OrdersGrid.ItemsSource = SampleOrder.Create(5_000, seed: 42);
        BigGrid.ItemsSource = SampleOrder.Create(100_000, seed: 7);
        UpdateCounts();

        DataGridFilter.AddFilterChangedHandler(OrdersGrid, OnOrdersFilterChanged);
        DataGridFilter.AddFilterChangedHandler(BigGrid, OnBigFilterChanged);
    }

    private void OnOrdersFilterChanged(
        object sender,
        DataGridFilterChangedEventArgs e)
    {
        var filters = string.Join(", ", e.Filters.Select(Describe));
        AppendLog(
            string.Create(
                CultureInfo.InvariantCulture,
                $"FilterChanged: rows={e.ItemCount} filters=[{filters}] search='{e.SearchText}'"));
        UpdateCounts();
    }

    private void OnBigFilterChanged(
        object sender,
        DataGridFilterChangedEventArgs e)
        => UpdateCounts();

    private void UpdateCounts()
    {
        OrdersRowCount.Text = string.Create(CultureInfo.InvariantCulture, $"{OrdersGrid.Items.Count} rows");
        BigRowCount.Text = string.Create(CultureInfo.InvariantCulture, $"{BigGrid.Items.Count:N0} rows");
    }

    private void OnColumnsClick(
        object sender,
        RoutedEventArgs e)
        => DataGridFilter.OpenColumnChooser(OrdersGrid);

    private void OnClearFiltersClick(
        object sender,
        RoutedEventArgs e)
        => DataGridFilter.ClearAllFilters(OrdersGrid);

    private void OnSaveLayoutClick(
        object sender,
        RoutedEventArgs e)
    {
        savedLayout = DataGridFilter.SaveLayout(OrdersGrid);
        AppendLog("Layout saved");
    }

    private void OnRestoreLayoutClick(
        object sender,
        RoutedEventArgs e)
    {
        if (savedLayout is null)
        {
            AppendLog("No layout saved yet");
            return;
        }

        DataGridFilter.LoadLayout(OrdersGrid, savedLayout);
    }

    private void AppendLog(string line)
    {
        // Keep the last 12 lines.
        var lines = FilterLog.Text
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Append(line)
            .TakeLast(12);
        FilterLog.Text = string.Join(Environment.NewLine, lines);
    }

    private static string Describe(DataGridColumnFilter filter)
        => filter.Kind switch
        {
            DataGridColumnFilterKind.Values => $"{filter.ColumnKey} in ({string.Join("|", filter.Values)})",
            DataGridColumnFilterKind.Text => $"{filter.ColumnKey} {filter.TextOperator} '{filter.Text}'",
            DataGridColumnFilterKind.NumberRange => string.Create(CultureInfo.InvariantCulture, $"{filter.ColumnKey} {filter.Minimum}..{filter.Maximum}"),
            _ => string.Create(CultureInfo.InvariantCulture, $"{filter.ColumnKey} {filter.From:d}..{filter.To:d}"),
        };
}