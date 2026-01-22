namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies the dock position for a region within a <see cref="Layouts.DockPanelPro"/>.
/// </summary>
public enum DockPosition
{
    /// <summary>
    /// The region is docked to the left side.
    /// </summary>
    Left,

    /// <summary>
    /// The region is docked to the right side.
    /// </summary>
    Right,

    /// <summary>
    /// The region is docked to the top.
    /// </summary>
    Top,

    /// <summary>
    /// The region is docked to the bottom.
    /// </summary>
    Bottom,

    /// <summary>
    /// The region fills the center/remaining space.
    /// </summary>
    Center,
}
