namespace Atc.Wpf.Controls.Tests.DataGrids;

using static DataGridFilterTestData;

/// <summary>
/// 100,000 rows: the distinct values are computed when the pop-up opens and a filter change refreshes the view once.
/// The timings are written to the test output (not asserted, to keep the run stable on slow machines).
/// </summary>
public sealed class DataGridFilterPerformanceTests : IDisposable
{
    private const int RowCount = 100_000;

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Hundred_thousand_rows_distinct_values_and_apply_with_one_refresh()
    {
        var statuses = new[] { "Open", "Closed", "Pending", "Cancelled", "Shipped" };
        var countries = new[] { "Denmark", "Germany", "Sweden", "Norway", "Finland", "France", "Spain" };
        var orders = new List<TestOrder>(RowCount);
        for (var i = 0; i < RowCount; i++)
        {
            orders.Add(new TestOrder
            {
                Id = i,
                Customer = "Customer " + (i % 2_000).ToString(CultureInfo.InvariantCulture),
                Country = countries[i % countries.Length],
                Status = statuses[i % statuses.Length],
                Amount = (i % 10_000) / 4m,
                Date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).AddDays(i % 730),
            });
        }

        var grid = CreateGrid(orders);
        var resets = 0;
        ((INotifyCollectionChanged)CollectionViewSource.GetDefaultView(orders)).CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                resets++;
            }
        };

        var stopwatch = Stopwatch.StartNew();
        var customerValues = DataGridFilter.GetDistinctValues(grid, Column(grid, "Customer"));
        var distinctCustomersMs = stopwatch.ElapsedMilliseconds;

        stopwatch.Restart();
        var amountValues = DataGridFilter.GetDistinctValues(grid, Column(grid, "Amount"));
        var distinctAmountsMs = stopwatch.ElapsedMilliseconds;

        stopwatch.Restart();
        DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open", "Pending"]));
        var applyMs = stopwatch.ElapsedMilliseconds;

        stopwatch.Restart();
        DataGridFilter.SetSearchText(grid, "customer 1");
        var searchMs = stopwatch.ElapsedMilliseconds;

        Assert.Equal(2_000, customerValues.Count);
        Assert.Equal(10_000, amountValues.Count);
        Assert.Equal(2, resets);
        Assert.Equal(
            orders.Count(x => (x.Status is "Open" or "Pending") && x.Customer!.Contains("customer 1", StringComparison.OrdinalIgnoreCase)),
            grid.Items.Count);

        TestContext.Current.TestOutputHelper?.WriteLine(
            $"100k rows: distinct customers {distinctCustomersMs} ms, distinct amounts {distinctAmountsMs} ms, " +
            $"apply values filter {applyMs} ms, apply search {searchMs} ms");
    }
}