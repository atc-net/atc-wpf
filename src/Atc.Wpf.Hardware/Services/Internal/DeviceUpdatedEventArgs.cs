namespace Atc.Wpf.Hardware.Services.Internal;

internal sealed class DeviceUpdatedEventArgs(DeviceInformationUpdate update) : EventArgs
{
    public DeviceInformationUpdate Update { get; } = update;
}