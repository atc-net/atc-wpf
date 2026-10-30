namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

[SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation", Justification = "Bound to the CardSelector dependency properties of these types.")]
public partial class CardSelectorDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Selection Mode", "Behavior", 1)]
    [ObservableProperty]
    private Atc.Wpf.Controls.Selectors.CardSelectionMode selectionMode = Atc.Wpf.Controls.Selectors.CardSelectionMode.Multiple;

    [PropertyDisplay("Is Enabled", "Behavior", 2)]
    [ObservableProperty]
    private bool isControlEnabled = true;

    [PropertyDisplay("Show Check Mark", "Appearance", 1)]
    [ObservableProperty]
    private bool showCheckMark = true;

    [PropertyDisplay("Card Width", "Layout", 1)]
    [ObservableProperty]
    private double cardWidth = 180d;

    [PropertyDisplay("Item Spacing", "Layout", 2)]
    [ObservableProperty]
    private double itemSpacing = 8d;

    public CardSelectorDemoViewModel()
    {
        Modules =
        [
            new("reports", "Reports", "Scheduled and ad-hoc reports", ""),
            new("alarms", "Alarms", "Thresholds and notifications", ""),
            new("maps", "Maps", "Sites on an interactive map", ""),
            new("audit", "Audit log", "Who changed what and when", ""),
            new("export", "Export", "CSV, Excel and PDF export", ""),
        ];

        ReportTemplates =
        [
            new("summary", "Summary", "One page with the key figures", ""),
            new("detailed", "Detailed", "Every row, grouped by site", ""),
            new("trend", "Trend", "Charts over the last 12 months", ""),
            new("compare", "Comparison", "Two periods side by side", ""),
        ];
        SelectedReportTemplates = ["summary", "trend"];

        Plans =
        [
            new("free", "Free", "1 user · 1 site", "", "0 kr / month"),
            new("team", "Team", "10 users · 5 sites", "", "199 kr / month"),
            new("business", "Business", "50 users · unlimited sites", "", "799 kr / month"),
        ];
        SelectedPlans = ["team"];

        Devices =
        [
            new("cam-entrance", "Entrance camera", "Online", "", isAvailable: true),
            new("cam-yard", "Yard camera", "Offline", "", isAvailable: false),
            new("sensor-hall", "Hall sensor", "Online", "", isAvailable: true),
            new("sensor-roof", "Roof sensor", "Offline", "", isAvailable: false),
            new("gateway", "Gateway", "Online", "", isAvailable: true),
        ];
    }

    public ObservableCollection<CardOption> Modules { get; }

    public ObservableCollection<string> SelectedModules { get; set; } = [];

    public ObservableCollection<CardOption> ReportTemplates { get; }

    public ObservableCollection<string> SelectedReportTemplates { get; set; }

    public ObservableCollection<CardOption> Plans { get; }

    public ObservableCollection<string> SelectedPlans { get; set; }

    public ObservableCollection<CardOption> Devices { get; }

    public ObservableCollection<string> SelectedDevices { get; set; } = [];
}