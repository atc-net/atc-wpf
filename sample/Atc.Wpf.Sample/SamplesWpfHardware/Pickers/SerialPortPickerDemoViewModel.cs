namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class SerialPortPickerDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Watermark Text", "Content", 1)]
    [ObservableProperty]
    private string watermarkText = "Select serial port…";

    [PropertyDisplay("Show Refresh Button", "Behavior", 1)]
    [ObservableProperty]
    private bool showRefreshButton = true;

    [PropertyDisplay("Auto-refresh on hot-plug", "Behavior", 2)]
    [ObservableProperty]
    private bool autoRefreshOnDeviceChange = true;

    [PropertyDisplay("Detect in-use state (probe)", "Behavior", 3)]
    [ObservableProperty]
    private bool detectInUseState;

    [PropertyDisplay("Clear value on disconnect", "Behavior", 4)]
    [ObservableProperty]
    private bool clearValueOnDisconnect;

    [PropertyDisplay("Auto-rebind on reconnect", "Behavior", 5)]
    [ObservableProperty]
    private bool autoRebindOnReconnect = true;

    [PropertyDisplay("Auto-select first available", "Behavior", 6)]
    [ObservableProperty]
    private bool autoSelectFirstAvailable;
}