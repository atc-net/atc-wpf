namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Describes a video capture device (camera).
/// </summary>
public sealed partial class UsbCameraInfo : ObservableObject, IDeviceInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UsbCameraInfo"/> class.
    /// </summary>
    /// <param name="deviceId">The WinRT device ID of the camera.</param>
    /// <param name="friendlyName">The display name of the camera.</param>
    /// <param name="panel">Where the camera is mounted on the enclosure.</param>
    /// <param name="isEnabled">Whether the camera interface is enabled.</param>
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

    /// <inheritdoc />
    public string DeviceId { get; }

    /// <inheritdoc />
    public string FriendlyName { get; }

    /// <summary>
    /// Gets where the camera is mounted on the enclosure.
    /// </summary>
    public CameraPanel Panel { get; }

    /// <summary>
    /// Gets a value indicating whether the camera interface is enabled.
    /// </summary>
    public bool IsEnabled { get; }

    /// <summary>
    /// The current state of the camera.
    /// </summary>
    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    /// <summary>
    /// The video formats the camera supports, or <see langword="null"/> until they have been read.
    /// </summary>
    [ObservableProperty]
    private IReadOnlyList<UsbCameraFormat>? supportedFormats;

    /// <inheritdoc />
    public override string ToString()
        => Panel is CameraPanel.Unknown
            ? FriendlyName
            : $"{FriendlyName} ({Panel})";
}