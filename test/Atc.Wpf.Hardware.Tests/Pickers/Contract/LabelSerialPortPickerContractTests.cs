namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class LabelSerialPortPickerContractTests : LabelPickerContractTests<LabelSerialPortPicker, SerialPortPicker, SerialPortInfo>
{
    protected override IReadOnlyList<string> ExtraForwardedFlags
        => ["DetectInUseState"];

    protected override SerialPortInfo CreateItem(int id)
        => new SerialPortInfo($"port-{id}", $"COM{id}", $"Port {id}", vendorId: null, productId: null);

    protected override void SetState(
        SerialPortInfo item,
        DeviceState state)
        => item.State = state;
}