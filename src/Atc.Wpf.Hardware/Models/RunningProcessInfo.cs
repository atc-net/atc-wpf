namespace Atc.Wpf.Hardware.Models;

public sealed partial class RunningProcessInfo : ObservableObject, IDeviceInfo
{
    private string mainWindowTitle;

    public RunningProcessInfo(
        int processId,
        string processName,
        string mainWindowTitle,
        string? mainModulePath)
    {
        ProcessId = processId;
        ProcessName = processName;
        this.mainWindowTitle = mainWindowTitle;
        MainModulePath = mainModulePath;
    }

    public int ProcessId { get; }

    public string ProcessName { get; }

    public string MainWindowTitle
    {
        get => mainWindowTitle;
        internal set
        {
            if (string.Equals(value, mainWindowTitle, StringComparison.Ordinal))
            {
                return;
            }

            mainWindowTitle = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(FriendlyName));
        }
    }

    public string? MainModulePath { get; }

    public string DeviceId => ProcessId.ToString(CultureInfo.InvariantCulture);

    public string FriendlyName => string.IsNullOrEmpty(MainWindowTitle)
        ? ProcessName
        : $"{ProcessName} — {MainWindowTitle}";

    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    public override string ToString()
        => $"{FriendlyName} (PID {ProcessId})";
}