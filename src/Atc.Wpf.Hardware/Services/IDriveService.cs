namespace Atc.Wpf.Hardware.Services;

public interface IDriveService : IDisposable
{
    ObservableCollection<DiskDriveInfo> Drives { get; }

    TimeSpan PollingInterval { get; set; }

    void StartWatching();

    void StopWatching();

    Task RefreshAsync();
}