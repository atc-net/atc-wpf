namespace Atc.Wpf.Sample.SamplesWpfControls.DataGrids;

public sealed class SampleOrder
{
    public int Id { get; init; }

    public string? Customer { get; init; }

    public string Country { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public decimal Amount { get; init; }

    public DateTime Date { get; init; }

    [SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Sample data.")]
    [SuppressMessage("Security", "S2245:Use a cryptographically strong random number generator", Justification = "Sample data.")]
    public static IList<SampleOrder> Create(
        int count,
        int seed)
    {
        string?[] customers =
        [
            "Contoso", "Fabrikam", "Northwind", "Adventure Works", "Tailspin Toys", "Wide World Importers",
            "Litware", "Proseware", "Woodgrove Bank", "Alpine Ski House", "Coho Winery", "Margie's Travel",
            null, string.Empty,
        ];
        string[] countries = ["Denmark", "Germany", "Sweden", "Norway", "Finland", "France", "Spain", "Italy"];
        string[] statuses = ["Open", "Pending", "Shipped", "Closed", "Cancelled"];

        var random = new Random(seed);
        var start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var orders = new List<SampleOrder>(count);
        for (var i = 1; i <= count; i++)
        {
            orders.Add(new SampleOrder
            {
                Id = i,
                Customer = customers[random.Next(customers.Length)],
                Country = countries[random.Next(countries.Length)],
                Status = statuses[random.Next(statuses.Length)],
                Amount = random.Next(100, 500_000) / 100m,
                Date = start.AddDays(random.Next(730)),
            });
        }

        return orders;
    }
}