namespace Atc.Wpf.Hardware.Services.Internal;

internal sealed class DeviceRemovedEventArgs(string deviceId) : EventArgs
{
    public string DeviceId { get; } = deviceId;
}