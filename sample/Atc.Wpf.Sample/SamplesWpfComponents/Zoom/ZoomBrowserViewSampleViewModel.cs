namespace Atc.Wpf.Sample.SamplesWpfComponents.Zoom;

public partial class ZoomBrowserViewSampleViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool showToolbar = true;

    [ObservableProperty]
    private bool showMiniMap = true;

    [ObservableProperty]
    private bool showStatusBar = true;
}