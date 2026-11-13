namespace Atc.Wpf.Benchmarks;

/// <summary>
/// Measures what opening a DataGridFilter column pop-up costs: the distinct values of a column over 100,000 rows,
/// with two other columns filtered (each row is checked against them). <c>Objects_*</c> uses a list of objects,
/// <c>DataTable_*</c> a <see cref="DataTable"/> (whose rows are copied through a filtered <see cref="DataView"/>).
/// </summary>
[MemoryDiagnoser]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "BenchmarkDotNet owns the instance; the table is disposed in GlobalCleanup.")]
public class DataGridFilterDistinctValuesBenchmarks
{
    private const int RowCount = 100_000;

    private static readonly string[] Statuses = ["Open", "Closed", "Pending", "Cancelled", "Shipped"];
    private static readonly string[] Countries = ["Denmark", "Germany", "Sweden", "Norway", "Finland", "France", "Spain"];

    private readonly DataTable table = new();
    private DataGrid objectGrid = null!;
    private DataGrid tableGrid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var orders = new List<FilterOrder>(RowCount);
        table.Columns.Add(nameof(FilterOrder.Customer), typeof(string));
        table.Columns.Add(nameof(FilterOrder.Country), typeof(string));
        table.Columns.Add(nameof(FilterOrder.Status), typeof(string));
        table.Columns.Add(nameof(FilterOrder.Amount), typeof(decimal));
        table.Columns.Add(nameof(FilterOrder.Date), typeof(DateTime));
        for (var i = 0; i < RowCount; i++)
        {
            var order = new FilterOrder(
                "Customer " + (i % 2_000).ToString(CultureInfo.InvariantCulture),
                Countries[i % Countries.Length],
                Statuses[i % Statuses.Length],
                (i % 10_000) / 4m,
                new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).AddDays(i % 730));
            orders.Add(order);
            table.Rows.Add(order.Customer, order.Country, order.Status, order.Amount, order.Date);
        }

        objectGrid = CreateGrid(orders);
        tableGrid = CreateGrid(table.DefaultView);
    }

    [GlobalCleanup]
    public void Cleanup()
        => table.Dispose();

    [Benchmark(Baseline = true)]
    [STAThread]
    public int Objects_customers()
        => DataGridFilter.GetDistinctValues(objectGrid, Column(objectGrid, "Customer")).Count;

    [Benchmark]
    [STAThread]
    public int Objects_amounts()
        => DataGridFilter.GetDistinctValues(objectGrid, Column(objectGrid, "Amount")).Count;

    [Benchmark]
    [STAThread]
    public int Objects_dates()
        => DataGridFilter.GetDistinctValues(objectGrid, Column(objectGrid, "Date")).Count;

    [Benchmark]
    [STAThread]
    public int DataTable_customers()
        => DataGridFilter.GetDistinctValues(tableGrid, Column(tableGrid, "Customer")).Count;

    private static DataGrid CreateGrid(IEnumerable itemsSource)
    {
        var grid = new DataGrid
        {
            AutoGenerateColumns = false,
            CanUserAddRows = false,
            Language = XmlLanguage.GetLanguage("en-US"),
            ItemsSource = itemsSource,
        };

        grid.Columns.Add(new DataGridTextColumn { Header = "Customer", Binding = new Binding(nameof(FilterOrder.Customer)) });
        grid.Columns.Add(new DataGridTextColumn { Header = "Country", Binding = new Binding(nameof(FilterOrder.Country)) });
        grid.Columns.Add(new DataGridTextColumn { Header = "Status", Binding = new Binding(nameof(FilterOrder.Status)) });
        grid.Columns.Add(new DataGridTextColumn { Header = "Amount", Binding = new Binding(nameof(FilterOrder.Amount)) { StringFormat = "N2" } });
        grid.Columns.Add(new DataGridTextColumn { Header = "Date", Binding = new Binding(nameof(FilterOrder.Date)) { StringFormat = "{0:yyyy-MM-dd}" } });

        DataGridFilter.SetSearchDelay(grid, TimeSpan.Zero);
        DataGridFilter.SetIsEnabled(grid, true);

        // Two other columns filtered, so every row is also checked against them.
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open", "Pending", "Shipped"]));
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Country", ["Denmark", "Germany", "Sweden", "Norway"]));
        return grid;
    }

    private static DataGridColumn Column(
        DataGrid grid,
        string header)
        => grid.Columns.First(x => string.Equals((string)x.Header, header, StringComparison.Ordinal));
}