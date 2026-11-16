namespace Atc.Wpf.Displays;

/// <summary>
/// The kind of display a <see cref="DisplayMirrorDescribe"/> message names.
/// </summary>
public enum DisplayMirrorDisplayKind
{
    /// <summary>
    /// Not stated (the value 0).
    /// </summary>
    None = 0,

    /// <summary>
    /// A character LCD or VFD (HD44780 style).
    /// </summary>
    CharacterLcd = 1,

    /// <summary>
    /// A monochrome OLED.
    /// </summary>
    MonoOled = 2,

    /// <summary>
    /// A colour TFT or OLED panel.
    /// </summary>
    ColorPanel = 3,

    /// <summary>
    /// An e-paper panel.
    /// </summary>
    EPaper = 4,

    /// <summary>
    /// An LED dot matrix (for example MAX7219 modules).
    /// </summary>
    LedMatrix = 5,

    /// <summary>
    /// A seven-segment display.
    /// </summary>
    SevenSegment = 6,

    /// <summary>
    /// A monochrome graphic LCD (for example ST7920).
    /// </summary>
    GraphicLcd = 7,
}