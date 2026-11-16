namespace Atc.Wpf.Displays;

/// <summary>
/// The data of <see cref="EPaperFrameBuffer.Refreshed"/>.
/// </summary>
public sealed class EPaperRefreshedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EPaperRefreshedEventArgs"/> class.
    /// </summary>
    /// <param name="kind">The kind of the refresh.</param>
    public EPaperRefreshedEventArgs(EPaperRefreshKind kind)
        => Kind = kind;

    /// <summary>
    /// Gets the kind of the refresh.
    /// </summary>
    public EPaperRefreshKind Kind { get; }
}