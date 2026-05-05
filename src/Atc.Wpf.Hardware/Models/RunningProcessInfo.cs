namespace Atc.Wpf.Hardware.Models;

public sealed partial class RunningProcessInfo : ObservableObject, IDeviceInfo
{
    public RunningProcessInfo(
        int processId,
        string processName,
        string mainWindowTitle,
        string? mainModulePath)
    {
        ProcessId = processId;
        ProcessName = processName;
        MainWindowTitle = mainWindowTitle;
        MainModulePath = mainModulePath;
    }

    public int ProcessId { get; }

    public string ProcessName { get; }

    public string MainWindowTitle { get; }

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