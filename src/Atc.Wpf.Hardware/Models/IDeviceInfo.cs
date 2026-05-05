namespace Atc.Wpf.Hardware.Models;

public interface IDeviceInfo
{
    string DeviceId { get; }

    string FriendlyName { get; }

    DeviceState State { get; }
}