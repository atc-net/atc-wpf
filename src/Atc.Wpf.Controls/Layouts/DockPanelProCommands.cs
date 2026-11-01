namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// Routed commands for the panes of a <see cref="DockPanelPro"/>. The command parameter is the
/// <see cref="DockPane"/> to act on; without a parameter the command acts on the pane (or the active pane of the
/// region) the command was raised from.
/// </summary>
public static class DockPanelProCommands
{
    /// <summary>
    /// Gets the command that floats a pane into its own tool window.
    /// </summary>
    public static RoutedUICommand FloatPane { get; } = new(
        "Float pane",
        nameof(FloatPane),
        typeof(DockPanelProCommands));

    /// <summary>
    /// Gets the command that docks a floating pane back to its previous region and position.
    /// </summary>
    public static RoutedUICommand RedockPane { get; } = new(
        "Dock pane",
        nameof(RedockPane),
        typeof(DockPanelProCommands));

    /// <summary>
    /// Gets the command that closes a pane (it can be restored with <see cref="DockPanelPro.RestorePane(DockPane)"/>).
    /// </summary>
    public static RoutedUICommand ClosePane { get; } = new(
        "Close pane",
        nameof(ClosePane),
        typeof(DockPanelProCommands));

    /// <summary>
    /// Gets the command that activates the next tab of a region (Ctrl+Tab), wrapping around.
    /// </summary>
    public static RoutedUICommand NextPane { get; } = new(
        "Next pane",
        nameof(NextPane),
        typeof(DockPanelProCommands));

    /// <summary>
    /// Gets the command that activates the previous tab of a region (Ctrl+Shift+Tab), wrapping around.
    /// </summary>
    public static RoutedUICommand PreviousPane { get; } = new(
        "Previous pane",
        nameof(PreviousPane),
        typeof(DockPanelProCommands));
}