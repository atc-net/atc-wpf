namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class ProcessPickerContractTests : DevicePickerContractTests<ProcessPicker, RunningProcessInfo, IProcessService>
{
    protected override bool WatchesWhileLoaded => false;

    protected override IProcessService CreateService(
        ObservableCollection<RunningProcessInfo> devices,
        Func<Task> refreshAsync)
    {
        var service = Substitute.For<IProcessService>();
        service.Processes.Returns(devices);
        service.RefreshAsync().Returns(_ => refreshAsync());
        return service;
    }

    protected override ProcessPicker CreatePicker(IProcessService service)
        => new(service);

    protected override RunningProcessInfo CreateItem(int id)
        => new RunningProcessInfo(id, $"process{id}", $"Window {id}", mainModulePath: null);

    protected override void SetState(
        RunningProcessInfo item,
        DeviceState state)
        => item.State = state;
}