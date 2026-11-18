namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Describes a display monitor.
/// </summary>
public sealed partial class DisplayInfo : ObservableObject, IDeviceInfo
{
    /// <summary>
    /// Gets the full bounds of the monitor in virtual-screen coordinates.
    /// </summary>
    [ObservableProperty(SetterAccessModifier = PropertyAccessModifier.Internal)]
    private Rect bounds;

    /// <summary>
    /// Gets the working area of the monitor in virtual-screen coordinates, excluding taskbars and docked windows.
    /// </summary>
    [ObservableProperty(SetterAccessModifier = PropertyAccessModifier.Internal)]
    private Rect workingArea;

    /// <summary>
    /// Gets a value indicating whether the monitor is the primary display.
    /// </summary>
    [ObservableProperty(SetterAccessModifier = PropertyAccessModifier.Internal)]
    private bool isPrimary;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayInfo"/> class.
    /// </summary>
    /// <param name="handle">The native monitor handle (<c>HMONITOR</c>).</param>
    /// <param name="deviceName">The device name of the monitor, for example <c>\\.\DISPLAY1</c>.</param>
    /// <param name="bounds">The full bounds of the monitor.</param>
    /// <param name="workingArea">The working area of the monitor, excluding taskbars and docked windows.</param>
    /// <param name="isPrimary">Whether the monitor is the primary display.</param>
    public DisplayInfo(
        nint handle,
        string deviceName,
        Rect bounds,
        Rect workingArea,
        bool isPrimary)
    {
        Handle = handle;
        DeviceName = deviceName;
        this.bounds = bounds;
        this.workingArea = workingArea;
        this.isPrimary = isPrimary;
    }

    /// <summary>
    /// Gets the native monitor handle (<c>HMONITOR</c>).
    /// </summary>
    public nint Handle { get; }

    /// <summary>
    /// Gets the device name of the monitor, for example <c>\\.\DISPLAY1</c>.
    /// </summary>
    public string DeviceName { get; }

    /// <inheritdoc />
    public string DeviceId
        => DeviceName;

    /// <inheritdoc />
    public string FriendlyName
        => DeviceName;

    /// <summary>
    /// The current state of the monitor.
    /// </summary>
    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    /// <inheritdoc />
    public override string ToString()
        => IsPrimary
            ? $"{DeviceName} ★ ({(int)Bounds.Width}×{(int)Bounds.Height})"
            : $"{DeviceName} ({(int)Bounds.Width}×{(int)Bounds.Height})";
}