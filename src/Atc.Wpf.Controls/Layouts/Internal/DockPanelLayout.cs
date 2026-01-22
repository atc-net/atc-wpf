namespace Atc.Wpf.Controls.Layouts.Internal;

/// <summary>
/// Represents the persisted layout state of a <see cref="DockPanelPro"/>.
/// </summary>
public sealed class DockPanelLayout
{
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
}