namespace Atc.Wpf.Hardware.Models;

public sealed partial class DisplayInfo : ObservableObject, IDeviceInfo
{
    private Rect bounds;
    private Rect workingArea;
    private bool isPrimary;

    public DisplayInfo(
        IntPtr handle,
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

    public IntPtr Handle { get; }

    public string DeviceName { get; }

    public Rect Bounds
    {
        get => bounds;
        internal set
        {
            if (value == bounds)
            {
                return;
            }

            bounds = value;
            RaisePropertyChanged();
        }
    }

    public Rect WorkingArea
    {
        get => workingArea;
        internal set
        {
            if (value == workingArea)
            {
                return;
            }

            workingArea = value;
            RaisePropertyChanged();
        }
    }

    public bool IsPrimary
    {
        get => isPrimary;
        internal set
        {
            if (value == isPrimary)
            {
                return;
            }

            isPrimary = value;
            RaisePropertyChanged();
        }
    }

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