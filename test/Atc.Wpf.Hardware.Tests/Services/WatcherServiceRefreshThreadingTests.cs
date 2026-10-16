namespace Atc.Wpf.Hardware.Tests.Services;

/// <summary>
/// The watcher-based services continue on a thread-pool thread after <c>FindAllAsync</c>, while the
/// watcher host raises its events on the UI thread. The device collection (and the ComboBox view bound
/// to it) must only ever change on the UI thread, whichever thread the refresh happens to run on.
/// </summary>
public sealed class WatcherServiceRefreshThreadingTests
{
    public static TheoryData<string> ServiceNames
        => new()
        {
            nameof(AudioDeviceService),
            nameof(UsbCameraService),
            nameof(BluetoothDeviceService),
            nameof(UsbDeviceService),
            nameof(SerialPortService),
        };

    [WpfTheory]
    [MemberData(nameof(ServiceNames))]
    public async Task RefreshAsync_FindAllCompletesOffTheUiThread_ChangesTheCollectionOnTheUiThread(
        string serviceName)
    {
        var uiDispatcher = Dispatcher.CurrentDispatcher;
        using var host = new FakeDeviceWatcherHost { DeferFindAll = true };
        using var subject = CreateSubject(serviceName, host);

        host.RaiseAdded(new DeviceSnapshot { Id = "stale", Name = "Stale", IsEnabled = true, PortName = "COM9" });
        host.SeedSnapshots.Add(new DeviceSnapshot { Id = "fresh-1", Name = "Fresh 1", IsEnabled = true, PortName = "COM1" });
        host.SeedSnapshots.Add(new DeviceSnapshot { Id = "fresh-2", Name = "Fresh 2", IsEnabled = true, PortName = "COM2" });

        // Mirrors the picker's ComboBox binding: a bound view rejects changes made from other threads.
        _ = new ListCollectionView(subject.Items);

        var offUiThreadChanges = 0;
        subject.Collection.CollectionChanged += (_, _) => CountIfOffUiThread();
        var stale = (INotifyPropertyChanged)subject.Items[0]!;
        stale.PropertyChanged += (_, _) => CountIfOffUiThread();

        var refresh = subject.RefreshAsync();
        await Task.Run(host.CompleteFindAll);
        await refresh;

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

    private static Subject CreateSubject(
        string serviceName,
        FakeDeviceWatcherHost host)
    {
        switch (serviceName)
        {
            case nameof(AudioDeviceService):
            {
                var service = new AudioDeviceService(AudioDeviceKind.Input, host);
                return new Subject(service, service.Devices, service.RefreshAsync);
            }

            case nameof(UsbCameraService):
            {
                var service = new UsbCameraService(host);
                return new Subject(service, service.Cameras, service.RefreshAsync);
            }

            case nameof(BluetoothDeviceService):
            {
                var service = new BluetoothDeviceService(host);
                return new Subject(service, service.Devices, service.RefreshAsync);
            }

            case nameof(UsbDeviceService):
            {
                var service = new UsbDeviceService(_ => host);
                return new Subject(service, service.Devices, service.RefreshAsync);
            }

            case nameof(SerialPortService):
            {
                var service = new SerialPortService(host);
                return new Subject(service, service.Ports, service.RefreshAsync);
            }

            default:
                throw new ArgumentOutOfRangeException(nameof(serviceName), serviceName, message: null);
        }
    }

    private sealed class Subject : IDisposable
    {
        private readonly IDisposable service;
        private readonly Func<Task> refreshAsync;

        public Subject(
            IDisposable service,
            IList items,
            Func<Task> refreshAsync)
        {
            this.service = service;
            this.refreshAsync = refreshAsync;
            Items = items;
            Collection = (INotifyCollectionChanged)items;
        }

        public IList Items { get; }

        public INotifyCollectionChanged Collection { get; }

        public Task RefreshAsync()
            => refreshAsync();

        public void Dispose()
            => service.Dispose();
    }
}