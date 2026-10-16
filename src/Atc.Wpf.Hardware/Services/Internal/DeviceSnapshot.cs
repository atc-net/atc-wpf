namespace Atc.Wpf.Hardware.Services.Internal;

internal sealed class DeviceSnapshot
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public bool IsEnabled { get; init; }

    public bool IsDefault { get; init; }

    public CameraPanel? Panel { get; init; }

    public bool? IsPaired { get; init; }

    /// <summary>
    /// The <c>System.Devices.Aep.IsConnected</c> property; <see langword="null"/> when it was not requested or not reported.
    /// </summary>
    public bool? IsConnected { get; init; }

    public string? PortName { get; init; }
}