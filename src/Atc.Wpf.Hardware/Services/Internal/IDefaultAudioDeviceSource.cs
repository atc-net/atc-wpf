namespace Atc.Wpf.Hardware.Services.Internal;

/// <summary>
/// Reports the system default audio endpoint of one kind and when it changes.
/// </summary>
internal interface IDefaultAudioDeviceSource
{
    /// <summary>
    /// Occurs when the default endpoint changes. May be raised on any thread.
    /// </summary>
    event EventHandler? DefaultDeviceChanged;

    /// <summary>
    /// Gets the device ID of the current default endpoint, or <see langword="null"/> when there is none.
    /// </summary>
    string? GetDefaultDeviceId();
}