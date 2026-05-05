namespace Atc.Wpf.Hardware.Services.Internal;

internal sealed class DeviceSnapshotEventArgs(DeviceSnapshot snapshot) : EventArgs
{
    public DeviceSnapshot Snapshot { get; } = snapshot;
}