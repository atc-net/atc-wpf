namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class UsbCameraPickerContractTests : DevicePickerContractTests<UsbCameraPicker, UsbCameraInfo, IUsbCameraService>
{
    protected override IUsbCameraService CreateService(
        ObservableCollection<UsbCameraInfo> devices,
        Func<Task> refreshAsync)
    {
        var service = Substitute.For<IUsbCameraService>();
        service.Cameras.Returns(devices);
        service.RefreshAsync().Returns(_ => refreshAsync());
        return service;
    }

    protected override UsbCameraPicker CreatePicker(IUsbCameraService service)
        => new(service);

    protected override UsbCameraInfo CreateItem(int id)
        => new UsbCameraInfo($"camera-{id}", $"Camera {id}", CameraPanel.Unknown, isEnabled: true);

    protected override void SetState(
        UsbCameraInfo item,
        DeviceState state)
        => item.State = state;
}