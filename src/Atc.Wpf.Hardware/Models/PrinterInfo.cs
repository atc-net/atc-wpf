namespace Atc.Wpf.Hardware.Models;

public sealed partial class PrinterInfo : ObservableObject, IDeviceInfo
{
    private bool isDefault;
    private string queueStatus;

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
        this.isDefault = isDefault;
        this.queueStatus = queueStatus;
    }

    public string Name { get; }

    public string FullName { get; }

    public bool IsLocal { get; }

    public bool IsShared { get; }

    public bool IsDefault
    {
        get => isDefault;
        internal set
        {
            if (value == isDefault)
            {
                return;
            }

            isDefault = value;
            RaisePropertyChanged();
        }
    }

    public string QueueStatus
    {
        get => queueStatus;
        internal set
        {
            if (string.Equals(value, queueStatus, StringComparison.Ordinal))
            {
                return;
            }

            queueStatus = value;
            RaisePropertyChanged();
        }
    }

    public string DeviceId
        => FullName;

    public string FriendlyName
        => Name;

    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    public override string ToString()
        => IsDefault ? $"{Name} ★" : Name;
}