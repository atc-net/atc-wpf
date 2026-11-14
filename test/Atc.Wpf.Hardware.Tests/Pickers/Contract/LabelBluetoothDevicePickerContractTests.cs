namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class LabelBluetoothDevicePickerContractTests : LabelPickerContractTests<LabelBluetoothDevicePicker, BluetoothDevicePicker, BluetoothDeviceInfo>
{
    protected override BluetoothDeviceInfo CreateItem(int id)
        => new BluetoothDeviceInfo($"bt-{id}", $"Headset {id}", isPaired: true, isConnected: true);

    protected override void SetState(
        BluetoothDeviceInfo item,
        DeviceState state)
        => item.State = state;
}