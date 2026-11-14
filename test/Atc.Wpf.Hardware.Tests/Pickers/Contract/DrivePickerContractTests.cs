namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class DrivePickerContractTests : DevicePickerContractTests<DrivePicker, DiskDriveInfo, IDriveService>
{
    protected override IDriveService CreateService(
        ObservableCollection<DiskDriveInfo> devices,
        Func<Task> refreshAsync)
    {
        var service = Substitute.For<IDriveService>();
        service.Drives.Returns(devices);
        service.RefreshAsync().Returns(_ => refreshAsync());
        return service;
    }

    protected override DrivePicker CreatePicker(IDriveService service)
        => new(service);

    protected override DiskDriveInfo CreateItem(int id)
        => new DiskDriveInfo($"{(char)('C' + id)}:\\", $"Drive {id}", DriveType.Fixed, isReady: true, totalSize: null, availableFreeSpace: null);

    protected override void SetState(
        DiskDriveInfo item,
        DeviceState state)
        => item.State = state;
}