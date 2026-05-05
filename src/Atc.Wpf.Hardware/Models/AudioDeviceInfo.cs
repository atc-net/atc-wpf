namespace Atc.Wpf.Hardware.Models;

public sealed partial class AudioDeviceInfo : ObservableObject, IDeviceInfo
{
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

    public string DeviceId { get; }

    public string FriendlyName { get; }

    public AudioDeviceKind Kind { get; }

    public bool IsEnabled { get; }

    public bool IsDefault { get; }

    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    public override string ToString()
        => IsDefault
            ? $"{FriendlyName} ★"
            : FriendlyName;
}