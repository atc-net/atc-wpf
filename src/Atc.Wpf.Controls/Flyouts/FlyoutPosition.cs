namespace Atc.Wpf.Controls.Flyouts;

/// <summary>
/// Specifies the position from which a flyout slides in.
/// </summary>
public enum FlyoutPosition
{
    /// <summary>
    /// The flyout slides in from the right edge (default, Azure Portal style).
    /// </summary>
    Right,

    /// <summary>
    /// The flyout slides in from the left edge.
    /// </summary>
    Left,

    /// <summary>
    /// The flyout slides in from the top edge.
    /// </summary>
    Top,

    /// <summary>
    /// The flyout slides in from the bottom edge.
    /// </summary>
    Bottom,

    /// <summary>
    /// The flyout appears centered as a modal-like overlay.
    /// </summary>
    Center,
}