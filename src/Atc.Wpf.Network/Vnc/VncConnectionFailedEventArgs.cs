namespace Atc.Wpf.Network.Vnc;

/// <summary>
/// Provides data for the <see cref="VncConnectionService.ConnectionFailed"/> event.
/// </summary>
public sealed class VncConnectionFailedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VncConnectionFailedEventArgs"/> class.
    /// </summary>
    /// <param name="message">The localized message describing why the connection failed.</param>
    public VncConnectionFailedEventArgs(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Gets the localized message describing why the connection failed.
    /// </summary>
    public string Message { get; }
}