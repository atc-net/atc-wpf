namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates the running processes and polls for changes; the collection is only changed on the UI thread.
/// </summary>
public sealed class ProcessService : IProcessService
{
    private readonly Func<IReadOnlyList<ProcessSnapshot>> enumerate;
    private readonly Func<int, string?> resolveModulePath;
    private readonly PollingServiceHost host;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessService"/> class.
    /// </summary>
    public ProcessService()
        : this(EnumerateProcesses, ResolveModulePath)
    {
    }

    internal ProcessService(
        Func<IReadOnlyList<ProcessSnapshot>> enumerate,
        Func<int, string?> resolveModulePath,
        Dispatcher? uiDispatcher = null)
    {
        this.enumerate = enumerate;
        this.resolveModulePath = resolveModulePath;
        Processes = new ObservableCollection<RunningProcessInfo>();
        host = new PollingServiceHost(nameof(ProcessService), PollAsync, uiDispatcher);
    }

    /// <inheritdoc />
    public ObservableCollection<RunningProcessInfo> Processes { get; }

    /// <inheritdoc />
    public TimeSpan PollingInterval
    {
        get => host.Interval;
        set => host.Interval = value;
    }

    /// <inheritdoc />
    public bool OnlyWithMainWindow { get; set; } = true;

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
        var snapshots = Filter(enumerate(), OnlyWithMainWindow);
        if (host.IsDisposed)
        {
            return;
        }

        var foundIds = snapshots
            .Select(snapshot => snapshot.ProcessId)
            .ToHashSet();

        // The collection is bound to the picker, so it is only read and changed on the UI thread.
        var newcomers = await host
            .InvokeOnUiAsync(() => host.IsDisposed ? [] : UpdateKnown(snapshots))
            .ConfigureAwait(false);
        if (host.IsDisposed)
        {
            return;
        }

        // MainModule access is slow (and can throw for protected processes): resolve off the UI thread.
        var modulePaths = newcomers.Count == 0
            ? []
            : await Task
                .Run(() => newcomers.Select(n => resolveModulePath(n.Snapshot.ProcessId)).ToList())
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
                    AddNewcomer(newcomers[i].Snapshot, modulePaths[i], newcomers[i].Stale);
                }

                MarkMissingAsDisconnected(foundIds);
            })
            .ConfigureAwait(false);
    }

    private static List<ProcessSnapshot> Filter(
        IReadOnlyList<ProcessSnapshot> snapshots,
        bool onlyWithMainWindow)
        => snapshots
            .Where(snapshot => !onlyWithMainWindow || snapshot.HasMainWindow)
            .ToList();

    /// <summary>
    /// Updates the entries that are still the same process and returns the snapshots that need a new entry.
    /// </summary>
    private List<(ProcessSnapshot Snapshot, RunningProcessInfo? Stale)> UpdateKnown(
        IReadOnlyList<ProcessSnapshot> snapshots)
    {
        var newcomers = new List<(ProcessSnapshot Snapshot, RunningProcessInfo? Stale)>();

        foreach (var snapshot in snapshots)
        {
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

        return newcomers;
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