namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class CardDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Elevation", "Appearance", 1)]
    [PropertyRange(0, 5, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private int elevation = 2;

    [PropertyDisplay("Show Header", "Layout", 1)]
    [ObservableProperty]
    private bool showHeader = true;

    [PropertyDisplay("Is Expandable", "Behavior", 1)]
    [ObservableProperty]
    private bool isExpandable;

    [PropertyDisplay("Is Expanded", "Behavior", 2)]
    [ObservableProperty]
    private bool isExpanded = true;

    [PropertyDisplay("Expander Button Location", "Behavior", 3)]
    [ObservableProperty]
    private ExpanderButtonLocation expanderButtonLocation = ExpanderButtonLocation.Left;

    [PropertyDisplay("Corner Radius", "Appearance", 2)]
    [PropertyRange(0, 30, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double cornerRadius = 4;

    [PropertyDisplay("Header", "Content", 1)]
    [ObservableProperty]
    private string header = "Card Header";

    [PropertyDisplay("Content", "Content", 2)]
    [PropertyEditorHint(EditorHint.MultiLineText)]
    [ObservableProperty]
    private string content = "Card content goes here.";
}