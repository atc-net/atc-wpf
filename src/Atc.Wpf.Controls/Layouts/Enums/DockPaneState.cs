namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies where a <see cref="Layouts.DockPane"/> of a <see cref="Layouts.DockPanelPro"/> is shown.
/// </summary>
public enum DockPaneState
{
    /// <summary>
    /// The pane is a tab in its region.
    /// </summary>
    Docked,

    /// <summary>
    /// The pane is shown in its own floating tool window.
    /// </summary>
    Floating,

    /// <summary>
    /// The pane is closed (hidden); it can be restored to its region.
    /// </summary>
    Closed,
}