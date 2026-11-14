namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class SerialPortPickerContractTests : DevicePickerContractTests<SerialPortPicker, SerialPortInfo, ISerialPortService>
{
    protected override ISerialPortService CreateService(
        ObservableCollection<SerialPortInfo> devices,
        Func<Task> refreshAsync)
    {
        var service = Substitute.For<ISerialPortService>();
        service.Ports.Returns(devices);
        service.RefreshAsync().Returns(_ => refreshAsync());
        return service;
    }

    protected override SerialPortPicker CreatePicker(ISerialPortService service)
        => new(service);

    protected override SerialPortInfo CreateItem(int id)
        => new SerialPortInfo($"port-{id}", $"COM{id}", $"Port {id}", vendorId: null, productId: null);

    protected override void SetState(
        SerialPortInfo item,
        DeviceState state)
        => item.State = state;
}