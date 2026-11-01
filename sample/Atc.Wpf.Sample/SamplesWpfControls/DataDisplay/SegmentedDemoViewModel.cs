namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

[SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation", Justification = "Bound to the Segmented.SelectedKeys dependency property of this type.")]
public partial class SegmentedDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Selected Index", "Behavior", 1)]
    [PropertyRange(-1, 5, 1)]
    [ObservableProperty]
    private int selectedIndex;

    [PropertyDisplay("Selection Mode", "Behavior", 2)]
    [ObservableProperty]
    private SegmentedSelectionMode selectionMode = SegmentedSelectionMode.Single;

    [PropertyDisplay("Corner Radius", "Appearance", 1)]
    [PropertyRange(0, 20, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double cornerRadius = 4;

    [PropertyDisplay("Size", "Appearance", 2)]
    [ObservableProperty]
    private SegmentedSize size = SegmentedSize.Medium;

    [PropertyDisplay("Equal Segment Width", "Layout", 1)]
    [ObservableProperty]
    private bool equalSegmentWidth = true;

    [PropertyDisplay("Orientation", "Layout", 2)]
    [ObservableProperty]
    private Orientation orientation = Orientation.Horizontal;

    [PropertyDisplay("Is Joined", "Layout", 3)]
    [ObservableProperty]
    private bool isJoined = true;

    public SegmentedDemoViewModel()
    {
        TextStyleKeys.CollectionChanged += (_, _) => RaisePropertyChanged(nameof(TextStyleSummary));
    }

    public System.Collections.ObjectModel.ObservableCollection<string> WeekdayKeys { get; set; } = [];

    public System.Collections.ObjectModel.ObservableCollection<string> TextStyleKeys { get; set; } = ["bold"];

    public string TextStyleSummary
        => TextStyleKeys.Count == 0
            ? "Plain text"
            : "Text style: " + string.Join(" + ", TextStyleKeys);

    public System.Collections.ObjectModel.ObservableCollection<string> MapLayerKeys { get; set; } = ["roads", "traffic"];

    public System.Collections.ObjectModel.ObservableCollection<string> ChannelKeys { get; set; } = ["email", "push"];
}