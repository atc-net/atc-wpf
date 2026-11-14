namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class DisplayPickerContractTests : DevicePickerContractTests<DisplayPicker, DisplayInfo, IDisplayService>
{
    protected override IDisplayService CreateService(
        ObservableCollection<DisplayInfo> devices,
        Func<Task> refreshAsync)
    {
        var service = Substitute.For<IDisplayService>();
        service.Displays.Returns(devices);
        service.RefreshAsync().Returns(_ => refreshAsync());
        return service;
    }

    protected override DisplayPicker CreatePicker(IDisplayService service)
        => new(service);

    protected override DisplayInfo CreateItem(int id)
        => new DisplayInfo(id, $"DISPLAY{id}", new Rect(0, 0, 1920, 1080), new Rect(0, 0, 1920, 1040), isPrimary: id == 1);

    protected override void SetState(
        DisplayInfo item,
        DeviceState state)
        => item.State = state;
}