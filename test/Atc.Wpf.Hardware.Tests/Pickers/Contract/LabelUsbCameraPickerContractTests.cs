namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class LabelUsbCameraPickerContractTests : LabelPickerContractTests<LabelUsbCameraPicker, UsbCameraPicker, UsbCameraInfo>
{
    protected override IReadOnlyList<string> ExtraForwardedFlags
        => ["ShowLivePreview"];

    protected override UsbCameraInfo CreateItem(int id)
        => new UsbCameraInfo($"camera-{id}", $"Camera {id}", CameraPanel.Unknown, isEnabled: true);

    protected override void SetState(
        UsbCameraInfo item,
        DeviceState state)
        => item.State = state;
}