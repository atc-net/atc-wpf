namespace Atc.Wpf.Hardware.Services;

public interface IProcessService : IDisposable
{
    ObservableCollection<RunningProcessInfo> Processes { get; }

    TimeSpan PollingInterval { get; set; }

    bool OnlyWithMainWindow { get; set; }

    void StartWatching();

    void StopWatching();

    Task RefreshAsync();
}