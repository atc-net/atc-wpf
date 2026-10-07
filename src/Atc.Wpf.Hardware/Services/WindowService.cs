namespace Atc.Wpf.Hardware.Services;

public sealed class WindowService : IWindowService
{
    private readonly Func<bool, IReadOnlyList<WindowSnapshot>> enumerate;
    private readonly Func<int, string> resolveProcessName;
    private readonly DispatcherTimer pollTimer;
    private bool started;
    private bool disposed;

    public WindowService()
        : this(EnumerateWindows, ResolveProcessName)
    {
    }

    internal WindowService(
        Func<bool, IReadOnlyList<WindowSnapshot>> enumerate,
        Func<int, string> resolveProcessName)
    {
        this.enumerate = enumerate;
        this.resolveProcessName = resolveProcessName;
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

    private static IReadOnlyList<WindowSnapshot> EnumerateWindows(
        bool onlyVisibleWithTitle)
    {
        var snapshots = new List<WindowSnapshot>();

        NativeWindowMethods.EnumWindows(
            (hWnd, _) =>
            {
                if (onlyVisibleWithTitle)
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

                snapshots.Add(new WindowSnapshot(
                    hWnd,
                    NativeWindowMethods.ReadWindowTitle(hWnd),
                    NativeWindowMethods.ReadClassName(hWnd),
                    ReadProcessId(hWnd)));
                return true;
            },
            lParam: IntPtr.Zero);

        return snapshots;
    }

    private static int ReadProcessId(IntPtr hWnd)
    {
        _ = NativeWindowMethods.GetWindowThreadProcessId(hWnd, out var pid);
        return (int)pid;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Process metadata access can throw.")]
    private static string ResolveProcessName(int processId)
    {
        try
        {
            using var p = System.Diagnostics.Process.GetProcessById(processId);
            return p.ProcessName;
        }
        catch (Exception)
        {
            // Process may have exited between EnumWindows and lookup.
            return "(unknown)";
        }
    }

    private void EnumerateAndSync()
    {
        var foundHandles = new HashSet<IntPtr>();

        foreach (var snapshot in enumerate(OnlyVisibleWithTitle))
        {
            foundHandles.Add(snapshot.Handle);
            Upsert(snapshot);
        }

        for (var i = Windows.Count - 1; i >= 0; i--)
        {
            if (!foundHandles.Contains(Windows[i].Handle))
            {
                Windows[i].State = DeviceState.Disconnected;
            }
        }
    }

    private void Upsert(WindowSnapshot snapshot)
    {
        var existing = FindByHandle(snapshot.Handle);

        if (existing is not null &&
            existing.ProcessId == snapshot.ProcessId &&
            string.Equals(existing.ClassName, snapshot.ClassName, StringComparison.Ordinal))
        {
            if (existing.State is DeviceState.Disconnected)
            {
                existing.State = DeviceState.Available;
            }

            existing.Title = snapshot.Title;
            return;
        }

        var info = new TopLevelWindowInfo(
            handle: snapshot.Handle,
            title: snapshot.Title,
            className: snapshot.ClassName,
            processId: snapshot.ProcessId,
            processName: resolveProcessName(snapshot.ProcessId))
        {
            State = DeviceState.Available,
        };

        Windows.Add(info);

        // The window handle was reused by another window: the old window is gone.
        if (existing is not null)
        {
            RemoveStale(existing);
        }
    }

    /// <summary>
    /// Retires an entry whose id now belongs to something else. Called after the fresh entry has been added,
    /// so a picker that still has the stale entry selected sees it disconnect and leave - and does not
    /// auto-rebind to the newcomer just because it reuses the same id.
    /// </summary>
    private void RemoveStale(TopLevelWindowInfo stale)
    {
        stale.State = DeviceState.Disconnected;
        Windows.Remove(stale);
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