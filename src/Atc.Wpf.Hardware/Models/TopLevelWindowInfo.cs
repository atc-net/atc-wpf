namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Describes a top-level window.
/// </summary>
public sealed partial class TopLevelWindowInfo : ObservableObject, IDeviceInfo
{
    private string title;

    /// <summary>
    /// Initializes a new instance of the <see cref="TopLevelWindowInfo"/> class.
    /// </summary>
    /// <param name="handle">The native window handle (<c>HWND</c>).</param>
    /// <param name="title">The window title.</param>
    /// <param name="className">The window class name.</param>
    /// <param name="processId">The ID of the process that owns the window.</param>
    /// <param name="processName">The name of the process that owns the window.</param>
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

    /// <summary>
    /// Gets the native window handle (<c>HWND</c>).
    /// </summary>
    public IntPtr Handle { get; }

    /// <summary>
    /// Gets the window title.
    /// </summary>
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

    /// <summary>
    /// Gets the window class name.
    /// </summary>
    public string ClassName { get; }

    /// <summary>
    /// Gets the ID of the process that owns the window.
    /// </summary>
    public int ProcessId { get; }

    /// <summary>
    /// Gets the name of the process that owns the window.
    /// </summary>
    public string ProcessName { get; }

    /// <summary>
    /// Gets the window handle as an invariant-culture string.
    /// </summary>
    public string DeviceId
        => Handle.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// Gets the window title, or the class name in parentheses when the title is empty.
    /// </summary>
    public string FriendlyName
        => string.IsNullOrEmpty(Title) ? $"({ClassName})" : Title;

    /// <summary>
    /// The current state of the window.
    /// </summary>
    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    /// <inheritdoc />
    public override string ToString()
        => $"{FriendlyName} — {ProcessName}";
}