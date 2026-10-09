namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Describes a logical drive (drive letter) and its volume.
/// </summary>
public sealed partial class DiskDriveInfo : ObservableObject, IDeviceInfo
{
    private long? availableFreeSpace;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiskDriveInfo"/> class.
    /// </summary>
    /// <param name="deviceId">The drive root name, for example <c>C:\</c>.</param>
    /// <param name="friendlyName">The volume label of the drive.</param>
    /// <param name="driveType">The type of the drive.</param>
    /// <param name="isReady">Whether the drive is ready to be read.</param>
    /// <param name="totalSize">The total size of the volume in bytes, if known.</param>
    /// <param name="availableFreeSpace">The free space available on the volume in bytes, if known.</param>
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
        this.availableFreeSpace = availableFreeSpace;
    }

    /// <inheritdoc />
    public string DeviceId { get; }

    /// <inheritdoc />
    public string FriendlyName { get; }

    /// <summary>
    /// Gets the type of the drive, such as fixed, removable or network.
    /// </summary>
    public System.IO.DriveType DriveType { get; }

    /// <summary>
    /// Gets a value indicating whether the drive is ready to be read.
    /// </summary>
    public bool IsReady { get; }

    /// <summary>
    /// Gets the total size of the volume in bytes, or <see langword="null"/> when the drive is not ready.
    /// </summary>
    public long? TotalSize { get; }

    /// <summary>
    /// Gets the free space available on the volume in bytes, or <see langword="null"/> when the drive is not ready.
    /// </summary>
    public long? AvailableFreeSpace
    {
        get => availableFreeSpace;
        internal set
        {
            if (value == availableFreeSpace)
            {
                return;
            }

            availableFreeSpace = value;
            RaisePropertyChanged();
        }
    }

    /// <summary>
    /// The current state of the drive.
    /// </summary>
    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    /// <inheritdoc />
    public override string ToString()
        => string.IsNullOrWhiteSpace(FriendlyName) || string.Equals(FriendlyName, DeviceId, StringComparison.Ordinal)
            ? $"{DeviceId} ({DriveType})"
            : $"{DeviceId} {FriendlyName} ({DriveType})";
}