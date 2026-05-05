namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class WindowPickerDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Watermark Text", "Content", 1)]
    [ObservableProperty]
    private string watermarkText = "Select window…";

    [PropertyDisplay("Show Refresh Button", "Behavior", 1)]
    [ObservableProperty]
    private bool showRefreshButton = true;

    [PropertyDisplay("Auto-refresh on hot-plug", "Behavior", 2)]
    [ObservableProperty]
    private bool autoRefreshOnDeviceChange = true;

    [PropertyDisplay("Clear value on disconnect", "Behavior", 3)]
    [ObservableProperty]
    private bool clearValueOnDisconnect;

    [PropertyDisplay("Auto-rebind on reconnect", "Behavior", 4)]
    [ObservableProperty]
    private bool autoRebindOnReconnect = true;

    [PropertyDisplay("Auto-select first available", "Behavior", 5)]
    [ObservableProperty]
    private bool autoSelectFirstAvailable;
}