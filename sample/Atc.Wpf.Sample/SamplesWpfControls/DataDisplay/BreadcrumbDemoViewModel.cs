namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class BreadcrumbDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Separator", "Appearance", 1)]
    [ObservableProperty]
    private string separator = "/";

    [PropertyDisplay("Overflow Mode", "Behavior", 1)]
    [ObservableProperty]
    private BreadcrumbOverflowMode overflowMode = BreadcrumbOverflowMode.None;

    [PropertyDisplay("Max Visible Items", "Behavior", 2)]
    [PropertyRange(0, 10, 1)]
    [ObservableProperty]
    private int maxVisibleItems;
}