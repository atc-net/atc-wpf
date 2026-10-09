namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Describes a running process.
/// </summary>
public sealed partial class RunningProcessInfo : ObservableObject, IDeviceInfo
{
    private string mainWindowTitle;

    /// <summary>
    /// Initializes a new instance of the <see cref="RunningProcessInfo"/> class.
    /// </summary>
    /// <param name="processId">The process ID.</param>
    /// <param name="processName">The name of the process.</param>
    /// <param name="mainWindowTitle">The title of the process's main window, or an empty string.</param>
    /// <param name="mainModulePath">The full path of the process's main module, if it could be read.</param>
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

    /// <summary>
    /// Gets the process ID.
    /// </summary>
    public int ProcessId { get; }

    /// <summary>
    /// Gets the name of the process.
    /// </summary>
    public string ProcessName { get; }

    /// <summary>
    /// Gets the title of the process's main window, or an empty string when it has none.
    /// </summary>
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

    /// <summary>
    /// Gets the full path of the process's main module, or <see langword="null"/> when it could not be read.
    /// </summary>
    public string? MainModulePath { get; }

    /// <summary>
    /// Gets the process ID as an invariant-culture string.
    /// </summary>
    public string DeviceId => ProcessId.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// Gets the process name, followed by the main window title when there is one.
    /// </summary>
    public string FriendlyName => string.IsNullOrEmpty(MainWindowTitle)
        ? ProcessName
        : $"{ProcessName} — {MainWindowTitle}";

    /// <summary>
    /// The current state of the process.
    /// </summary>
    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    /// <inheritdoc />
    public override string ToString()
        => $"{FriendlyName} (PID {ProcessId})";
}