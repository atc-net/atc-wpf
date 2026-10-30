namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

public static class MultiColumnSampleData
{
    private static readonly string[] Cities =
    [
        "Aarhus", "Berlin", "Copenhagen", "Dublin", "Edinburgh", "Frankfurt", "Gothenburg", "Hamburg", "Helsinki", "Oslo",
    ];

    private static readonly string[] Countries =
    [
        "Denmark", "Germany", "Denmark", "Ireland", "Scotland", "Germany", "Sweden", "Germany", "Finland", "Norway",
    ];

    private static readonly string[] Names =
    [
        "Alpha Logistics", "Bravo Foods", "Charlie Marine", "Delta Energy", "Echo Media", "Foxtrot Tools",
        "Golf Retail", "Hotel Systems", "India Textiles", "Juliet Pharma", "Kilo Freight", "Lima Robotics",
        "Mike Builders", "November Labs", "Oscar Optics", "Papa Bakery", "Quebec Steel", "Romeo Travel",
        "Sierra Software", "Tango Audio", "Uniform Wear", "Victor Motors", "Whiskey Cellars", "Xray Medical",
    ];

    private static readonly string[] Categories = ["Bearing", "Bolt", "Filter", "Gasket", "Hose", "Seal", "Sensor", "Valve"];

    public static IReadOnlyList<SampleCustomer> CreateCustomers()
        => Names
            .Select((name, i) => new SampleCustomer(
                (1001 + i).ToString(CultureInfo.InvariantCulture),
                name,
                Cities[i % Cities.Length],
                Countries[i % Countries.Length],
                name.Split(' ')[0].ToLowerInvariant() + "@example.com",
                isActive: i % 7 != 6))
            .ToList();

    public static IReadOnlyList<SamplePart> CreateParts(int count)
        => Enumerable.Range(1, count)
            .Select(i =>
            {
                var category = Categories[i % Categories.Length];
                return new SamplePart(
                    "P-" + i.ToString("D5", CultureInfo.InvariantCulture),
                    string.Create(CultureInfo.InvariantCulture, $"{category} {10 + (i % 90)} mm, type {(char)('A' + (i % 26))}"),
                    category,
                    (i * 37) % 500);
            })
            .ToList();

    public static IReadOnlyList<SampleUser> CreateUsers()
        =>
        [
            new("akj", "Anna K. Jensen", "Finance", "akj@example.com"),
            new("bmo", "Bo Mortensen", "Sales", "bmo@example.com"),
            new("cla", "Clara Larsen", "Engineering", "cla@example.com"),
            new("dni", "David Nielsen", "Engineering", "dni@example.com"),
            new("eha", "Emma Hansen", "Support", "eha@example.com"),
            new("fpe", "Frederik Petersen", "Sales", "fpe@example.com"),
            new("gso", "Greta Sørensen", "HR", "gso@example.com"),
        ];
}