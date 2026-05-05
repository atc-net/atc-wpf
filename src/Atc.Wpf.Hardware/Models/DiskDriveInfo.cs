namespace Atc.Wpf.Hardware.Models;

public sealed partial class DiskDriveInfo : ObservableObject, IDeviceInfo
{
    public DiskDriveInfo(
        string deviceId,
        string friendlyName,
        System.IO.DriveType driveType,
        bool isReady,
        long? totalSize,
        long? availableFreeSpace)
    {
        DeviceId = deviceId;
        FriendlyName = friendlyName;
        DriveType = driveType;
        IsReady = isReady;
        TotalSize = totalSize;
        AvailableFreeSpace = availableFreeSpace;
    }

    public string DeviceId { get; }

    public string FriendlyName { get; }

    public System.IO.DriveType DriveType { get; }

    public bool IsReady { get; }

    public long? TotalSize { get; }

    public long? AvailableFreeSpace { get; }

    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    public override string ToString()
        => string.IsNullOrWhiteSpace(FriendlyName) || string.Equals(FriendlyName, DeviceId, StringComparison.Ordinal)
            ? $"{DeviceId} ({DriveType})"
            : $"{DeviceId} {FriendlyName} ({DriveType})";
}