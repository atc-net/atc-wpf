namespace Atc.Wpf.Controls.Layouts.Internal;

/// <summary>
/// Represents the persisted layout state of a <see cref="DockPanelPro"/>.
/// </summary>
public sealed class DockPanelLayout
{
    /// <summary>
    /// The layout version written by <see cref="DockPanelPro.SaveLayout"/>: 2 adds pane groups, active tabs,
    /// closed and floating panes.
    /// </summary>
    public const int CurrentVersion = 2;

    /// <summary>
    /// Gets or sets the layout format version. A layout without a version is version 1 (region sizes only).
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Gets or sets the layout identifier.
    /// </summary>
    public string? LayoutId { get; set; }

    /// <summary>
    /// Gets or sets the collection of region states.
    /// </summary>
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Used for JSON serialization")]
    [SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Used for JSON serialization")]
    public List<DockRegionState> Regions { get; set; } = [];

    /// <summary>
    /// Gets or sets the states of the panes that have a <see cref="DockPane.PaneId"/> (version 2).
    /// </summary>
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Used for JSON serialization")]
    [SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Used for JSON serialization")]
    public List<DockPaneLayoutState>? Panes { get; set; }
}