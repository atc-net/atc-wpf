namespace Atc.Wpf.Hardware.Models;

public sealed partial class TopLevelWindowInfo : ObservableObject, IDeviceInfo
{
    private string title;

    public TopLevelWindowInfo(
        IntPtr handle,
        string title,
        string className,
        int processId,
        string processName)
    {
        Handle = handle;
        this.title = title;
        ClassName = className;
        ProcessId = processId;
        ProcessName = processName;
    }

    public IntPtr Handle { get; }

    public string Title
    {
        get => title;
        internal set
        {
            if (string.Equals(value, title, StringComparison.Ordinal))
            {
                return;
            }

            title = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(FriendlyName));
        }
    }

    public string ClassName { get; }

    public int ProcessId { get; }

    public string ProcessName { get; }

    public string DeviceId
        => Handle.ToString(CultureInfo.InvariantCulture);

    public string FriendlyName
        => string.IsNullOrEmpty(Title) ? $"({ClassName})" : Title;

    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    public override string ToString()
        => $"{FriendlyName} — {ProcessName}";
}