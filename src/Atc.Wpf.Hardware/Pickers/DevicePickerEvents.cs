namespace Atc.Wpf.Hardware.Pickers;

/// <summary>
/// Routed events shared by all hardware pickers and their <c>Label*</c> wrappers.
/// </summary>
public static class DevicePickerEvents
{
    /// <summary>
    /// Identifies the <c>DeviceError</c> routed event: a picker could not start watching for device changes
    /// or could not read its device list. Every picker and <c>Label*</c> wrapper adds itself as an owner, so a
    /// handler on a wrapper or any ancestor receives the event of the picker inside it.
    /// </summary>
    public static readonly RoutedEvent DeviceErrorEvent = EventManager.RegisterRoutedEvent(
        "DeviceError",
        RoutingStrategy.Bubble,
        typeof(EventHandler<DeviceErrorRoutedEventArgs>),
        typeof(DevicePickerEvents));
}