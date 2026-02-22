namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public class BreadcrumbDemoViewModel : ViewModelBase
{
    private string separator = "/";
    private BreadcrumbOverflowMode overflowMode = BreadcrumbOverflowMode.None;
    private int maxVisibleItems;

    [PropertyDisplay("Separator", "Appearance", 1)]
    public string Separator
    {
        get => separator;
        set => Set(ref separator, value);
    }

    [PropertyDisplay("Overflow Mode", "Behavior", 1)]
    public BreadcrumbOverflowMode OverflowMode
    {
        get => overflowMode;
        set => Set(ref overflowMode, value);
    }

    [PropertyDisplay("Max Visible Items", "Behavior", 2)]
    [PropertyRange(0, 10, 1)]
    public int MaxVisibleItems
    {
        get => maxVisibleItems;
        set => Set(ref maxVisibleItems, value);
    }
}