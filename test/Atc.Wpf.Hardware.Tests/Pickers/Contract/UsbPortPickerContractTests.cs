namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class UsbPortPickerContractTests : DevicePickerContractTests<UsbPortPicker, UsbDeviceInfo, IUsbDeviceService>
{
    protected override IUsbDeviceService CreateService(
        ObservableCollection<UsbDeviceInfo> devices,
        Func<Task> refreshAsync)
    {
        var service = Substitute.For<IUsbDeviceService>();
        service.Devices.Returns(devices);
        service.RefreshAsync().Returns(_ => refreshAsync());
        return service;
    }

    protected override UsbPortPicker CreatePicker(IUsbDeviceService service)
        => new(service);

    protected override UsbDeviceInfo CreateItem(int id)
        => new UsbDeviceInfo($"usb-{id}", $"Device {id}", vendorId: null, productId: null, pnpClass: null, interfaceEnabled: true);

    protected override void SetState(
        UsbDeviceInfo item,
        DeviceState state)
        => item.State = state;
}