namespace Atc.Wpf.Hardware.Tests.Pickers.Internal;

public sealed class DevicePickerControllerTests
{
    private readonly ObservableCollection<RunningProcessInfo> items = [];
    private readonly FakeHost host = new();
    private readonly List<string> serviceCalls = [];

    // The controller subscribes weakly (as in production, where the picker holds it). Keep the controllers
    // under test reachable, otherwise a Release-mode GC can collect them mid-test.
    private readonly List<DevicePickerController<RunningProcessInfo>> keepAlive = [];
    private Func<Task> refresh = () => Task.CompletedTask;

    [Fact]
    public void ItemStateChange_RaisesDeviceStateChangedOncePerChange()
    {
        var item = Item(1, DeviceState.Available);
        items.Add(item);
        CreateSut();

        item.State = DeviceState.InUse;
        item.State = DeviceState.InUse;

        Assert.Equal(["1:Available->InUse"], host.StateChanges);
    }

    [Fact]
    public void RemovedItem_NoLongerRaisesDeviceStateChanged()
    {
        var item = Item(1, DeviceState.Available);
        CreateSut();
        items.Add(item);

        items.Remove(item);
        item.State = DeviceState.InUse;

        Assert.Empty(host.StateChanges);
    }

    [Fact]
    public void SelectedItemDisconnects_RaisesDeviceLostAndShowsTheStateMessage()
    {
        var item = Item(1, DeviceState.Available);
        items.Add(item);
        var sut = CreateSut();
        Select(sut, item);

        item.State = DeviceState.Disconnected;
        items.Add(Item(2, DeviceState.Available));

        Assert.Equal(["1"], host.Lost);
        Assert.Same(item, host.Value);
        Assert.Equal(Miscellaneous.DeviceDisconnected, host.SelectedStateMessage);
    }

    [Fact]
    public void SelectedItemDisconnects_WithClearValueOnDisconnect_ClearsTheValue()
    {
        host.ClearValueOnDisconnect = true;
        var item = Item(1, DeviceState.Available);
        items.Add(item);
        var sut = CreateSut();
        Select(sut, item);

        item.State = DeviceState.Disconnected;
        items.Remove(item);

        Assert.Equal(["1"], host.Lost);
        Assert.Null(host.Value);
    }

    [Fact]
    public void LostDeviceComesBack_WithAutoRebind_IsSelectedAgainAndRaisesReconnected()
    {
        host.ClearValueOnDisconnect = true;
        var item = Item(1, DeviceState.Available);
        items.Add(item);
        var sut = CreateSut();
        Select(sut, item);
        item.State = DeviceState.Disconnected;
        items.Remove(item);

        var returned = Item(1, DeviceState.Available);
        items.Add(returned);

        Assert.Same(returned, host.Value);
        Assert.Equal(["1"], host.Reconnected);
    }

    [Fact]
    public void LostDeviceComesBack_WithoutAutoRebind_StaysUnselected()
    {
        host.ClearValueOnDisconnect = true;
        host.AutoRebindOnReconnect = false;
        var item = Item(1, DeviceState.Available);
        items.Add(item);
        var sut = CreateSut();
        Select(sut, item);
        item.State = DeviceState.Disconnected;
        items.Remove(item);

        items.Add(Item(1, DeviceState.Available));

        Assert.Null(host.Value);
        Assert.Empty(host.Reconnected);
    }

    [Fact]
    public void AutoSelectFirstAvailable_SelectsTheFirstAvailableAddedItem()
    {
        host.AutoSelectFirstAvailable = true;
        CreateSut();

        items.Add(Item(1, DeviceState.Disconnected));
        var available = Item(2, DeviceState.JustConnected);
        items.Add(available);
        items.Add(Item(3, DeviceState.Available));

        Assert.Same(available, host.Value);
    }

    [Fact]
    public void ValueChanged_RaisesValueChangedAndTracksTheSelectedState()
    {
        var item = Item(1, DeviceState.Available);
        var sut = CreateSut();

        Select(sut, item);
        item.State = DeviceState.InUse;

        Assert.Equal(["null->1"], host.ValueChanges);
        Assert.Equal(Miscellaneous.DeviceInUse, host.SelectedStateMessage);
    }

    [Fact]
    public void ValueChanged_AwayFromAnItem_StopsTrackingItsState()
    {
        var first = Item(1, DeviceState.Available);
        var second = Item(2, DeviceState.Available);
        var sut = CreateSut();
        Select(sut, first);
        Select(sut, second);

        first.State = DeviceState.Disconnected;

        Assert.Equal(string.Empty, host.SelectedStateMessage);
    }

