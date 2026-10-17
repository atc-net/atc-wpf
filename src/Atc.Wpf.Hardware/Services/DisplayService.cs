namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates the display monitors and polls for changes; the collection is only changed on the UI thread.
/// </summary>
public sealed class DisplayService : IDisplayService
{
    private readonly Func<IReadOnlyList<DisplaySnapshot>> enumerate;
    private readonly PollingServiceHost host;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayService"/> class.
    /// </summary>
    public DisplayService()
        : this(EnumerateDisplays)
    {
    }

    internal DisplayService(
        Func<IReadOnlyList<DisplaySnapshot>> enumerate,
        Dispatcher? uiDispatcher = null)
    {
        this.enumerate = enumerate;
        Displays = new ObservableCollection<DisplayInfo>();
        host = new PollingServiceHost(nameof(DisplayService), PollAsync, uiDispatcher);
    }

    /// <inheritdoc />
    public ObservableCollection<DisplayInfo> Displays { get; }

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
        var snapshots = enumerate();
        if (host.IsDisposed)
        {
            return;
        }

        // The collection is bound to the picker, so the sync has to run on the UI thread.
        await host
            .InvokeOnUiAsync(() =>
            {
                if (!host.IsDisposed)
                {
                    SyncWith(snapshots);
                }
            })
            .ConfigureAwait(false);
    }

    private static IReadOnlyList<DisplaySnapshot> EnumerateDisplays()
    {
        var snapshots = new List<DisplaySnapshot>();

        NativeMonitorMethods.EnumDisplayMonitors(
            hdc: IntPtr.Zero,
            lprcClip: IntPtr.Zero,
            lpfnEnum: (IntPtr hMonitor, IntPtr _, ref NativeMonitorMethods.NativeRect _, IntPtr _) =>
            {
                var info = NativeMonitorMethods.MonitorInfoEx.Default();
                if (NativeMonitorMethods.GetMonitorInfo(hMonitor, ref info))
                {
                    snapshots.Add(new DisplaySnapshot(
                        hMonitor,
                        info.szDevice,
                        ToRect(info.rcMonitor),
                        ToRect(info.rcWork),
                        IsPrimary: (info.dwFlags & NativeMonitorMethods.MONITORINFOF_PRIMARY) != 0));
                }

                return true;
            },
            dwData: IntPtr.Zero);

        return snapshots;
    }

    private void SyncWith(IReadOnlyList<DisplaySnapshot> snapshots)
    {
        var foundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var snapshot in snapshots)
        {
            foundIds.Add(snapshot.DeviceName);
            Upsert(snapshot);
        }

        for (var i = Displays.Count - 1; i >= 0; i--)
        {
            if (!foundIds.Contains(Displays[i].DeviceName))
            {
                Displays[i].State = DeviceState.Disconnected;
            }
        }
    }

    private void Upsert(DisplaySnapshot snapshot)
    {
        var existing = FindByDeviceName(snapshot.DeviceName);

        if (existing is not null)
        {
            if (existing.State is DeviceState.Disconnected)
            {
                existing.State = DeviceState.Available;
            }

            // Resolution, arrangement and the primary display can change while the monitor stays connected.
            existing.Bounds = snapshot.Bounds;
            existing.WorkingArea = snapshot.WorkingArea;
            existing.IsPrimary = snapshot.IsPrimary;
            return;
        }

        var display = new DisplayInfo(
            handle: snapshot.Handle,
            deviceName: snapshot.DeviceName,
            bounds: snapshot.Bounds,
            workingArea: snapshot.WorkingArea,
            isPrimary: snapshot.IsPrimary)
        {
            State = DeviceState.Available,
        };

        Displays.Add(display);
    }

    private static Rect ToRect(NativeMonitorMethods.NativeRect r)
        => new(
            x: r.Left,
            y: r.Top,
            width: r.Right - r.Left,
            height: r.Bottom - r.Top);

    private DisplayInfo? FindByDeviceName(string deviceName)
    {
        for (var i = 0; i < Displays.Count; i++)
        {
            if (string.Equals(Displays[i].DeviceName, deviceName, StringComparison.OrdinalIgnoreCase))
            {
                return Displays[i];
            }
        }

        return null;
    }
}