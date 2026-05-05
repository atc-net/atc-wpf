namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class UsbPortPickerDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Watermark Text", "Content", 1)]
    [ObservableProperty]
    private string watermarkText = "Select USB device…";

    [PropertyDisplay("Class Filter", "Behavior", 1)]
    [ObservableProperty]
    private UsbDeviceClassFilter classFilter = UsbDeviceClassFilter.None;

    [PropertyDisplay("Show Refresh Button", "Behavior", 2)]
    [ObservableProperty]
    private bool showRefreshButton = true;

    [PropertyDisplay("Auto-refresh on hot-plug", "Behavior", 3)]
    [ObservableProperty]
    private bool autoRefreshOnDeviceChange = true;

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