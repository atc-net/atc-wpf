namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public class CardDemoViewModel : ViewModelBase
{
    private int elevation = 2;
    private bool showHeader = true;
    private bool isExpandable;
    private bool isExpanded = true;
    private ExpanderButtonLocation expanderButtonLocation = ExpanderButtonLocation.Left;
    private double cornerRadius = 4;
    private string header = "Card Header";
    private string content = "Card content goes here.";

    [PropertyDisplay("Elevation", "Appearance", 1)]
    [PropertyRange(0, 5, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    public int Elevation
    {
        get => elevation;
        set => Set(ref elevation, value);
    }

    [PropertyDisplay("Show Header", "Layout", 1)]
    public bool ShowHeader
    {
        get => showHeader;
        set => Set(ref showHeader, value);
    }

    [PropertyDisplay("Is Expandable", "Behavior", 1)]
    public bool IsExpandable
    {
        get => isExpandable;
        set => Set(ref isExpandable, value);
    }

    [PropertyDisplay("Is Expanded", "Behavior", 2)]
    public bool IsExpanded
    {
        get => isExpanded;
        set => Set(ref isExpanded, value);
    }

    [PropertyDisplay("Expander Button Location", "Behavior", 3)]
    public ExpanderButtonLocation ExpanderButtonLocation
    {
        get => expanderButtonLocation;
        set => Set(ref expanderButtonLocation, value);
    }

    [PropertyDisplay("Corner Radius", "Appearance", 2)]
    [PropertyRange(0, 30, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double CornerRadius
    {
        get => cornerRadius;
        set => Set(ref cornerRadius, value);
    }

    [PropertyDisplay("Header", "Content", 1)]
    public string Header
    {
        get => header;
        set => Set(ref header, value);
    }

    [PropertyDisplay("Content", "Content", 2)]
    [PropertyEditorHint(EditorHint.MultiLineText)]
    public string Content
    {
        get => content;
        set => Set(ref content, value);
    }
}