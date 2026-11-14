namespace Atc.Wpf.Sample.SamplesWpfControls.Clocks;

public partial class SevenSegmentDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Digit", "Value", 1)]
    [PropertyRange(0, 9, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private int digit = 8;

    [PropertyDisplay("Show Colon", "Behavior", 1)]
    [ObservableProperty]
    private bool showColon = true;

    [PropertyDisplay("Show Dot", "Behavior", 2)]
    [ObservableProperty]
    private bool showDot;

    [PropertyDisplay("Play Animation", "Behavior", 3)]
    [ObservableProperty]
    private bool playAnimation = true;

    [PropertyDisplay("Blur Radius", "Appearance", 1)]
    [PropertyRange(0, 30, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private int blurRadius = 5;

    [PropertyDisplay("Unlit Segment Opacity", "Appearance", 2)]
    [PropertyRange(0, 1, 0.05)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double numberMinimumOpacity = 0.05;

    [PropertyDisplay("Number", "Custom Colors", 1)]
    [ObservableProperty]
    private Color numberColor = Colors.LimeGreen;

    [PropertyDisplay("Colon", "Custom Colors", 2)]
    [ObservableProperty]
    private Color colonColor = Colors.LimeGreen;
}