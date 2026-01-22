namespace Atc.Wpf.Controls.Layouts.Internal;

/// <summary>
/// Represents the persisted state of a single <see cref="DockRegion"/>.
/// </summary>
public sealed class DockRegionState
{
    /// <summary>
    /// Gets or sets the unique identifier for the region.
    /// </summary>
    public string? RegionId { get; set; }

    /// <summary>
    /// Gets or sets the dock position of the region.
    /// </summary>
    public DockPosition Dock { get; set; }

    /// <summary>
    /// Gets or sets the width of the region.
    /// </summary>
    public double Width { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the height of the region.
    /// </summary>
    public double Height { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets a value indicating whether the region is expanded.
    /// </summary>
    public bool IsExpanded { get; set; } = true;
}