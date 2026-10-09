namespace Atc.Wpf.Controls.Zoom;

/// <summary>Specifies how the minimum zoom level of a <see cref="ZoomBox"/> is determined.</summary>
public enum ZoomMinimumType
{
    /// <summary>The minimum is the zoom level at which the content fits the screen.</summary>
    FitScreen,

    /// <summary>The minimum is the zoom level at which the content fills the screen.</summary>
    FillScreen,

    /// <summary>The minimum is the explicit <see cref="ZoomBox.MinimumZoom"/> value.</summary>
    MinimumZoom,
}