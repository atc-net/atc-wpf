namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates the top-level windows and polls for changes.
/// </summary>
public sealed class WindowService : IWindowService
{
    private readonly Func<bool, IReadOnlyList<WindowSnapshot>> enumerate;
    private readonly Func<int, string> resolveProcessName;
    private readonly DispatcherTimer pollTimer;
    private Task? inFlightPoll;
    private bool started;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowService"/> class.
    /// </summary>
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

    /// <inheritdoc />
    public ObservableCollection<TopLevelWindowInfo> Windows { get; }

    /// <inheritdoc />
    public TimeSpan PollingInterval
    {
        get => pollTimer.Interval;
        set => pollTimer.Interval = value;
    }

    /// <inheritdoc />
    public bool OnlyVisibleWithTitle { get; set; } = true;

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

    /// <summary>
    /// Enumerates on a background thread and applies the result on the calling (UI) thread.
    /// A refresh requested while a poll is in flight shares that poll.
    /// </summary>
    public Task RefreshAsync()
    {
        if (inFlightPoll is { IsCompleted: false })
        {
            return inFlightPoll;
        }

        inFlightPoll = PollAsync();
        return inFlightPoll;
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

    private void OnPollTick(
        object? sender,
        EventArgs e)
        => _ = PollFromTimerAsync();

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Polling must not crash on enumeration errors.")]
    private async Task PollFromTimerAsync()
    {
        try
        {
            await RefreshAsync().ConfigureAwait(true);
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

    private async Task PollAsync()
    {
        var onlyVisibleWithTitle = OnlyVisibleWithTitle;
        var snapshots = await Task.Run(() => enumerate(onlyVisibleWithTitle)).ConfigureAwait(true);
        if (disposed)
        {
            return;
        }

        var foundHandles = new HashSet<IntPtr>();
        var newcomers = new List<(WindowSnapshot Snapshot, TopLevelWindowInfo? Stale)>();

        foreach (var snapshot in snapshots)
        {
            foundHandles.Add(snapshot.Handle);

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
            }
            else
            {
                // New window, or the handle was reused by another window (the old one is gone).
                newcomers.Add((snapshot, existing));
            }
        }

        if (newcomers.Count > 0)
        {
            var processNames = await Task
                .Run(() => newcomers.Select(n => resolveProcessName(n.Snapshot.ProcessId)).ToList())
                .ConfigureAwait(true);
            if (disposed)
            {
                return;
            }

            for (var i = 0; i < newcomers.Count; i++)
            {
                AddNewcomer(newcomers[i].Snapshot, processNames[i], newcomers[i].Stale);
            }
        }

        MarkMissingAsDisconnected(foundHandles);
    }

    private void MarkMissingAsDisconnected(HashSet<IntPtr> found)
    {
        for (var i = Windows.Count - 1; i >= 0; i--)
        {
            if (!found.Contains(Windows[i].Handle))
            {
                Windows[i].State = DeviceState.Disconnected;
            }
        }
    }

    private void AddNewcomer(
        WindowSnapshot snapshot,
        string processName,
        TopLevelWindowInfo? stale)
    {
        var info = new TopLevelWindowInfo(
            handle: snapshot.Handle,
            title: snapshot.Title,
            className: snapshot.ClassName,
            processId: snapshot.ProcessId,
            processName: processName)
        {
            State = DeviceState.Available,
        };

        Windows.Add(info);

        if (stale is not null)
        {
            RemoveStale(stale);
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