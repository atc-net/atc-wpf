namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class NetworkAdapterPickerContractTests : DevicePickerContractTests<NetworkAdapterPicker, NetworkAdapterInfo, INetworkAdapterService>
{
    protected override INetworkAdapterService CreateService(
        ObservableCollection<NetworkAdapterInfo> devices,
        Func<Task> refreshAsync)
    {
        var service = Substitute.For<INetworkAdapterService>();
        service.Adapters.Returns(devices);
        service.RefreshAsync().Returns(_ => refreshAsync());
        return service;
    }

    protected override NetworkAdapterPicker CreatePicker(
        INetworkAdapterService service)
        => new(service);

    protected override NetworkAdapterInfo CreateItem(int id)
        => new NetworkAdapterInfo($"adapter-{id}", $"Ethernet {id}", "Adapter", NetworkInterfaceType.Ethernet, "00-11-22-33-44-55", speed: null, isLoopback: false);

    protected override void SetState(
        NetworkAdapterInfo item,
        DeviceState state)
        => item.State = state;
}