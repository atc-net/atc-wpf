namespace Atc.Wpf.Controls.Zoom;

/// <summary>
/// Specifies how two linked <see cref="ZoomBox"/> viewports synchronize.
/// </summary>
public enum ViewportLinkMode
{
    /// <summary>
    /// No synchronization — viewports are independent.
    /// </summary>
    Independent,

    /// <summary>
    /// The secondary viewport mirrors the primary exactly
    /// (same zoom, same offset).
    /// </summary>
    Mirror,

    /// <summary>
    /// The secondary viewport follows the primary's offset
    /// but maintains its own zoom level.
    /// </summary>
    FollowPan,
}