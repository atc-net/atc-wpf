namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates the printers (print queues) and polls for changes; the collection is only changed on the UI thread.
/// </summary>
public sealed class PrinterService : IPrinterService
{
    private readonly Func<IReadOnlyList<PrinterSnapshot>> enumerate;
    private readonly PollingServiceHost host;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrinterService"/> class.
    /// </summary>
    public PrinterService()
        : this(EnumeratePrinters)
    {
    }

    internal PrinterService(
        Func<IReadOnlyList<PrinterSnapshot>> enumerate,
        Dispatcher? uiDispatcher = null)
    {
        this.enumerate = enumerate;
        Printers = new ObservableCollection<PrinterInfo>();
        host = new PollingServiceHost(nameof(PrinterService), PollAsync, uiDispatcher);
    }

    /// <inheritdoc />
    public ObservableCollection<PrinterInfo> Printers { get; }

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
        // The spooler query can block for seconds when a network print server is offline,
        // so it runs on the thread pool; the bound collection is only changed on the UI thread.
        var snapshots = enumerate();
        if (host.IsDisposed)
        {
            return;
        }

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

    private void SyncWith(IReadOnlyList<PrinterSnapshot> snapshots)
    {
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