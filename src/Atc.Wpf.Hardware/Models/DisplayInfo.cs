namespace Atc.Wpf.Hardware.Models;

public sealed partial class DisplayInfo : ObservableObject, IDeviceInfo
{
    public DisplayInfo(
        IntPtr handle,
        string deviceName,
        Rect bounds,
        Rect workingArea,
        bool isPrimary)
    {
        Handle = handle;
        DeviceName = deviceName;
        Bounds = bounds;
        WorkingArea = workingArea;
        IsPrimary = isPrimary;
    }

    public IntPtr Handle { get; }

    public string DeviceName { get; }

    public Rect Bounds { get; }

    public Rect WorkingArea { get; }

    public bool IsPrimary { get; }

    public string DeviceId
        => DeviceName;

    public string FriendlyName
        => DeviceName;

    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    public override string ToString()
        => IsPrimary
            ? $"{DeviceName} ★ ({(int)Bounds.Width}×{(int)Bounds.Height})"
            : $"{DeviceName} ({(int)Bounds.Width}×{(int)Bounds.Height})";
}