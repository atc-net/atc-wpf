namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class LabelUsbPortPickerContractTests : LabelPickerContractTests<LabelUsbPortPicker, UsbPortPicker, UsbDeviceInfo>
{
    protected override IReadOnlyList<string> ExtraForwardedFlags
        => ["DetectInUseState"];

    protected override UsbDeviceInfo CreateItem(int id)
        => new UsbDeviceInfo($"usb-{id}", $"Device {id}", vendorId: null, productId: null, pnpClass: null, interfaceEnabled: true);

    protected override void SetState(
        UsbDeviceInfo item,
        DeviceState state)
        => item.State = state;
}