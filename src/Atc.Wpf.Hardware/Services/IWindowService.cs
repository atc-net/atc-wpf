namespace Atc.Wpf.Hardware.Services;

public interface IWindowService : IDisposable
{
    ObservableCollection<TopLevelWindowInfo> Windows { get; }

    TimeSpan PollingInterval { get; set; }

    bool OnlyVisibleWithTitle { get; set; }

    void StartWatching();

    void StopWatching();

    Task RefreshAsync();
}