namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Provides data for the <c>DeviceError</c> routed event raised by the pickers.
/// </summary>
public sealed class DeviceErrorRoutedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceErrorRoutedEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event these arguments belong to.</param>
    /// <param name="exception">The exception the operation failed with.</param>
    /// <param name="operation">The operation that failed.</param>
    public DeviceErrorRoutedEventArgs(
        RoutedEvent routedEvent,
        Exception exception,
        DevicePickerOperation operation)
        : base(routedEvent)
    {
        Exception = exception;
        Operation = operation;
    }

    /// <summary>
    /// Gets the exception the operation failed with.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Gets the operation that failed.
    /// </summary>
    public DevicePickerOperation Operation { get; }
}