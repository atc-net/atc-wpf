namespace Atc.Wpf.Network.Vnc;

/// <summary>
/// How the remote screen is scaled in the <see cref="VncViewerView"/>.
/// </summary>
public enum VncScaleMode
{
    /// <summary>
    /// The whole screen fits in the view, keeping its proportions.
    /// </summary>
    Fit,

    /// <summary>
    /// The screen fills the view, keeping its proportions; the edges that don't fit are cut off.
    /// </summary>
    Fill,

    /// <summary>
    /// One remote pixel per screen pixel, with scroll bars when the screen is larger than the view.
    /// </summary>
    Original,
}