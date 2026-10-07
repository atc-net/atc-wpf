namespace Atc.Wpf.Hardware.Services.Internal;

/// <summary>
/// The volume metadata read for one drive letter.
/// </summary>
internal sealed record DriveSnapshot(
    string Name,
    string Label,
    System.IO.DriveType DriveType,
    bool IsReady,
    long? TotalSize,
    long? AvailableFreeSpace);