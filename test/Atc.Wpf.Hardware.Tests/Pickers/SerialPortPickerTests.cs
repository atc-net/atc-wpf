namespace Atc.Wpf.Hardware.Tests.Pickers;

public sealed class SerialPortPickerTests
{
    private readonly ObservableCollection<SerialPortInfo> ports = [];
    private readonly ISerialPortService service = Substitute.For<ISerialPortService>();

    public SerialPortPickerTests()
    {
        service.Ports.Returns(ports);
        service.RefreshAsync().Returns(Task.CompletedTask);
    }

    [WpfFact]
    public void DetectInUseState_SelectedPortProbedInUse_IsMarkedInUse()
    {
        var port = AvailablePort();
        service.ProbeInUseAsync(port).Returns(true);
        var picker = new SerialPortPicker(service) { DetectInUseState = true };

        picker.Value = port;

        Assert.Equal(DeviceState.InUse, port.State);
        Assert.Equal(Miscellaneous.DeviceInUse, picker.SelectedStateMessage);
    }

    [WpfFact]
    public void DetectInUseState_SelectedPortProbedFree_StaysAvailable()
    {
        var port = AvailablePort();
        service.ProbeInUseAsync(port).Returns(false);
        var picker = new SerialPortPicker(service) { DetectInUseState = true };

        picker.Value = port;

        Assert.Equal(DeviceState.Available, port.State);
    }

    [WpfFact]
    public void DetectInUseState_DisconnectedPortProbedInUse_StaysDisconnected()
    {
        var port = AvailablePort();
        port.State = DeviceState.Disconnected;
        service.ProbeInUseAsync(port).Returns(true);
        var picker = new SerialPortPicker(service) { DetectInUseState = true };

        picker.Value = port;

        Assert.Equal(DeviceState.Disconnected, port.State);
    }

    [WpfFact]
    public void DetectInUseState_ProbeFails_DoesNotThrowAndKeepsTheState()
    {
        var port = AvailablePort();
        service.ProbeInUseAsync(port).Returns(Task.FromException<bool>(new UnauthorizedAccessException("denied")));
        var picker = new SerialPortPicker(service) { DetectInUseState = true };

        var exception = Record.Exception(() => picker.Value = port);

        Assert.Null(exception);
        Assert.Equal(DeviceState.Available, port.State);
    }

    [WpfFact]
    public void WithoutDetectInUseState_SelectingAPort_DoesNotProbe()
    {
        var port = AvailablePort();
        var picker = new SerialPortPicker(service);

        picker.Value = port;

        _ = service.DidNotReceive().ProbeInUseAsync(Arg.Any<SerialPortInfo>());
    }

    [WpfFact]
    public void DetectInUseState_Loaded_ProbesTheAlreadySelectedPort()
    {
        var port = AvailablePort();
        var picker = new SerialPortPicker(service) { Value = port };
        picker.DetectInUseState = true;
        service.ProbeInUseAsync(port).Returns(true);

        picker.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

        Assert.Equal(DeviceState.InUse, port.State);
    }

    private SerialPortInfo AvailablePort()
    {
        var port = new SerialPortInfo("port-1", "COM1", "Port 1", vendorId: null, productId: null)
        {
            State = DeviceState.Available,
        };
        ports.Add(port);
        return port;
    }
}