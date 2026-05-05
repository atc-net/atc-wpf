namespace Atc.Wpf.Hardware.Services;

public interface IUsbCameraService : IDisposable
{
    ObservableCollection<UsbCameraInfo> Cameras { get; }

    void StartWatching();

    void StopWatching();

    Task RefreshAsync();
}