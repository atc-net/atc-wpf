namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class BluetoothDevicePickerContractTests : DevicePickerContractTests<BluetoothDevicePicker, BluetoothDeviceInfo, IBluetoothDeviceService>
{
    protected override IBluetoothDeviceService CreateService(
        ObservableCollection<BluetoothDeviceInfo> devices,
        Func<Task> refreshAsync)
    {
        var service = Substitute.For<IBluetoothDeviceService>();
        service.Devices.Returns(devices);
        service.RefreshAsync().Returns(_ => refreshAsync());
        return service;
    }

    protected override BluetoothDevicePicker CreatePicker(
        IBluetoothDeviceService service)
        => new(service);

    protected override BluetoothDeviceInfo CreateItem(int id)
        => new BluetoothDeviceInfo($"bt-{id}", $"Headset {id}", isPaired: true, isConnected: true);

    protected override void SetState(
        BluetoothDeviceInfo item,
        DeviceState state)
        => item.State = state;
}