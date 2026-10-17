namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates the top-level windows and polls for changes; the collection is only changed on the UI thread.
/// </summary>
public sealed class WindowService : IWindowService
{
    private readonly Func<bool, IReadOnlyList<WindowSnapshot>> enumerate;
    private readonly Func<int, string> resolveProcessName;
    private readonly PollingServiceHost host;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowService"/> class.
    /// </summary>
    public WindowService()
        : this(EnumerateWindows, ResolveProcessName)
    {
    }

    internal WindowService(
        Func<bool, IReadOnlyList<WindowSnapshot>> enumerate,
        Func<int, string> resolveProcessName,
        Dispatcher? uiDispatcher = null)
    {
        this.enumerate = enumerate;
        this.resolveProcessName = resolveProcessName;
        Windows = new ObservableCollection<TopLevelWindowInfo>();
        host = new PollingServiceHost(nameof(WindowService), PollAsync, uiDispatcher);
    }

    /// <inheritdoc />
    public ObservableCollection<TopLevelWindowInfo> Windows { get; }

    /// <inheritdoc />
    public TimeSpan PollingInterval
    {
        get => host.Interval;
        set => host.Interval = value;
    }

    /// <inheritdoc />
    public bool OnlyVisibleWithTitle { get; set; } = true;

    /// <inheritdoc />
    public void StartWatching()
        => host.Start();

    /// <inheritdoc />
    public void StopWatching()
        => host.Stop();

    /// <summary>
    /// Enumerates on a background thread and applies the result on the UI thread.
    /// A refresh requested while a poll is in flight shares that poll.
    /// </summary>
    /// <remarks>Safe to call from any thread: the collection is only changed on the UI thread.</remarks>
    /// <returns>A task that completes when the enumeration has been applied.</returns>
    public Task RefreshAsync()
        => host.RefreshAsync();

    /// <inheritdoc />
    public void Dispose()
        => host.Dispose();

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
        var snapshots = enumerate(OnlyVisibleWithTitle);
        if (host.IsDisposed)
        {
            return;
        }

        var foundHandles = snapshots
            .Select(snapshot => snapshot.Handle)
            .ToHashSet();

        // The collection is bound to the picker, so it is only read and changed on the UI thread.
        var newcomers = await host
            .InvokeOnUiAsync(() => host.IsDisposed ? [] : UpdateKnown(snapshots))
            .ConfigureAwait(false);
        if (host.IsDisposed)
        {
            return;
        }

        var processNames = newcomers.Count == 0
            ? []
            : await Task
                .Run(() => newcomers.Select(n => resolveProcessName(n.Snapshot.ProcessId)).ToList())
                .ConfigureAwait(false);
        if (host.IsDisposed)
        {
            return;
        }

        await host
            .InvokeOnUiAsync(() =>
            {
                if (host.IsDisposed)
                {
                    return;
                }

                for (var i = 0; i < newcomers.Count; i++)
                {
                    AddNewcomer(newcomers[i].Snapshot, processNames[i], newcomers[i].Stale);
                }

                MarkMissingAsDisconnected(foundHandles);
            })
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates the entries that are still the same window and returns the snapshots that need a new entry.
    /// </summary>
    private List<(WindowSnapshot Snapshot, TopLevelWindowInfo? Stale)> UpdateKnown(
        IReadOnlyList<WindowSnapshot> snapshots)
    {
        var newcomers = new List<(WindowSnapshot Snapshot, TopLevelWindowInfo? Stale)>();

        foreach (var snapshot in snapshots)
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
            }
            else
            {
                // New window, or the handle was reused by another window (the old one is gone).
                newcomers.Add((snapshot, existing));
            }
        }

        return newcomers;
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