namespace Atc.Wpf.Hardware.Services;

public interface ISerialPortService : IDisposable
{
    ObservableCollection<SerialPortInfo> Ports { get; }

    void StartWatching();

    void StopWatching();

    Task RefreshAsync();

    Task<bool> ProbeInUseAsync(SerialPortInfo port);
}