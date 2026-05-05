namespace Atc.Wpf.Hardware.Services;

public sealed class UsbDeviceService : IUsbDeviceService
{
    private const string UsbDeviceInterfaceAqs =
        "System.Devices.InterfaceClassGuid:=\"{a5dcbf10-6530-11d2-901f-00c04fb951ed}\"";

    private static readonly TimeSpan JustConnectedDuration = TimeSpan.FromSeconds(3);

    private readonly DeviceWatcherHost watcher;
    private bool started;
    private bool initialEnumerationCompleted;
    private bool disposed;

    public UsbDeviceService()
    {
        Devices = new ObservableCollection<UsbDeviceInfo>();

        watcher = new DeviceWatcherHost(UsbDeviceInterfaceAqs);

        watcher.Added += OnAdded;
        watcher.Removed += OnRemoved;
        watcher.Updated += OnUpdated;
        watcher.EnumerationCompleted += OnEnumerationCompleted;
    }

    public ObservableCollection<UsbDeviceInfo> Devices { get; }

    public UsbDeviceClassFilter ClassFilter { get; set; } = UsbDeviceClassFilter.None;

    public void StartWatching()
    {
        if (started)
        {
            return;
        }

        started = true;
        watcher.StartWatching();
    }

    public void StopWatching()
    {
        if (!started)
        {
            return;
        }

        started = false;
        watcher.StopWatching();
    }

    public async Task RefreshAsync()
    {
        var found = await DeviceInformation.FindAllAsync(UsbDeviceInterfaceAqs);

        var foundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var info in found)
        {
            foundIds.Add(info.Id);
            UpsertFromInfo(info, isInitialEnumeration: true);
        }

        for (var i = Devices.Count - 1; i >= 0; i--)
        {
            if (!foundIds.Contains(Devices[i].DeviceId))
            {
                Devices[i].State = DeviceState.Disconnected;
            }
        }
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        watcher.Added -= OnAdded;
        watcher.Removed -= OnRemoved;
        watcher.Updated -= OnUpdated;
        watcher.EnumerationCompleted -= OnEnumerationCompleted;
        watcher.Dispose();
    }

    private void OnAdded(
        object? sender,
        DeviceArrivedEventArgs e)
        => UpsertFromInfo(e.Device, isInitialEnumeration: !initialEnumerationCompleted);

    private void OnRemoved(
        object? sender,
        DeviceRemovedEventArgs e)
    {
        var existing = FindByDeviceId(e.Update.Id);
        if (existing is not null)
        {
            existing.State = DeviceState.Disconnected;
        }
    }

    private static void OnUpdated(
        object? sender,
        DeviceUpdatedEventArgs e)
    {
        // No-op for USB. Property updates are rare.
    }

    private void OnEnumerationCompleted(
        object? sender,
        EventArgs e)
    {
        initialEnumerationCompleted = true;
    }

    private void UpsertFromInfo(
        DeviceInformation info,
        bool isInitialEnumeration)
    {
        var existing = FindByDeviceId(info.Id);

        if (existing is not null)
        {
            if (existing.State is DeviceState.Disconnected)
            {
                existing.State = DeviceState.Available;
            }

            return;
        }

        var (vid, pid) = UsbIdParser.Parse(info.Id);

        var newInfo = new UsbDeviceInfo(
            deviceId: info.Id,
            friendlyName: info.Name,
            vendorId: vid,
            productId: pid,
            pnpClass: null,
            interfaceEnabled: info.IsEnabled)
        {
            State = !info.IsEnabled
                ? DeviceState.InUse
                : isInitialEnumeration
                    ? DeviceState.Available
                    : DeviceState.JustConnected,
        };

        Devices.Add(newInfo);

        if (newInfo.State is DeviceState.JustConnected)
        {
            JustConnectedTimer.TransitionToAvailableAfter(
                state => newInfo.State = state,
                JustConnectedDuration);
        }
    }

    private UsbDeviceInfo? FindByDeviceId(string deviceId)
    {
        for (var i = 0; i < Devices.Count; i++)
        {
            if (string.Equals(Devices[i].DeviceId, deviceId, StringComparison.OrdinalIgnoreCase))
            {
                return Devices[i];
            }
        }

        return null;
    }
}