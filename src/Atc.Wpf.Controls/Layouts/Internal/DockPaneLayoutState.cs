namespace Atc.Wpf.Controls.Layouts.Internal;

/// <summary>
/// Represents the persisted state of a single <see cref="DockPane"/>.
/// </summary>
public sealed class DockPaneLayoutState
{
    /// <summary>
    /// Gets or sets the pane identifier.
    /// </summary>
    public string? PaneId { get; set; }

    /// <summary>
    /// Gets or sets whether the pane is docked, floating or closed.
    /// </summary>
    public DockPaneState State { get; set; }

    /// <summary>
    /// Gets or sets the left edge of the floating window, in device-independent units.
    /// </summary>
    public double FloatingLeft { get; set; }

    /// <summary>
    /// Gets or sets the top edge of the floating window, in device-independent units.
    /// </summary>
    public double FloatingTop { get; set; }

    /// <summary>
    /// Gets or sets the width of the floating window; 0 when the pane has never floated.
    /// </summary>
    public double FloatingWidth { get; set; }

    /// <summary>
    /// Gets or sets the height of the floating window; 0 when the pane has never floated.
    /// </summary>
    public double FloatingHeight { get; set; }
}