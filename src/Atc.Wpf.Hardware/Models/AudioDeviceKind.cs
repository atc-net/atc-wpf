namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Specifies the direction of an audio endpoint.
/// </summary>
public enum AudioDeviceKind
{
    /// <summary>
    /// An audio capture endpoint, such as a microphone.
    /// </summary>
    Input,

    /// <summary>
    /// An audio render endpoint, such as speakers or headphones.
    /// </summary>
    Output,
}