namespace Atc.Wpf.Displays;

/// <summary>
/// Specifies how the cursor of a character display is shown.
/// </summary>
public enum CharacterDisplayCursor
{
    /// <summary>
    /// No cursor is shown.
    /// </summary>
    None,

    /// <summary>
    /// An underline on the cursor row (dot row 7, the bottom row) of the character cell.
    /// </summary>
    Underline,

    /// <summary>
    /// The whole character cell blinks.
    /// </summary>
    Blink,

    /// <summary>
    /// Both the underline and the blinking cell are shown.
    /// </summary>
    UnderlineAndBlink,
}