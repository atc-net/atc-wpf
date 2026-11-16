namespace Atc.Wpf.Displays;

/// <summary>
/// A RAM plane of an e-paper controller such as the SSD1680.
/// </summary>
public enum EPaperPlane
{
    /// <summary>
    /// The black/white plane (SSD1680 RAM 0x24): a bit of 0 is black, 1 is white.
    /// </summary>
    Black,

    /// <summary>
    /// The red plane (SSD1680 RAM 0x26): a bit of 1 is red, 0 is not red. On a black/white panel it is not shown.
    /// </summary>
    Red,
}