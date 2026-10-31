namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// Provides data for the <see cref="DockPanelPro.PaneStateChanged"/> event.
/// </summary>
public sealed class DockPaneStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockPaneStateChangedEventArgs"/> class.
    /// </summary>
    /// <param name="pane">The pane whose state changed.</param>
    /// <param name="oldState">The state before the change.</param>
    /// <param name="newState">The state after the change.</param>
    public DockPaneStateChangedEventArgs(
        DockPane pane,
        DockPaneState oldState,
        DockPaneState newState)
    {
        Pane = pane;
        OldState = oldState;
        NewState = newState;
    }

    /// <summary>
    /// Gets the pane whose state changed.
    /// </summary>
    public DockPane Pane { get; }

    /// <summary>
    /// Gets the state before the change.
    /// </summary>
    public DockPaneState OldState { get; }

    /// <summary>
    /// Gets the state after the change.
    /// </summary>
    public DockPaneState NewState { get; }
}