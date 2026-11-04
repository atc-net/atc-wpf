namespace Atc.Wpf.Forms.Dialogs;

/// <summary>
/// A button of a dialog box.
/// </summary>
public enum DialogBoxButton
{
    /// <summary>
    /// The affirmative button, such as OK or Yes.
    /// </summary>
    Affirmative,

    /// <summary>
    /// The negative button, such as Cancel in an OK/Cancel dialog or No in a Yes/No dialog.
    /// </summary>
    Negative,

    /// <summary>
    /// The Cancel button of a Yes/No/Cancel dialog.
    /// </summary>
    Cancel,
}