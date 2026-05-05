namespace Atc.Wpf.Hardware.Services;

public sealed class DriveService : IDriveService
{
    private static readonly TimeSpan JustConnectedDuration = TimeSpan.FromSeconds(3);

    private readonly DispatcherTimer pollTimer;
    private bool started;
    private bool initialEnumerationCompleted;
    private bool disposed;

    public DriveService()
    {
        Drives = new ObservableCollection<DiskDriveInfo>();
        pollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2),
        };
        pollTimer.Tick += OnPollTick;
    }

    public ObservableCollection<DiskDriveInfo> Drives { get; }

    public TimeSpan PollingInterval
    {
        get => pollTimer.Interval;
        set => pollTimer.Interval = value;
    }

    public void StartWatching()
    {
        if (started)
        {
            return;
        }

        started = true;
        pollTimer.Start();
    }

    public void StopWatching()
    {
        if (!started)
        {
            return;
        }

        started = false;
        pollTimer.Stop();
    }

    [SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait", Justification = "WPF service ties to the dispatcher.")]
    public Task RefreshAsync()
    {
        EnumerateAndSync(isInitialEnumeration: !initialEnumerationCompleted);
        initialEnumerationCompleted = true;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        pollTimer.Tick -= OnPollTick;
        pollTimer.Stop();
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Polling must not crash on transient drive enumeration errors.")]
    private void OnPollTick(
        object? sender,
        EventArgs e)
    {
        try
        {
            EnumerateAndSync(isInitialEnumeration: false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"DriveService poll failed: {ex.Message}");
        }
    }

    private void EnumerateAndSync(bool isInitialEnumeration)
    {
        var found = System.IO.DriveInfo.GetDrives();
        var foundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var drive in found)
        {
            foundIds.Add(drive.Name);
            UpsertFromDrive(drive, isInitialEnumeration);
        }

        for (var i = Drives.Count - 1; i >= 0; i--)
        {
            if (!foundIds.Contains(Drives[i].DeviceId))
            {
                Drives[i].State = DeviceState.Disconnected;
            }
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Removable drives without media throw on metadata access.")]
    private void UpsertFromDrive(
        System.IO.DriveInfo drive,
        bool isInitialEnumeration)
    {
        var existing = FindByDeviceId(drive.Name);

        if (existing is not null)
        {
            if (existing.State is DeviceState.Disconnected)
            {
                existing.State = DeviceState.Available;
            }

            return;
        }

        string label;
        bool isReady;
        long? totalSize = null;
        long? availableFreeSpace = null;

        try
        {
            isReady = drive.IsReady;
            label = isReady ? drive.VolumeLabel : drive.Name;

            if (isReady)
            {
                totalSize = drive.TotalSize;
                availableFreeSpace = drive.AvailableFreeSpace;
            }
        }
        catch (Exception)
        {
            isReady = false;
            label = drive.Name;
        }

        var newInfo = new DiskDriveInfo(
            deviceId: drive.Name,
            friendlyName: label,
            driveType: drive.DriveType,
            isReady: isReady,
            totalSize: totalSize,
            availableFreeSpace: availableFreeSpace)
        {
            State = isInitialEnumeration
                ? DeviceState.Available
                : DeviceState.JustConnected,
        };

        Drives.Add(newInfo);

        if (newInfo.State is DeviceState.JustConnected)
        {
            JustConnectedTimer.TransitionToAvailableAfter(
                state => newInfo.State = state,
                JustConnectedDuration);
        }
    }

    private DiskDriveInfo? FindByDeviceId(string deviceId)
    {
        for (var i = 0; i < Drives.Count; i++)
        {
            if (string.Equals(Drives[i].DeviceId, deviceId, StringComparison.OrdinalIgnoreCase))
            {
                return Drives[i];
            }
        }

        return null;
    }
}