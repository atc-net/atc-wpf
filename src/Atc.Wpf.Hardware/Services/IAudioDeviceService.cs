namespace Atc.Wpf.Hardware.Services;

public interface IAudioDeviceService : IDisposable
{
    ObservableCollection<AudioDeviceInfo> Devices { get; }

    AudioDeviceKind Kind { get; }

    void StartWatching();

    void StopWatching();

    Task RefreshAsync();
}