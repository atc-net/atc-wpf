namespace Atc.Wpf.Hardware.Models;

public sealed partial class PrinterInfo : ObservableObject, IDeviceInfo
{
    public PrinterInfo(
        string name,
        string fullName,
        bool isLocal,
        bool isShared,
        bool isDefault,
        string queueStatus)
    {
        Name = name;
        FullName = fullName;
        IsLocal = isLocal;
        IsShared = isShared;
        IsDefault = isDefault;
        QueueStatus = queueStatus;
    }

    public string Name { get; }

    public string FullName { get; }

    public bool IsLocal { get; }

    public bool IsShared { get; }

    public bool IsDefault { get; }

    public string QueueStatus { get; }

    public string DeviceId
        => FullName;

    public string FriendlyName
        => Name;

    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    public override string ToString()
        => IsDefault ? $"{Name} ★" : Name;
}