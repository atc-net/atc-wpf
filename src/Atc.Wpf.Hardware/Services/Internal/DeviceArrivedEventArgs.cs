namespace Atc.Wpf.Hardware.Services.Internal;

internal sealed class DeviceArrivedEventArgs(DeviceInformation device) : EventArgs
{
    public DeviceInformation Device { get; } = device;
}