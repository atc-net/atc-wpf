namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates the running processes and polls for changes.
/// </summary>
public sealed class ProcessService : IProcessService
{
    private readonly Func<IReadOnlyList<ProcessSnapshot>> enumerate;
    private readonly Func<int, string?> resolveModulePath;
    private readonly DispatcherTimer pollTimer;
    private Task? inFlightPoll;
    private bool started;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessService"/> class.
    /// </summary>
    public ProcessService()
        : this(EnumerateProcesses, ResolveModulePath)
    {
    }

    internal ProcessService(
        Func<IReadOnlyList<ProcessSnapshot>> enumerate,
        Func<int, string?> resolveModulePath)
    {
        this.enumerate = enumerate;
        this.resolveModulePath = resolveModulePath;
        Processes = new ObservableCollection<RunningProcessInfo>();
        pollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2),
        };
        pollTimer.Tick += OnPollTick;
    }

    /// <inheritdoc />
    public ObservableCollection<RunningProcessInfo> Processes { get; }

    /// <inheritdoc />
    public TimeSpan PollingInterval
    {
        get => pollTimer.Interval;
        set => pollTimer.Interval = value;
    }

    /// <inheritdoc />
    public bool OnlyWithMainWindow { get; set; } = true;

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

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Polling must not crash on transient process enumeration errors.")]
    private async Task PollFromTimerAsync()
    {
        try
        {
            await RefreshAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ProcessService poll failed: {ex.Message}");
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Process metadata access can throw for protected processes.")]
    private static IReadOnlyList<ProcessSnapshot> EnumerateProcesses()
    {
        var snapshots = new List<ProcessSnapshot>();

        foreach (var p in System.Diagnostics.Process.GetProcesses())
        {
            try
            {
                var hasWindow = p.MainWindowHandle != IntPtr.Zero;
                snapshots.Add(new ProcessSnapshot(
                    p.Id,
                    p.ProcessName,
                    hasWindow ? p.MainWindowTitle : string.Empty,
                    hasWindow));
            }
            catch (Exception)
            {
                // Some processes (system / protected) deny metadata access.
            }
            finally
            {
                p.Dispose();
            }
        }

        return snapshots;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Module access can throw for protected processes.")]
    private static string? ResolveModulePath(int processId)
    {
        try
        {
            using var process = System.Diagnostics.Process.GetProcessById(processId);
            return process.MainModule?.FileName;
        }
        catch (Exception)
        {
            // Access denied or already exited — leave path null.
            return null;
        }
    }

    private async Task PollAsync()
    {
        var snapshots = await Task.Run(enumerate).ConfigureAwait(true);
        if (disposed)
        {
            return;
        }

        var foundIds = new HashSet<int>();
        var newcomers = new List<(ProcessSnapshot Snapshot, RunningProcessInfo? Stale)>();

        foreach (var snapshot in snapshots)
        {
            if (OnlyWithMainWindow && !snapshot.HasMainWindow)
            {
                continue;
            }

            foundIds.Add(snapshot.ProcessId);

            var existing = FindByProcessId(snapshot.ProcessId);
            if (existing is not null &&
                string.Equals(existing.ProcessName, snapshot.ProcessName, StringComparison.OrdinalIgnoreCase))
            {
                if (existing.State is DeviceState.Disconnected)
                {
                    existing.State = DeviceState.Available;
                }

                existing.MainWindowTitle = snapshot.MainWindowTitle;
            }
            else
            {
                // New process, or the PID was reused by another process (the old one is gone).
                newcomers.Add((snapshot, existing));
            }
        }

        if (newcomers.Count > 0)
        {
            // MainModule access is slow (and can throw for protected processes): resolve off the UI thread.
            var modulePaths = await Task
                .Run(() => newcomers.Select(n => resolveModulePath(n.Snapshot.ProcessId)).ToList())
                .ConfigureAwait(true);
            if (disposed)
            {
                return;
            }

            for (var i = 0; i < newcomers.Count; i++)
            {
                AddNewcomer(newcomers[i].Snapshot, modulePaths[i], newcomers[i].Stale);
            }
        }

        MarkMissingAsDisconnected(foundIds);
    }

    private void MarkMissingAsDisconnected(HashSet<int> found)
    {
        for (var i = Processes.Count - 1; i >= 0; i--)
        {
            if (!found.Contains(Processes[i].ProcessId))
            {
                Processes[i].State = DeviceState.Disconnected;
            }
        }
    }

    private void AddNewcomer(
        ProcessSnapshot snapshot,
        string? modulePath,
        RunningProcessInfo? stale)
    {
        var info = new RunningProcessInfo(
            processId: snapshot.ProcessId,
            processName: snapshot.ProcessName,
            mainWindowTitle: snapshot.MainWindowTitle,
            mainModulePath: modulePath)
        {
            State = DeviceState.Available,
        };

        Processes.Add(info);

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
    private void RemoveStale(RunningProcessInfo stale)
    {
        stale.State = DeviceState.Disconnected;
        Processes.Remove(stale);
    }

    private RunningProcessInfo? FindByProcessId(int processId)
    {
        for (var i = 0; i < Processes.Count; i++)
        {
            if (Processes[i].ProcessId == processId)
            {
                return Processes[i];
            }
        }

        return null;
    }
}