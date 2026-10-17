namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates the logical drives and polls for changes; the collection is only changed on the UI thread.
/// </summary>
public sealed class DriveService : IDriveService
{
    private static readonly TimeSpan JustConnectedDuration = TimeSpan.FromSeconds(3);

    private readonly Func<IReadOnlyList<(string Name, System.IO.DriveType DriveType)>> listDrives;
    private readonly Func<string, System.IO.DriveType, DriveSnapshot> readVolume;
    private readonly PollingServiceHost host;
    private bool initialEnumerationCompleted;

    /// <summary>
    /// Initializes a new instance of the <see cref="DriveService"/> class.
    /// </summary>
    public DriveService()
        : this(ListDrives, ReadVolume)
    {
    }

    internal DriveService(
        Func<IReadOnlyList<(string Name, System.IO.DriveType DriveType)>> listDrives,
        Func<string, System.IO.DriveType, DriveSnapshot> readVolume,
        Dispatcher? uiDispatcher = null)
    {
        this.listDrives = listDrives;
        this.readVolume = readVolume;
        Drives = new ObservableCollection<DiskDriveInfo>();
        host = new PollingServiceHost(nameof(DriveService), PollAsync, uiDispatcher);
    }

    /// <inheritdoc />
    public ObservableCollection<DiskDriveInfo> Drives { get; }

    /// <inheritdoc />
    public TimeSpan PollingInterval
    {
        get => host.Interval;
        set => host.Interval = value;
    }

    /// <inheritdoc />
    public void StartWatching()
        => host.Start();

    /// <inheritdoc />
    public void StopWatching()
        => host.Stop();

    /// <inheritdoc />
    public Task RefreshAsync()
        => host.RefreshAsync();

    /// <inheritdoc />
    public void Dispose()
        => host.Dispose();

    private async Task PollAsync()
    {
        var drives = listDrives();
        if (host.IsDisposed)
        {
            return;
        }

        // Upsert decides per drive whether to re-read the volume from the existing entry and starts the
        // just-connected DispatcherTimer, so the whole sync runs on the UI thread with the bound collection.
        await host
            .InvokeOnUiAsync(() =>
            {
                if (host.IsDisposed)
                {
                    return;
                }

                SyncWith(drives, isInitialEnumeration: !initialEnumerationCompleted);
                initialEnumerationCompleted = true;
            })
            .ConfigureAwait(false);
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

    private void SyncWith(
        IReadOnlyList<(string Name, System.IO.DriveType DriveType)> drives,
        bool isInitialEnumeration)
    {
        var foundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (name, driveType) in drives)
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