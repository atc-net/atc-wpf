namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

[SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation", Justification = "Bound to the MultiSelectComboBox dependency properties of these types.")]
public partial class MultiSelectComboBoxDemoViewModel : ViewModelBase
{
    private int nextSourceSystem = 4;

    [PropertyDisplay("Max Names In Summary", "Summary", 1)]
    [ObservableProperty]
    private int maxNamesInSummary = 2;

    [PropertyDisplay("None Text", "Summary", 2)]
    [ObservableProperty]
    private string noneText = string.Empty;

    [PropertyDisplay("Show Select All Row", "Pop-up", 1)]
    [ObservableProperty]
    private bool showSelectAllRow = true;

    [PropertyDisplay("Show Clear Button", "Pop-up", 2)]
    [ObservableProperty]
    private bool showClearButton = true;

    [PropertyDisplay("Is Enabled", "Behavior", 1)]
    [ObservableProperty]
    private bool isControlEnabled = true;

    [ObservableProperty]
    private Dictionary<string, string> liveSourceSystems;

    public MultiSelectComboBoxDemoViewModel()
    {
        SourceSystems = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["orders"] = "Orders",
            ["payments"] = "Payments",
            ["shipping"] = "Shipping",
        };

        liveSourceSystems = new Dictionary<string, string>(SourceSystems, StringComparer.Ordinal);
        SelectedLiveSourceSystems = [];
        SelectedAllSourceSystems = ["orders", "payments", "shipping"];

        LogLevels = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["trace"] = "Trace",
            ["debug"] = "Debug",
            ["information"] = "Information",
            ["warning"] = "Warning",
            ["error"] = "Error",
            ["critical"] = "Critical",
        };
        SelectedLogLevels = ["information", "warning", "error", "critical"];

        Countries = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(x => new RegionInfo(x.Name))
            .DistinctBy(x => x.TwoLetterISORegionName, StringComparer.Ordinal)
            .OrderBy(x => x.EnglishName, StringComparer.Ordinal)
            .ToDictionary(x => x.TwoLetterISORegionName, x => x.EnglishName, StringComparer.Ordinal);
        SelectedCountries = ["DK", "SE", "NO"];

        ManyItems = Enumerable.Range(1, 1_000)
            .ToDictionary(
                i => i.ToString(CultureInfo.InvariantCulture),
                i => string.Create(CultureInfo.InvariantCulture, $"Item {i:0000}"),
                StringComparer.Ordinal);
    }

    public Dictionary<string, string> SourceSystems { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> SelectedLiveSourceSystems { get; set; }

    public Dictionary<string, string> LogLevels { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> SelectedAllSourceSystems { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<string> SelectedLogLevels { get; set; }

    public Dictionary<string, string> Countries { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> SelectedCountries { get; set; }

    public Dictionary<string, string> ManyItems { get; }

    [RelayCommand]
    private void AddSourceSystem()
    {
        var key = string.Create(CultureInfo.InvariantCulture, $"system{nextSourceSystem}");
        var items = new Dictionary<string, string>(LiveSourceSystems, StringComparer.Ordinal)
        {
            [key] = string.Create(CultureInfo.InvariantCulture, $"System {nextSourceSystem}"),
        };

        nextSourceSystem++;
        LiveSourceSystems = items;
    }

    [RelayCommand]
    private void RemoveFirstSourceSystem()
    {
        if (LiveSourceSystems.Count == 0)
        {
            return;
        }

        var items = new Dictionary<string, string>(LiveSourceSystems, StringComparer.Ordinal);
        items.Remove(items.Keys.First());
        LiveSourceSystems = items;
    }
}