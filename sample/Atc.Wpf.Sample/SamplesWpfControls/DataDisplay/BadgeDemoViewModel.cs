namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public class BadgeDemoViewModel : ViewModelBase
{
    private BadgePlacementMode badgePlacementMode = BadgePlacementMode.TopRight;
    private string badgeContent = "5";
    private bool isDot;
    private bool isBadgeVisible = true;
    private bool hideWhenZero;
    private int badgeMaxValue;
    private double badgeFontSize = 10;
    private double badgeCornerRadius = 8;

    [PropertyDisplay("Placement Mode", "Layout", 1)]
    public BadgePlacementMode BadgePlacementMode
    {
        get => badgePlacementMode;
        set => Set(ref badgePlacementMode, value);
    }

    [PropertyDisplay("Badge Content", "Content", 1)]
    public string BadgeContent
    {
        get => badgeContent;
        set => Set(ref badgeContent, value);
    }

    [PropertyDisplay("Is Dot", "Appearance", 1)]
    public bool IsDot
    {
        get => isDot;
        set => Set(ref isDot, value);
    }

    [PropertyDisplay("Is Badge Visible", "Appearance", 2)]
    public bool IsBadgeVisible
    {
        get => isBadgeVisible;
        set => Set(ref isBadgeVisible, value);
    }

    [PropertyDisplay("Hide When Zero", "Behavior", 1)]
    public bool HideWhenZero
    {
        get => hideWhenZero;
        set => Set(ref hideWhenZero, value);
    }

    [PropertyDisplay("Badge Max Value", "Behavior", 2)]
    [PropertyRange(0, 999, 1)]
    public int BadgeMaxValue
    {
        get => badgeMaxValue;
        set => Set(ref badgeMaxValue, value);
    }

    [PropertyDisplay("Badge Font Size", "Appearance", 3)]
    [PropertyRange(6, 20, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double BadgeFontSize
    {
        get => badgeFontSize;
        set => Set(ref badgeFontSize, value);
    }

    [PropertyDisplay("Badge Corner Radius", "Appearance", 4)]
    [PropertyRange(0, 20, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double BadgeCornerRadius
    {
        get => badgeCornerRadius;
        set => Set(ref badgeCornerRadius, value);
    }
}