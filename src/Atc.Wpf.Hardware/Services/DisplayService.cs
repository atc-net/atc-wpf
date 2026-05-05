namespace Atc.Wpf.Hardware.Services;

public sealed class DisplayService : IDisplayService
{
    private readonly DispatcherTimer pollTimer;
    private bool started;
    private bool disposed;

    public DisplayService()
    {
        Displays = new ObservableCollection<DisplayInfo>();
        pollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2),
        };
        pollTimer.Tick += OnPollTick;
    }

    public ObservableCollection<DisplayInfo> Displays { get; }

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

    public Task RefreshAsync()
    {
        EnumerateAndSync();
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

    private void EnumerateAndSync()
    {
        var foundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        NativeMonitorMethods.EnumDisplayMonitors(
            hdc: IntPtr.Zero,
            lprcClip: IntPtr.Zero,
            lpfnEnum: (IntPtr hMonitor, IntPtr _, ref NativeMonitorMethods.NativeRect _, IntPtr _) =>
            {
                var info = NativeMonitorMethods.MonitorInfoEx.Default();
                if (!NativeMonitorMethods.GetMonitorInfo(hMonitor, ref info))
                {
                    return true;
                }

                var deviceName = info.szDevice;
                foundIds.Add(deviceName);
                UpsertFromMonitor(hMonitor, info);
                return true;
            },
            dwData: IntPtr.Zero);

        for (var i = Displays.Count - 1; i >= 0; i--)
        {
            if (!foundIds.Contains(Displays[i].DeviceName))
            {
                Displays[i].State = DeviceState.Disconnected;
            }
        }
    }

    private void UpsertFromMonitor(
        IntPtr hMonitor,
        NativeMonitorMethods.MonitorInfoEx info)
    {
        var existing = FindByDeviceName(info.szDevice);

        if (existing is not null)
        {
            if (existing.State is DeviceState.Disconnected)
            {
                existing.State = DeviceState.Available;
            }

            return;
        }

        var bounds = ToRect(info.rcMonitor);
        var workingArea = ToRect(info.rcWork);
        var isPrimary = (info.dwFlags & NativeMonitorMethods.MONITORINFOF_PRIMARY) != 0;

        var display = new DisplayInfo(
            handle: hMonitor,
            deviceName: info.szDevice,
            bounds: bounds,
            workingArea: workingArea,
            isPrimary: isPrimary)
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