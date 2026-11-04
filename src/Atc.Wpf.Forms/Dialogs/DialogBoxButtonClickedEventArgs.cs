namespace Atc.Wpf.Forms.Dialogs;

/// <summary>
/// Provides the button clicked in a <see cref="DialogBoxButtonBar"/>.
/// </summary>
public sealed class DialogBoxButtonClickedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogBoxButtonClickedEventArgs"/> class.
    /// </summary>
    /// <param name="button">The clicked button.</param>
    public DialogBoxButtonClickedEventArgs(DialogBoxButton button)
        => Button = button;

    /// <summary>
    /// Gets the clicked button.
    /// </summary>
    public DialogBoxButton Button { get; }
}