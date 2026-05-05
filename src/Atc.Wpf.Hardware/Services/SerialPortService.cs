namespace Atc.Wpf.Hardware.Services;

public sealed class SerialPortService : ISerialPortService
{
    private const string PortNamePropertyKey = "System.DeviceInterface.Serial.PortName";

    private static readonly TimeSpan JustConnectedDuration = TimeSpan.FromSeconds(3);

    private static readonly string[] RequestedProperties =
    {
        PortNamePropertyKey,
    };

    private readonly DeviceWatcherHost watcher;
    private readonly System.Threading.Lock syncRoot = new();
    private bool started;
    private bool initialEnumerationCompleted;
    private bool disposed;

    public SerialPortService()
    {
        Ports = new ObservableCollection<SerialPortInfo>();

        watcher = new DeviceWatcherHost(
            SerialDevice.GetDeviceSelector(),
            RequestedProperties);

        watcher.Added += OnAdded;
        watcher.Removed += OnRemoved;
        watcher.Updated += OnUpdated;
        watcher.EnumerationCompleted += OnEnumerationCompleted;
    }

    public ObservableCollection<SerialPortInfo> Ports { get; }

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
        var found = await DeviceInformation
            .FindAllAsync(SerialDevice.GetDeviceSelector(), RequestedProperties);

        var foundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var info in found)
        {
            foundIds.Add(info.Id);
            UpsertFromInfo(info, isInitialEnumeration: true);
        }

        for (var i = Ports.Count - 1; i >= 0; i--)
        {
            if (!foundIds.Contains(Ports[i].DeviceId))
            {
                Ports[i].State = DeviceState.Disconnected;
            }
        }
    }

    public async Task<bool> ProbeInUseAsync(SerialPortInfo port)
    {
        ArgumentNullException.ThrowIfNull(port);

        try
        {
            using var device = await SerialDevice.FromIdAsync(port.DeviceId);
            return device is null;
        }
        catch (UnauthorizedAccessException)
        {
            return true;
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
        // Property updates (rare for serial). No-op today.
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

        var portName = info.Properties.TryGetValue(PortNamePropertyKey, out var raw)
            && raw is string s
                ? s
                : info.Name;

        var (vid, pid) = UsbIdParser.Parse(info.Id);

        var newInfo = new SerialPortInfo(
            deviceId: info.Id,
            portName: portName,
            friendlyName: info.Name,
            vendorId: vid,
            productId: pid)
        {
            State = isInitialEnumeration
                ? DeviceState.Available
                : DeviceState.JustConnected,
        };

        lock (syncRoot)
        {
            Ports.Add(newInfo);
        }

        if (newInfo.State is DeviceState.JustConnected)
        {
            JustConnectedTimer.TransitionToAvailableAfter(
                state => newInfo.State = state,
                JustConnectedDuration);
        }
    }

    private SerialPortInfo? FindByDeviceId(string deviceId)
    {
        for (var i = 0; i < Ports.Count; i++)
        {
            if (string.Equals(Ports[i].DeviceId, deviceId, StringComparison.OrdinalIgnoreCase))
            {
                return Ports[i];
            }
        }

        return null;
    }
}