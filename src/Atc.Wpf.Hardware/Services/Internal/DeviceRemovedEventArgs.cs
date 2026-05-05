namespace Atc.Wpf.Hardware.Services.Internal;

internal sealed class DeviceRemovedEventArgs(DeviceInformationUpdate update) : EventArgs
{
    public DeviceInformationUpdate Update { get; } = update;
}