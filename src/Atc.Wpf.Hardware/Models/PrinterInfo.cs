namespace Atc.Wpf.Hardware.Models;

/// <summary>
/// Describes a printer (print queue).
/// </summary>
public sealed partial class PrinterInfo : ObservableObject, IDeviceInfo
{
    private bool isDefault;
    private string queueStatus;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrinterInfo"/> class.
    /// </summary>
    /// <param name="name">The name of the print queue.</param>
    /// <param name="fullName">The full name of the print queue, including the print server for a shared queue.</param>
    /// <param name="isLocal">Whether the queue is hosted on this computer.</param>
    /// <param name="isShared">Whether the queue is shared.</param>
    /// <param name="isDefault">Whether the queue is the default printer.</param>
    /// <param name="queueStatus">The status of the queue as text.</param>
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

    /// <summary>
    /// Gets the name of the print queue.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the full name of the print queue, including the print server for a shared queue.
    /// </summary>
    public string FullName { get; }

    /// <summary>
    /// Gets a value indicating whether the queue is hosted on this computer.
    /// </summary>
    public bool IsLocal { get; }

    /// <summary>
    /// Gets a value indicating whether the queue is shared.
    /// </summary>
    public bool IsShared { get; }

    /// <summary>
    /// Gets a value indicating whether the queue is the default printer.
    /// </summary>
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

    /// <summary>
    /// Gets the status of the queue as text, for example "None" or "Paused".
    /// </summary>
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

    /// <inheritdoc />
    public string DeviceId
        => FullName;

    /// <inheritdoc />
    public string FriendlyName
        => Name;

    /// <summary>
    /// The current state of the printer.
    /// </summary>
    [ObservableProperty]
    private DeviceState state = DeviceState.Unknown;

    /// <inheritdoc />
    public override string ToString()
        => IsDefault ? $"{Name} ★" : Name;
}