namespace Atc.Wpf.Controls.Displays;

/// <summary>
/// Specifies the character ROM of an HD44780-compatible character display.
/// </summary>
public enum CharacterRom
{
    /// <summary>
    /// ROM code A00: ASCII, Japanese half-width katakana and a few Greek and math symbols (0x5C is a yen sign, 0x7E and 0x7F are arrows).
    /// </summary>
    A00,

    /// <summary>
    /// ROM code A02: ASCII, Cyrillic, Greek and the Western European (Latin-1) letters and symbols.
    /// </summary>
    A02,
}