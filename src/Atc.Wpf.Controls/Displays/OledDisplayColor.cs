namespace Atc.Wpf.Controls.Displays;

/// <summary>
/// Specifies the pixel colour of an OLED display module.
/// </summary>
public enum OledDisplayColor
{
    /// <summary>
    /// White pixels on every row.
    /// </summary>
    White,

    /// <summary>
    /// Blue pixels on every row.
    /// </summary>
    Blue,

    /// <summary>
    /// Yellow pixels on every row.
    /// </summary>
    Yellow,

    /// <summary>
    /// Green pixels on every row.
    /// </summary>
    Green,

    /// <summary>
    /// The two-colour module: yellow pixels on the top 16 rows and blue pixels on the rest.
    /// </summary>
    YellowBlue,
}