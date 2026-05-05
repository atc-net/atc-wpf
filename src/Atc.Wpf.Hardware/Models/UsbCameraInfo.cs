namespace Atc.Wpf.Hardware.Models;

public sealed partial class UsbCameraInfo : ObservableObject, IDeviceInfo
{
    public UsbCameraInfo(
        string deviceId,
        string friendlyName,
        CameraPanel panel,
        bool isEnabled)
    {
        DeviceId = deviceId;
        FriendlyName = friendlyName;
        Panel = panel;
        IsEnabled = isEnabled;
    }

    public string DeviceId { get; }

    public string FriendlyName { get; }

    public CameraPanel Panel { get; }

    public bool IsEnabled { get; }

    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    [ObservableProperty]
    private IReadOnlyList<UsbCameraFormat>? supportedFormats;

    public override string ToString()
        => Panel is CameraPanel.Unknown
            ? FriendlyName
            : $"{FriendlyName} ({Panel})";
}