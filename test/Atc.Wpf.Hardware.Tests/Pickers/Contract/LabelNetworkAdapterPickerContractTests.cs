namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class LabelNetworkAdapterPickerContractTests : LabelPickerContractTests<LabelNetworkAdapterPicker, NetworkAdapterPicker, NetworkAdapterInfo>
{
    protected override NetworkAdapterInfo CreateItem(int id)
        => new NetworkAdapterInfo($"adapter-{id}", $"Ethernet {id}", "Adapter", NetworkInterfaceType.Ethernet, "00-11-22-33-44-55", speed: null, isLoopback: false);

    protected override void SetState(
        NetworkAdapterInfo item,
        DeviceState state)
        => item.State = state;
}