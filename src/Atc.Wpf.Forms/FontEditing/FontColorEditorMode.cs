namespace Atc.Wpf.Forms.FontEditing;

/// <summary>
/// Selects which UI <see cref="AdvancedFontPicker"/> uses to edit
/// the foreground/background brushes.
/// </summary>
public enum FontColorEditorMode
{
    /// <summary>
    /// Use a dropdown of well-known named colors. Renders inline,
    /// which is the right default when the picker itself is hosted
    /// inside a dialog (avoids opening a nested dialog box).
    /// </summary>
    WellKnownColorSelector = 0,

    /// <summary>
    /// Use the labeled color picker, which opens a dedicated
    /// color-picker dialog when the user edits the value.
    /// </summary>
    ColorPicker = 1,
}