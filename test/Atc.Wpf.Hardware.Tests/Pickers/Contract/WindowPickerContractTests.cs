namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class WindowPickerContractTests : DevicePickerContractTests<WindowPicker, TopLevelWindowInfo, IWindowService>
{
    protected override bool WatchesWhileLoaded => false;

    protected override IWindowService CreateService(
        ObservableCollection<TopLevelWindowInfo> devices,
        Func<Task> refreshAsync)
    {
        var service = Substitute.For<IWindowService>();
        service.Windows.Returns(devices);
        service.RefreshAsync().Returns(_ => refreshAsync());
        return service;
    }

    protected override WindowPicker CreatePicker(IWindowService service)
        => new(service);

    protected override TopLevelWindowInfo CreateItem(int id)
        => new TopLevelWindowInfo(id, $"Window {id}", "Class", processId: id, processName: $"process{id}");

    protected override void SetState(
        TopLevelWindowInfo item,
        DeviceState state)
        => item.State = state;
}