namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Provides data for the <c>DeviceStateChanged</c> routed event raised by the pickers.
/// </summary>
public sealed class DeviceStateChangedRoutedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceStateChangedRoutedEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event these arguments belong to.</param>
    /// <param name="deviceId">The ID of the device whose state changed.</param>
    /// <param name="oldState">The state before the change.</param>
    /// <param name="newState">The state after the change.</param>
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

    /// <summary>
    /// Gets the ID of the device whose state changed.
    /// </summary>
    public string DeviceId { get; }

    /// <summary>
    /// Gets the state before the change.
    /// </summary>
    public DeviceState OldState { get; }

    /// <summary>
    /// Gets the state after the change.
    /// </summary>
    public DeviceState NewState { get; }
}