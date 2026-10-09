namespace Atc.Wpf.Theming.Controls.Images;

/// <summary>
/// Specifies how a <see cref="MultiFrameImage"/> picks and renders a frame.
/// </summary>
public enum MultiFrameImageMode
{
    /// <summary>
    /// Uses the smallest frame that is at least as large as the render size, scaled down to fit.
    /// </summary>
    ScaleDownLargerFrame,

    /// <summary>
    /// Uses the largest frame that fits within the render size, drawn centered without scaling.
    /// </summary>
    NoScaleSmallerFrame,
}