namespace Atc.Wpf.Hardware.Services;

public interface IDisplayService : IDisposable
{
    ObservableCollection<DisplayInfo> Displays { get; }

    TimeSpan PollingInterval { get; set; }

    void StartWatching();

    void StopWatching();

    Task RefreshAsync();
}