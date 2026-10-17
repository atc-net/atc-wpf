namespace Atc.Wpf.Hardware.Tests.Services;

/// <summary>
/// The polling services enumerate on the thread pool and are driven by a thread-pool timer. The collection
/// (and the ComboBox view bound to it) must only ever change on the UI thread, whichever thread the refresh
/// or the timer runs on, and the timer must not depend on the thread that constructed the service.
/// </summary>
public sealed class PollingServiceRefreshThreadingTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    public static TheoryData<string> ServiceNames
        => new()
        {
            nameof(DriveService),
            nameof(NetworkAdapterService),
            nameof(DisplayService),
            nameof(PrinterService),
            nameof(ProcessService),
            nameof(WindowService),
        };

    [WpfTheory]
    [MemberData(nameof(ServiceNames))]
    public async Task RefreshAsync_CalledOffTheUiThread_ChangesTheCollectionOnTheUiThread(
        string serviceName)
    {
        var uiDispatcher = Dispatcher.CurrentDispatcher;
        var source = new SnapshotSource();
        using var subject = CreateSubject(serviceName, source, uiDispatcher: null);

        await subject.RefreshAsync();
        var stale = (INotifyPropertyChanged)Assert.Single(subject.Items.Cast<object>());

        // Mirrors the picker's ComboBox binding: a bound view rejects changes made from other threads.
        _ = new ListCollectionView(subject.Items);

        var offUiThreadChanges = 0;
        subject.Collection.CollectionChanged += (_, _) => CountIfOffUiThread();
        stale.PropertyChanged += (_, _) => CountIfOffUiThread();

        source.Fresh = true;
        await Task.Run(subject.RefreshAsync);

        Assert.Equal(3, subject.Items.Count);
        Assert.Equal(DeviceState.Disconnected, ((IDeviceInfo)stale).State);
        Assert.Equal(0, offUiThreadChanges);

        void CountIfOffUiThread()
        {
            if (!uiDispatcher.CheckAccess())
            {
                Interlocked.Increment(ref offUiThreadChanges);
            }
        }
    }

    [WpfTheory]
    [MemberData(nameof(ServiceNames))]
    public async Task StartWatching_ServiceBuiltOffTheUiThread_PollsAndChangesTheCollectionOnTheUiThread(
        string serviceName)
    {
        var uiDispatcher = Dispatcher.CurrentDispatcher;
        var source = new SnapshotSource { Fresh = true };

        // A timer bound to the constructing thread's dispatcher would never tick here: nothing pumps the
        // thread-pool thread the service is built and started on.
        using var subject = await Task.Run(() =>
        {
            var created = CreateSubject(serviceName, source, uiDispatcher);
            created.SetPollingInterval(TimeSpan.FromMilliseconds(20));
            created.StartWatching();
            return created;
        });

        var offUiThreadChanges = 0;
        subject.Collection.CollectionChanged += (_, _) =>
        {
            if (!uiDispatcher.CheckAccess())
            {
                Interlocked.Increment(ref offUiThreadChanges);
            }
        };

        var deadline = DateTime.UtcNow + Timeout;
        while (subject.Items.Count < 2 && DateTime.UtcNow < deadline)
        {
            await Task.Delay(20);
        }

        subject.StopWatching();

        Assert.Equal(2, subject.Items.Count);
        Assert.Equal(0, offUiThreadChanges);
    }

    private static Subject CreateSubject(
        string serviceName,
        SnapshotSource source,
        Dispatcher? uiDispatcher)
    {
        switch (serviceName)
        {
            case nameof(DriveService):
            {
                var service = new DriveService(
                    listDrives: () => source.Fresh
                        ? [(@"E:\", DriveType.Removable), (@"F:\", DriveType.Removable)]
                        : [(@"X:\", DriveType.Removable)],
                    readVolume: (name, driveType) => new DriveSnapshot(name, name, driveType, IsReady: true, TotalSize: 1_000, AvailableFreeSpace: 500),
                    uiDispatcher);
                return new Subject(service, service.Drives, service.RefreshAsync, service.StartWatching, service.StopWatching, v => service.PollingInterval = v);
            }

            case nameof(NetworkAdapterService):
            {
                var service = new NetworkAdapterService(
                    enumerate: () => source.Fresh
                        ? [Adapter("{FRESH-1}"), Adapter("{FRESH-2}")]
                        : [Adapter("{STALE}")],
                    uiDispatcher);
                return new Subject(service, service.Adapters, service.RefreshAsync, service.StartWatching, service.StopWatching, v => service.PollingInterval = v);
            }

            case nameof(DisplayService):
            {
                var service = new DisplayService(
                    enumerate: () => source.Fresh
                        ? [Display(@"\\.\DISPLAY1"), Display(@"\\.\DISPLAY2")]
                        : [Display(@"\\.\DISPLAY9")],
                    uiDispatcher);
                return new Subject(service, service.Displays, service.RefreshAsync, service.StartWatching, service.StopWatching, v => service.PollingInterval = v);
            }

            case nameof(PrinterService):
            {
                var service = new PrinterService(
                    enumerate: () => source.Fresh
                        ? [Printer("Fresh 1"), Printer("Fresh 2")]
                        : [Printer("Stale")],
                    uiDispatcher);
                return new Subject(service, service.Printers, service.RefreshAsync, service.StartWatching, service.StopWatching, v => service.PollingInterval = v);
            }

            case nameof(ProcessService):
            {
                var service = new ProcessService(
                    enumerate: () => source.Fresh
                        ? [Process(101), Process(102)]
                        : [Process(999)],
                    resolveModulePath: _ => null,
                    uiDispatcher);
                return new Subject(service, service.Processes, service.RefreshAsync, service.StartWatching, service.StopWatching, v => service.PollingInterval = v);
            }

            case nameof(WindowService):
            {
                var service = new WindowService(
                    enumerate: _ => source.Fresh
                        ? [Window(0x101), Window(0x102)]
                        : [Window(0x999)],
                    resolveProcessName: _ => "app",
                    uiDispatcher);
                return new Subject(service, service.Windows, service.RefreshAsync, service.StartWatching, service.StopWatching, v => service.PollingInterval = v);
            }

            default:
                throw new ArgumentOutOfRangeException(nameof(serviceName), serviceName, message: null);
        }
    }

    private static NetworkAdapterSnapshot Adapter(string id)
        => new(id, id, "Ethernet", NetworkInterfaceType.Ethernet, "001122334455", 1_000_000_000, OperationalStatus.Up);

    private static DisplaySnapshot Display(string deviceName)
        => new(new IntPtr(deviceName.GetHashCode(StringComparison.Ordinal)), deviceName, new Rect(0, 0, 1920, 1080), new Rect(0, 0, 1920, 1040), IsPrimary: false);

    private static PrinterSnapshot Printer(string name)
        => new(name, name, IsLocal: true, IsShared: false, IsDefault: false, QueueStatus: "None");

    private static ProcessSnapshot Process(int processId)
        => new(processId, "app", "Title", HasMainWindow: true);

    private static WindowSnapshot Window(int handle)
        => new(new IntPtr(handle), "Title", "Class", 42);

    private sealed class SnapshotSource
    {
        public bool Fresh { get; set; }
    }

    private sealed class Subject : IDisposable
    {
        private readonly IDisposable service;
        private readonly Func<Task> refreshAsync;
        private readonly Action startWatching;
        private readonly Action stopWatching;
        private readonly Action<TimeSpan> setPollingInterval;

        public Subject(
            IDisposable service,
            IList items,
            Func<Task> refreshAsync,
            Action startWatching,
            Action stopWatching,
            Action<TimeSpan> setPollingInterval)
        {
            this.service = service;
            this.refreshAsync = refreshAsync;
            this.startWatching = startWatching;
            this.stopWatching = stopWatching;
            this.setPollingInterval = setPollingInterval;
            Items = items;
            Collection = (INotifyCollectionChanged)items;
        }

        public IList Items { get; }

        public INotifyCollectionChanged Collection { get; }

        public void SetPollingInterval(TimeSpan interval)
            => setPollingInterval(interval);

        public Task RefreshAsync()
            => refreshAsync();

        public void StartWatching()
            => startWatching();

        public void StopWatching()
            => stopWatching();

        public void Dispose()
            => service.Dispose();
    }
}