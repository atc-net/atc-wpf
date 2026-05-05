namespace Atc.Wpf.Hardware.Services;

public interface INetworkAdapterService : IDisposable
{
    ObservableCollection<NetworkAdapterInfo> Adapters { get; }

    TimeSpan PollingInterval { get; set; }

    bool IncludeLoopback { get; set; }

    void StartWatching();

    void StopWatching();

    Task RefreshAsync();
}