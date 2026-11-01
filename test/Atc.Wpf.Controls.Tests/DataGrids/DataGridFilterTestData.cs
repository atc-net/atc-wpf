namespace Atc.Wpf.Controls.Tests.DataGrids;

internal static class DataGridFilterTestData
{
    public static List<TestOrder> Orders()
        =>
        [
            new() { Id = 1, Customer = "Alice", Country = "Denmark", Status = "Open", Amount = 10m, Date = new DateTime(2024, 1, 5, 10, 0, 0, DateTimeKind.Unspecified), Address = new TestAddress { City = "Aarhus" } },
            new() { Id = 2, Customer = "Bob", Country = "Germany", Status = "Closed", Amount = 2m, Date = new DateTime(2024, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), Address = new TestAddress { City = "Berlin" } },
            new() { Id = 3, Customer = null, Country = "Denmark", Status = "Open", Amount = 1234.5m, Date = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Unspecified) },
            new() { Id = 4, Customer = string.Empty, Country = "Sweden", Status = "Pending", Amount = 300m, Date = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Unspecified) },
            new() { Id = 5, Customer = "Carol", Country = "Germany", Status = "Open", Amount = 2m, Date = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Unspecified) },
            new() { Id = 6, Customer = "alice", Country = "Sweden", Status = "Closed", Amount = 75m, Date = new DateTime(2024, 4, 1, 0, 0, 0, DateTimeKind.Unspecified) },
        ];

    public static DataGrid CreateGrid(
        IEnumerable? itemsSource = null,
        bool isEnabled = true)
    {
        var grid = new DataGrid
        {
            AutoGenerateColumns = false,
            CanUserAddRows = false,
            Language = XmlLanguage.GetLanguage("en-US"),
            ItemsSource = itemsSource ?? Orders(),
        };

        grid.Columns.Add(new DataGridTextColumn { Header = "Id", Binding = new Binding(nameof(TestOrder.Id)) });
        grid.Columns.Add(new DataGridTextColumn { Header = "Customer", Binding = new Binding(nameof(TestOrder.Customer)) });
        grid.Columns.Add(new DataGridTextColumn { Header = "Country", Binding = new Binding(nameof(TestOrder.Country)) });
        grid.Columns.Add(new DataGridTextColumn { Header = "Status", Binding = new Binding(nameof(TestOrder.Status)) });
        grid.Columns.Add(new DataGridTextColumn { Header = "Amount", Binding = new Binding(nameof(TestOrder.Amount)) { StringFormat = "N2" } });
        grid.Columns.Add(new DataGridTextColumn { Header = "Date", Binding = new Binding(nameof(TestOrder.Date)) { StringFormat = "{0:yyyy-MM-dd}" } });
        grid.Columns.Add(new DataGridTextColumn { Header = "City", Binding = new Binding("Address.City") });

        DataGridFilter.SetSearchDelay(grid, TimeSpan.Zero);
        DataGridFilter.SetIsEnabled(grid, isEnabled);
        return grid;
    }

    public static DataGridColumn Column(
        DataGrid grid,
        string header)
        => grid.Columns.First(x => string.Equals((string)x.Header, header, StringComparison.Ordinal));

    public static List<int> VisibleIds(DataGrid grid)
        => grid.Items.OfType<TestOrder>().Select(x => x.Id).ToList();
}