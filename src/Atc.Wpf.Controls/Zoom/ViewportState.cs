namespace Atc.Wpf.Controls.Zoom;

/// <summary>
/// Captures the zoom level and scroll position of a <see cref="ZoomBox"/>
/// for serialization and later restoration.
/// </summary>
/// <param name="Zoom">The viewport zoom level (1.0 = 100%).</param>
/// <param name="OffsetX">The horizontal content offset.</param>
/// <param name="OffsetY">The vertical content offset.</param>
public sealed record ViewportState(
    double Zoom,
    double OffsetX,
    double OffsetY);