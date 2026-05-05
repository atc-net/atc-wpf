namespace Atc.Wpf.Hardware.Models;

public sealed class DeviceStateChangedRoutedEventArgs : RoutedEventArgs
{
    public DeviceStateChangedRoutedEventArgs(
        RoutedEvent routedEvent,
        string deviceId,
        DeviceState oldState,
        DeviceState newState)
        : base(routedEvent)
    {
        DeviceId = deviceId;
        OldState = oldState;
        NewState = newState;
    }

    public string DeviceId { get; }

    public DeviceState OldState { get; }

    public DeviceState NewState { get; }
}