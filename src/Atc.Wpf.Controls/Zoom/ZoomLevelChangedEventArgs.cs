namespace Atc.Wpf.Controls.Zoom;

/// <summary>
/// Provides data for the <see cref="ZoomBox.ZoomLevelChanged"/> event.
/// </summary>
public sealed class ZoomLevelChangedEventArgs : EventArgs
{
    public ZoomLevelChangedEventArgs(
        double oldZoom,
        double newZoom,
        double minimumZoom,
        double maximumZoom)
    {
        OldZoom = oldZoom;
        NewZoom = newZoom;
        LevelOfDetail = maximumZoom > minimumZoom
            ? (newZoom - minimumZoom) / (maximumZoom - minimumZoom)
            : 1.0;
    }

    /// <summary>
    /// Gets the previous zoom level.
    /// </summary>
    public double OldZoom { get; }

    /// <summary>
    /// Gets the new zoom level.
    /// </summary>
    public double NewZoom { get; }

    /// <summary>
    /// Gets the normalized level of detail (0.0 at minimum zoom, 1.0 at maximum zoom).
    /// Use this to simplify rendering at low zoom levels.
    /// </summary>
    public double LevelOfDetail { get; }
}