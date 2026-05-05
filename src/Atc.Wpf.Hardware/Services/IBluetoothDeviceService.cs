namespace Atc.Wpf.Hardware.Services;

public interface IBluetoothDeviceService : IDisposable
{
    ObservableCollection<BluetoothDeviceInfo> Devices { get; }

    void StartWatching();

    void StopWatching();

    Task RefreshAsync();
}