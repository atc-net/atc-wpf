namespace Atc.Wpf.Hardware.Services;

public sealed class PrinterService : IPrinterService
{
    private readonly Func<IReadOnlyList<PrinterSnapshot>> enumerate;
    private readonly DispatcherTimer pollTimer;
    private Task? inFlightPoll;
    private bool started;
    private bool disposed;

    public PrinterService()
        : this(EnumeratePrinters)
    {
    }

    internal PrinterService(Func<IReadOnlyList<PrinterSnapshot>> enumerate)
    {
        this.enumerate = enumerate;
        Printers = new ObservableCollection<PrinterInfo>();
        pollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2),
        };
        pollTimer.Tick += OnPollTick;
    }

    public ObservableCollection<PrinterInfo> Printers { get; }

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

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Polling must not crash on transient print spooler errors.")]
    private async Task PollFromTimerAsync()
    {
        try
        {
            await RefreshAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"PrinterService poll failed: {ex.Message}");
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Print queue metadata access can throw.")]
    private static IReadOnlyList<PrinterSnapshot> EnumeratePrinters()
    {
        using var server = new System.Printing.LocalPrintServer();

        string? defaultName = null;
        try
        {
            defaultName = server.DefaultPrintQueue?.FullName;
        }
        catch (Exception)
        {
            // No default printer configured.
        }

        var queues = server.GetPrintQueues(new[]
        {
            System.Printing.EnumeratedPrintQueueTypes.Local,
            System.Printing.EnumeratedPrintQueueTypes.Connections,
        });

        var snapshots = new List<PrinterSnapshot>();

        foreach (var queue in queues)
        {
            try
            {
                var isDefault = !string.IsNullOrEmpty(defaultName) &&
                    string.Equals(queue.FullName, defaultName, StringComparison.OrdinalIgnoreCase);

                snapshots.Add(new PrinterSnapshot(
                    queue.Name,
                    queue.FullName,
                    IsLocal: !queue.IsShared || queue.HostingPrintServer.Name is null,
                    queue.IsShared,
                    isDefault,
                    queue.QueueStatus.ToString()));
            }
            catch (Exception)
            {
                // Skip queues that can't be inspected.
            }
            finally
            {
                queue.Dispose();
            }
        }

        return snapshots;
    }

    private async Task PollAsync()
    {
        // The spooler query can block for seconds when a network print server is offline.
        var snapshots = await Task.Run(enumerate).ConfigureAwait(true);
        if (disposed)
        {
            return;
        }

        var foundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var snapshot in snapshots)
        {
            foundIds.Add(snapshot.FullName);
            Upsert(snapshot);
        }

        for (var i = Printers.Count - 1; i >= 0; i--)
        {
            if (!foundIds.Contains(Printers[i].DeviceId))
            {
                Printers[i].State = DeviceState.Disconnected;
            }
        }
    }

    private void Upsert(PrinterSnapshot snapshot)
    {
        var existing = FindByDeviceId(snapshot.FullName);

        if (existing is not null)
        {
            if (existing.State is DeviceState.Disconnected)
            {
                existing.State = DeviceState.Available;
            }

            existing.IsDefault = snapshot.IsDefault;
            existing.QueueStatus = snapshot.QueueStatus;
            return;
        }

        var info = new PrinterInfo(
            name: snapshot.Name,
            fullName: snapshot.FullName,
            isLocal: snapshot.IsLocal,
            isShared: snapshot.IsShared,
            isDefault: snapshot.IsDefault,
            queueStatus: snapshot.QueueStatus)
        {
            State = DeviceState.Available,
        };

        Printers.Add(info);
    }

    private PrinterInfo? FindByDeviceId(string deviceId)
    {
        for (var i = 0; i < Printers.Count; i++)
        {
            if (string.Equals(Printers[i].DeviceId, deviceId, StringComparison.OrdinalIgnoreCase))
            {
                return Printers[i];
            }
        }

        return null;
    }
}