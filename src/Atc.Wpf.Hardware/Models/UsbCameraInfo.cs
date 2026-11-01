namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Describes a video capture device (camera).
/// </summary>
public sealed partial class UsbCameraInfo : ObservableObject, IDeviceInfo
{
    private string friendlyName;
    private bool isEnabled;

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
        this.friendlyName = friendlyName;
        Panel = panel;
        this.isEnabled = isEnabled;
    }

    /// <inheritdoc />
    public string DeviceId { get; }

    /// <inheritdoc />
    /// <remarks>Follows a rename of the camera while the service is watching.</remarks>
    public string FriendlyName
    {
        get => friendlyName;
        internal set
        {
            if (string.Equals(value, friendlyName, StringComparison.Ordinal))
            {
                return;
            }

            friendlyName = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(DisplayName));
        }
    }

    /// <summary>
    /// Gets where the camera is mounted on the enclosure.
    /// </summary>
    public CameraPanel Panel { get; }

    /// <summary>
    /// Gets a value indicating whether the camera interface is enabled.
    /// Follows the camera being disabled or enabled while the service is watching.
    /// </summary>
    public bool IsEnabled
    {
        get => isEnabled;
        internal set
        {
            if (value == isEnabled)
            {
                return;
            }

            isEnabled = value;
            RaisePropertyChanged();
        }
    }

    /// <summary>
    /// Gets the text shown for the camera: its name, followed by where it is mounted when that is known.
    /// </summary>
    public string DisplayName
        => Panel is CameraPanel.Unknown
            ? FriendlyName
            : $"{FriendlyName} ({Panel})";

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
        => DisplayName;
}