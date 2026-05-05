namespace Atc.Wpf.Hardware.Services;

public interface IUsbDeviceService : IDisposable
{
    ObservableCollection<UsbDeviceInfo> Devices { get; }

    UsbDeviceClassFilter ClassFilter { get; set; }

    void StartWatching();

    void StopWatching();

    Task RefreshAsync();
}