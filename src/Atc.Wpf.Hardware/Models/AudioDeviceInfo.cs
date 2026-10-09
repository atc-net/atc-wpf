namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Describes an audio input (capture) or output (render) endpoint.
/// </summary>
public sealed partial class AudioDeviceInfo : ObservableObject, IDeviceInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioDeviceInfo"/> class.
    /// </summary>
    /// <param name="deviceId">The WinRT device ID of the endpoint.</param>
    /// <param name="friendlyName">The display name of the endpoint.</param>
    /// <param name="kind">Whether the endpoint captures or renders audio.</param>
    /// <param name="isEnabled">Whether the endpoint interface is enabled.</param>
    /// <param name="isDefault">Whether the endpoint is the system default for its kind.</param>
    public AudioDeviceInfo(
        string deviceId,
        string friendlyName,
        AudioDeviceKind kind,
        bool isEnabled,
        bool isDefault)
    {
        DeviceId = deviceId;
        FriendlyName = friendlyName;
        Kind = kind;
        IsEnabled = isEnabled;
        IsDefault = isDefault;
    }

    /// <inheritdoc />
    public string DeviceId { get; }

    /// <inheritdoc />
    public string FriendlyName { get; }

    /// <summary>
    /// Gets the direction of the endpoint: capture (input) or render (output).
    /// </summary>
    public AudioDeviceKind Kind { get; }

    /// <summary>
    /// Gets a value indicating whether the endpoint interface is enabled.
    /// </summary>
    public bool IsEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether the endpoint is the system default for its kind.
    /// </summary>
    public bool IsDefault { get; }

    /// <summary>
    /// The current state of the endpoint.
    /// </summary>
    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    /// <inheritdoc />
    public override string ToString()
        => IsDefault
            ? $"{FriendlyName} ★"
            : FriendlyName;
}