    [Fact]
    public async Task LoadedAsync_WithAutoRefresh_StartsWatchingAndRefreshes()
    {
        var sut = CreateSut();

        await sut.LoadedAsync();

        Assert.Equal(["start", "refresh"], serviceCalls);
    }

    [Fact]
    public async Task LoadedAsync_WithoutAutoRefresh_OnlyRefreshes()
    {
        host.AutoRefreshOnDeviceChange = false;
        var sut = CreateSut();

        await sut.LoadedAsync();

        Assert.Equal(["refresh"], serviceCalls);
    }

    [Fact]
    public async Task LoadedAsync_RefreshFails_DoesNotThrow()
    {
        refresh = () => Task.FromException(new InvalidOperationException("probe failed"));
        var sut = CreateSut();

        var exception = await Record.ExceptionAsync(sut.LoadedAsync);

        Assert.Null(exception);
    }

    [Fact]
    public async Task AutoRefreshTurnedOffWhileLoaded_StopsWatching()
    {
        var sut = CreateSut();
        await sut.LoadedAsync();
        serviceCalls.Clear();

        host.AutoRefreshOnDeviceChange = false;
        sut.AutoRefreshOnDeviceChangeChanged();

        Assert.Equal(["stop"], serviceCalls);
    }

    [Fact]
    public async Task AutoRefreshTurnedOnWhileLoaded_StartsWatching()
    {
        host.AutoRefreshOnDeviceChange = false;
        var sut = CreateSut();
        await sut.LoadedAsync();
        serviceCalls.Clear();

        host.AutoRefreshOnDeviceChange = true;
        sut.AutoRefreshOnDeviceChangeChanged();

        Assert.Equal(["start"], serviceCalls);
    }

    [Fact]
    public async Task AutoRefreshChangedWhileUnloaded_DoesNotTouchTheService()
    {
        var sut = CreateSut();
        await sut.LoadedAsync();
        sut.Unloaded();
        serviceCalls.Clear();

        host.AutoRefreshOnDeviceChange = false;
        sut.AutoRefreshOnDeviceChangeChanged();
        host.AutoRefreshOnDeviceChange = true;
        sut.AutoRefreshOnDeviceChangeChanged();

        Assert.Empty(serviceCalls);
    }

    [Fact]
    public void Unloaded_StopsWatching()
    {
        var sut = CreateSut();

        sut.Unloaded();

        Assert.Equal(["stop"], serviceCalls);
    }

    private static RunningProcessInfo Item(
        int processId,
        DeviceState state)
        => new(processId, "proc", "title", mainModulePath: null) { State = state };

    private void Select(
        DevicePickerController<RunningProcessInfo> sut,
        RunningProcessInfo? value)
    {
        var old = host.Value;
        host.Value = value;
        sut.ValueChanged(old, value);
    }

    private DevicePickerController<RunningProcessInfo> CreateSut()
    {
        var controller = new DevicePickerController<RunningProcessInfo>(
            host,
            items,
            startWatching: () => serviceCalls.Add("start"),
            stopWatching: () => serviceCalls.Add("stop"),
            refreshAsync: () =>
            {
                serviceCalls.Add("refresh");
                return refresh();
            });

        keepAlive.Add(controller);
        return controller;
    }

    private sealed class FakeHost : IDevicePickerHost<RunningProcessInfo>
    {
        public RunningProcessInfo? Value { get; set; }

        public bool AutoRefreshOnDeviceChange { get; set; } = true;

        public bool ClearValueOnDisconnect { get; set; }

        public bool AutoRebindOnReconnect { get; set; } = true;

        public bool AutoSelectFirstAvailable { get; set; }

        public string SelectedStateMessage { get; private set; } = string.Empty;

        public List<string> StateChanges { get; } = [];

        public List<string> Lost { get; } = [];

        public List<string> Reconnected { get; } = [];

        public List<string> ValueChanges { get; } = [];

        public void SetSelectedStateMessage(string message)
            => SelectedStateMessage = message;

        public void RaiseValueChanged(
            RunningProcessInfo? oldValue,
            RunningProcessInfo? newValue)
            => ValueChanges.Add($"{oldValue?.DeviceId ?? "null"}->{newValue?.DeviceId ?? "null"}");

        public void RaiseDeviceLost(RunningProcessInfo device)
            => Lost.Add(device.DeviceId);

        public void RaiseDeviceReconnected(RunningProcessInfo device)
            => Reconnected.Add(device.DeviceId);

        public void RaiseDeviceStateChanged(
            string deviceId,
            DeviceState oldState,
            DeviceState newState)
            => StateChanges.Add($"{deviceId}:{oldState}->{newState}");
    }
}