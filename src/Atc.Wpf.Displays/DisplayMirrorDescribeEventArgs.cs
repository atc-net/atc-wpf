namespace Atc.Wpf.Displays;

/// <summary>
/// The data of <see cref="DisplayMirrorReceiver.DescribeReceived"/>.
/// </summary>
public sealed class DisplayMirrorDescribeEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayMirrorDescribeEventArgs"/> class.
    /// </summary>
    /// <param name="displayId">The display id the description is for.</param>
    /// <param name="describe">The description.</param>
    public DisplayMirrorDescribeEventArgs(
        byte displayId,
        DisplayMirrorDescribe describe)
    {
        ArgumentNullException.ThrowIfNull(describe);

        DisplayId = displayId;
        Describe = describe;
    }

    /// <summary>
    /// Gets the display id the description is for.
    /// </summary>
    public byte DisplayId { get; }

    /// <summary>
    /// Gets the description; it is advisory, the receiver never creates or resizes targets from it.
    /// </summary>
    public DisplayMirrorDescribe Describe { get; }
}