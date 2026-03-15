namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class BadgeDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Placement Mode", "Layout", 1)]
    [ObservableProperty]
    private BadgePlacementMode badgePlacementMode = BadgePlacementMode.TopRight;

    [PropertyDisplay("Badge Content", "Content", 1)]
    [ObservableProperty]
    private string badgeContent = "5";

    [PropertyDisplay("Is Dot", "Appearance", 1)]
    [ObservableProperty]
    private bool isDot;

    [PropertyDisplay("Is Badge Visible", "Appearance", 2)]
    [ObservableProperty]
    private bool isBadgeVisible = true;

    [PropertyDisplay("Hide When Zero", "Behavior", 1)]
    [ObservableProperty]
    private bool hideWhenZero;

    [PropertyDisplay("Badge Max Value", "Behavior", 2)]
    [PropertyRange(0, 999, 1)]
    [ObservableProperty]
    private int badgeMaxValue;

    [PropertyDisplay("Badge Font Size", "Appearance", 3)]
    [PropertyRange(6, 20, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double badgeFontSize = 10;

    [PropertyDisplay("Badge Corner Radius", "Appearance", 4)]
    [PropertyRange(0, 20, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double badgeCornerRadius = 8;
}