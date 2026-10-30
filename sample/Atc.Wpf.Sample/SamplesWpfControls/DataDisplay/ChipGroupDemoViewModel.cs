namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

[SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation", Justification = "Bound to the ChipGroup dependency properties of these types.")]
public partial class ChipGroupDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Selection Mode", "Behavior", 1)]
    [ObservableProperty]
    private ChipSelectionMode selectionMode = ChipSelectionMode.Multiple;

    [PropertyDisplay("Show All Chip", "Behavior", 2)]
    [ObservableProperty]
    private bool showAllChip;

    [PropertyDisplay("Is Enabled", "Behavior", 3)]
    [ObservableProperty]
    private bool isControlEnabled = true;

    [PropertyDisplay("Chip Variant", "Appearance", 1)]
    [ObservableProperty]
    private ChipGroupVariant chipVariant = ChipGroupVariant.Filled;

    [PropertyDisplay("Chip Size", "Appearance", 2)]
    [ObservableProperty]
    private ChipSize chipSize = ChipSize.Medium;

    [PropertyDisplay("Show Check Mark", "Appearance", 3)]
    [ObservableProperty]
    private bool showCheckMark = true;

    [PropertyDisplay("Wrap", "Layout", 1)]
    [ObservableProperty]
    private bool wrap = true;

    [PropertyDisplay("Orientation", "Layout", 2)]
    [ObservableProperty]
    private Orientation orientation = Orientation.Horizontal;

    public ChipGroupDemoViewModel()
    {
        Fruits = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["apple"] = "Apple",
            ["banana"] = "Banana",
            ["cherry"] = "Cherry",
            ["date"] = "Date",
            ["elderberry"] = "Elderberry",
        };

        LogLevels = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["error"] = "Error",
            ["warning"] = "Warning",
            ["info"] = "Info",
            ["debug"] = "Debug",
        };
        LogLevelCounts = new Dictionary<string, int>(StringComparer.Ordinal)
        {
            ["error"] = 12,
            ["warning"] = 48,
            ["info"] = 1_204,
            ["debug"] = 7_810,
        };
        SelectedLogLevels = ["error", "warning"];

        Statuses = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["open"] = "Open",
            ["progress"] = "In progress",
            ["review"] = "In review",
            ["done"] = "Done",
        };
        SelectedStatuses = ["open"];

        Facets = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["laptops"] = "Laptops",
            ["desktops"] = "Desktops",
            ["monitors"] = "Monitors",
            ["keyboards"] = "Keyboards",
            ["mice"] = "Mice",
            ["headsets"] = "Headsets",
            ["webcams"] = "Webcams",
            ["docks"] = "Docking stations",
            ["cables"] = "Cables",
            ["storage"] = "Storage",
        };
        SelectedFacets = ["monitors", "docks"];
    }

    public Dictionary<string, string> Fruits { get; }

    public Dictionary<string, string> LogLevels { get; }

    public Dictionary<string, int> LogLevelCounts { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> SelectedLogLevels { get; set; }

    public Dictionary<string, string> Statuses { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> SelectedStatuses { get; set; }

    public Dictionary<string, string> Facets { get; }

    public System.Collections.ObjectModel.ObservableCollection<string> SelectedFacets { get; set; }

    public Dictionary<string, string> Toppings { get; } = new(StringComparer.Ordinal)
    {
        ["cheese"] = "Cheese",
        ["ham"] = "Ham",
        ["mushrooms"] = "Mushrooms",
        ["olives"] = "Olives",
        ["peppers"] = "Peppers",
        ["pineapple"] = "Pineapple",
    };

    public System.Collections.ObjectModel.ObservableCollection<string> SoldOutToppings { get; set; } = ["pineapple"];

    public System.Collections.ObjectModel.ObservableCollection<string> SelectedToppings { get; set; } = ["cheese"];
}