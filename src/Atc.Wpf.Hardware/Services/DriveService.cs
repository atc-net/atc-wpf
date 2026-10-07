namespace Atc.Wpf.Hardware.Services;

public sealed class DriveService : IDriveService
{
    private static readonly TimeSpan JustConnectedDuration = TimeSpan.FromSeconds(3);

    private readonly Func<IReadOnlyList<(string Name, System.IO.DriveType DriveType)>> listDrives;
    private readonly Func<string, System.IO.DriveType, DriveSnapshot> readVolume;
    private readonly DispatcherTimer pollTimer;
    private bool started;
    private bool initialEnumerationCompleted;
    private bool disposed;

    public DriveService()
        : this(ListDrives, ReadVolume)
    {
    }

    internal DriveService(
        Func<IReadOnlyList<(string Name, System.IO.DriveType DriveType)>> listDrives,
        Func<string, System.IO.DriveType, DriveSnapshot> readVolume)
    {
        this.listDrives = listDrives;
        this.readVolume = readVolume;
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

    private static IReadOnlyList<(string Name, System.IO.DriveType DriveType)> ListDrives()
        => System.IO.DriveInfo
            .GetDrives()
            .Select(d => (d.Name, d.DriveType))
            .ToList();

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Removable drives without media throw on metadata access.")]
    private static DriveSnapshot ReadVolume(
        string name,
        System.IO.DriveType driveType)
    {
        var drive = new System.IO.DriveInfo(name);

        try
        {
            var isReady = drive.IsReady;
            return isReady
                ? new DriveSnapshot(name, drive.VolumeLabel, driveType, IsReady: true, drive.TotalSize, drive.AvailableFreeSpace)
                : new DriveSnapshot(name, name, driveType, IsReady: false, TotalSize: null, AvailableFreeSpace: null);
        }
        catch (Exception)
        {
            return new DriveSnapshot(name, name, driveType, IsReady: false, TotalSize: null, AvailableFreeSpace: null);
        }
    }

    private void EnumerateAndSync(bool isInitialEnumeration)
    {
        var foundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (name, driveType) in listDrives())
        {
            foundIds.Add(name);
            Upsert(name, driveType, isInitialEnumeration);
        }

        for (var i = Drives.Count - 1; i >= 0; i--)
        {
            if (!foundIds.Contains(Drives[i].DeviceId))
            {
                Drives[i].State = DeviceState.Disconnected;
            }
        }
    }

    private void Upsert(
        string name,
        System.IO.DriveType driveType,
        bool isInitialEnumeration)
    {
        var existing = FindByDeviceId(name);

        if (existing is null)
        {
            AddNew(readVolume(name, driveType), isInitialEnumeration);
            return;
        }

        // Volume metadata of an offline network share or a spinning-up optical drive can block for seconds,
        // so known drives of those kinds are not re-read on every poll.
        if (driveType is System.IO.DriveType.Network or System.IO.DriveType.CDRom)
        {
            MarkAvailable(existing);
            return;
        }

        var snapshot = readVolume(name, driveType);
        if (IsSameVolume(existing, snapshot))
        {
            MarkAvailable(existing);
            existing.AvailableFreeSpace = snapshot.AvailableFreeSpace;
            return;
        }

        // Another volume is now mounted on this drive letter (e.g. a different USB stick).
        AddNew(snapshot, isInitialEnumeration);
        RemoveStale(existing);
    }

    private static void MarkAvailable(DiskDriveInfo drive)
    {
        if (drive.State is DeviceState.Disconnected)
        {
            drive.State = DeviceState.Available;
        }
    }

    private static bool IsSameVolume(
        DiskDriveInfo existing,
        DriveSnapshot snapshot)
        => existing.DriveType == snapshot.DriveType &&
           existing.IsReady == snapshot.IsReady &&
           existing.TotalSize == snapshot.TotalSize &&
           string.Equals(existing.FriendlyName, snapshot.Label, StringComparison.Ordinal);

    private void AddNew(
        DriveSnapshot snapshot,
        bool isInitialEnumeration)
    {
        var newInfo = new DiskDriveInfo(
            deviceId: snapshot.Name,
            friendlyName: snapshot.Label,
            driveType: snapshot.DriveType,
            isReady: snapshot.IsReady,
            totalSize: snapshot.TotalSize,
            availableFreeSpace: snapshot.AvailableFreeSpace)
        {
            State = isInitialEnumeration
                ? DeviceState.Available
                : DeviceState.JustConnected,
        };

        Drives.Add(newInfo);

        if (newInfo.State is DeviceState.JustConnected)
        {
            JustConnectedTimer.TransitionToAvailableAfter(
                () => newInfo.State,
                state => newInfo.State = state,
                JustConnectedDuration);
        }
    }

    /// <summary>
    /// Retires an entry whose id now belongs to something else. Called after the fresh entry has been added,
    /// so a picker that still has the stale entry selected sees it disconnect and leave - and does not
    /// auto-rebind to the newcomer just because it reuses the same id.
    /// </summary>
    private void RemoveStale(DiskDriveInfo stale)
    {
        stale.State = DeviceState.Disconnected;
        Drives.Remove(stale);
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