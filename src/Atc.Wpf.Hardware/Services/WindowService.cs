namespace Atc.Wpf.Hardware.Services;

public sealed class WindowService : IWindowService
{
    private readonly DispatcherTimer pollTimer;
    private bool started;
    private bool disposed;

    public WindowService()
    {
        Windows = new ObservableCollection<TopLevelWindowInfo>();
        pollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2),
        };
        pollTimer.Tick += OnPollTick;
    }

    public ObservableCollection<TopLevelWindowInfo> Windows { get; }

    public TimeSpan PollingInterval
    {
        get => pollTimer.Interval;
        set => pollTimer.Interval = value;
    }

    public bool OnlyVisibleWithTitle { get; set; } = true;

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

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Polling must not crash on enumeration errors.")]
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
            Debug.WriteLine($"WindowService poll failed: {ex.Message}");
        }
    }

    private void EnumerateAndSync()
    {
        var foundHandles = new HashSet<IntPtr>();

        NativeWindowMethods.EnumWindows(
            (hWnd, _) =>
            {
                if (OnlyVisibleWithTitle)
                {
                    if (!NativeWindowMethods.IsWindowVisible(hWnd))
                    {
                        return true;
                    }

                    if (NativeWindowMethods.GetWindowTextLength(hWnd) <= 0)
                    {
                        return true;
                    }
                }

                foundHandles.Add(hWnd);
                UpsertFromHandle(hWnd);
                return true;
            },
            lParam: IntPtr.Zero);

        for (var i = Windows.Count - 1; i >= 0; i--)
        {
            if (!foundHandles.Contains(Windows[i].Handle))
            {
                Windows[i].State = DeviceState.Disconnected;
            }
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Process metadata access can throw.")]
    private void UpsertFromHandle(IntPtr hWnd)
    {
        var existing = FindByHandle(hWnd);

        if (existing is not null)
        {
            if (existing.State is DeviceState.Disconnected)
            {
                existing.State = DeviceState.Available;
            }

            return;
        }

        var title = NativeWindowMethods.ReadWindowTitle(hWnd);
        var className = NativeWindowMethods.ReadClassName(hWnd);

        _ = NativeWindowMethods.GetWindowThreadProcessId(hWnd, out var pid);

        var processName = "(unknown)";
        try
        {
            using var p = System.Diagnostics.Process.GetProcessById((int)pid);
            processName = p.ProcessName;
        }
        catch (Exception)
        {
            // Process may have exited between EnumWindows and lookup.
        }

        var info = new TopLevelWindowInfo(
            handle: hWnd,
            title: title,
            className: className,
            processId: (int)pid,
            processName: processName)
        {
            State = DeviceState.Available,
        };

        Windows.Add(info);
    }

    private TopLevelWindowInfo? FindByHandle(IntPtr handle)
    {
        for (var i = 0; i < Windows.Count; i++)
        {
            if (Windows[i].Handle == handle)
            {
                return Windows[i];
            }
        }

        return null;
    }
}