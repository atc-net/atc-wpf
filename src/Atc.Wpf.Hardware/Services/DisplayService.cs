namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates the display monitors and polls for changes on the dispatcher.
/// </summary>
public sealed class DisplayService : IDisplayService
{
    private readonly Func<IReadOnlyList<DisplaySnapshot>> enumerate;
    private readonly DispatcherTimer pollTimer;
    private bool started;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayService"/> class.
    /// </summary>
    public DisplayService()
        : this(EnumerateDisplays)
    {
    }

    internal DisplayService(Func<IReadOnlyList<DisplaySnapshot>> enumerate)
    {
        this.enumerate = enumerate;
        Displays = new ObservableCollection<DisplayInfo>();
        pollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2),
        };
        pollTimer.Tick += OnPollTick;
    }

    /// <inheritdoc />
    public ObservableCollection<DisplayInfo> Displays { get; }

    /// <inheritdoc />
    public TimeSpan PollingInterval
    {
        get => pollTimer.Interval;
        set => pollTimer.Interval = value;
    }

    /// <inheritdoc />
    public void StartWatching()
    {
        if (started)
        {
            return;
        }

        started = true;
        pollTimer.Start();
    }

    /// <inheritdoc />
    public void StopWatching()
    {
        if (!started)
        {
            return;
        }

        started = false;
        pollTimer.Stop();
    }

    /// <inheritdoc />
    public Task RefreshAsync()
    {
        EnumerateAndSync();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
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

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Polling must not crash on transient enumeration errors.")]
    private void OnPollTick(
        object? sender,
        EventArgs e)
    {
        try
        {
            EnumerateAndSync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"DisplayService poll failed: {ex.Message}");
        }
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

    private void EnumerateAndSync()
    {
        var foundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var snapshot in enumerate())
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