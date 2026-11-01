namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

[SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation", Justification = "Bound to the TokenBox dependency properties of these types.")]
public partial class TokenBoxDemoViewModel : ViewModelBase
{
    private static readonly string[] FirstNames =
    [
        "Anna", "Bo", "Carla", "David", "Emma", "Frederik", "Greta", "Hans", "Ida", "Jens",
        "Jennifer", "Karen", "Lars", "Maja", "Niels", "Olivia", "Peter", "Rikke", "Søren", "Tove",
    ];

    private static readonly string[] LastNames =
    [
        "Andersen", "Berg", "Christensen", "Dahl", "Eriksen", "Frost", "Hansen", "Jensen", "Kristensen", "Larsen",
        "Madsen", "Nielsen", "Olsen", "Pedersen", "Rasmussen", "Schmidt", "Thomsen", "Vestergaard", "Winther", "Østergaard",
    ];

    [PropertyDisplay("Allow Create", "Behavior", 1)]
    [ObservableProperty]
    private bool allowCreate;

    [PropertyDisplay("Tokens Wrap", "Layout", 1)]
    [ObservableProperty]
    private bool tokensWrap = true;

    [PropertyDisplay("Show Clear Button", "Layout", 2)]
    [ObservableProperty]
    private bool showClearButton;

    [PropertyDisplay("Filter Mode", "Suggestions", 1)]
    [ObservableProperty]
    private TokenFilterMode filterMode = TokenFilterMode.Contains;

    [PropertyDisplay("Minimum Prefix Length", "Suggestions", 2)]
    [ObservableProperty]
    private int minimumPrefixLength = 1;

    [PropertyDisplay("Is Enabled", "Behavior", 2)]
    [ObservableProperty]
    private bool isControlEnabled = true;

    public TokenBoxDemoViewModel()
    {
        SourceSystems = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["orders"] = "Orders",
            ["payments"] = "Payments",
            ["shipping"] = "Shipping",
            ["billing"] = "Billing",
            ["returns"] = "Returns",
            ["inventory"] = "Inventory",
            ["invoicing"] = "Invoicing",
            ["customers"] = "Customers",
        };

        KnownTags = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["urgent"] = "urgent",
            ["bug"] = "bug",
            ["feature"] = "feature",
            ["documentation"] = "documentation",
            ["question"] = "question",
        };
        Tags = ["bug"];

        Countries = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(x => new RegionInfo(x.Name))
            .DistinctBy(x => x.TwoLetterISORegionName, StringComparer.Ordinal)
            .OrderBy(x => x.EnglishName, StringComparer.Ordinal)
            .ToDictionary(x => x.TwoLetterISORegionName, x => x.EnglishName, StringComparer.Ordinal);
        OneLineCountries = ["DK", "SE", "NO", "FI", "IS", "DE", "NL"];
        WrapCountries = ["DK", "SE", "NO", "FI", "IS", "DE", "NL"];

        Users = Enumerable.Range(0, 10_000)
            .Select(i => new KeyValuePair<string, string>(
                string.Create(CultureInfo.InvariantCulture, $"user{i:00000}"),
                string.Create(CultureInfo.InvariantCulture, $"{FirstNames[i % FirstNames.Length]} {LastNames[(i / FirstNames.Length) % LastNames.Length]} {i / 400:00}")))
            .ToList();
        AssignedUsers = [];

        FixedSourceSystems = ["orders"];
        SelectedFixedSourceSystems = ["orders", "payments"];
        Emails = [];
    }

    public Dictionary<string, string> SourceSystems { get; }

    public Dictionary<string, string> KnownTags { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> Tags { get; set; }

    public Dictionary<string, string> Countries { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> OneLineCountries { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<string> WrapCountries { get; set; }

    public IReadOnlyList<KeyValuePair<string, string>> Users { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> AssignedUsers { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<string> FixedSourceSystems { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<string> SelectedFixedSourceSystems { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<string> Emails { get; set; }

    /// <summary>
    /// Gets the lookup of the 10,000 users: it simulates a remote call of 400 ms and returns the first 50 matches.
    /// </summary>
    public Func<string, CancellationToken, Task<IReadOnlyList<KeyValuePair<string, string>>>> LookupUsers
        => LookupUsersAsync;

    /// <summary>
    /// Gets the e-mail check: a value without "@" is refused with an error text.
    /// </summary>
    public Func<string, string?> ValidateEmail
        => static x => x.Contains('@', StringComparison.Ordinal) && x.Contains('.', StringComparison.Ordinal)
            ? null
            : "Not an e-mail address";

    private async Task<IReadOnlyList<KeyValuePair<string, string>>> LookupUsersAsync(
        string text,
        CancellationToken cancellationToken)
    {
        await Task.Delay(400, cancellationToken).ConfigureAwait(false);
        return FindUsers(text);
    }

    private List<KeyValuePair<string, string>> FindUsers(string text)
        => Users
            .Where(x => x.Value.Contains(text, StringComparison.OrdinalIgnoreCase))
            .Take(50)
            .ToList();
}