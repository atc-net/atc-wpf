namespace Atc.Wpf.Displays;

/// <summary>
/// Specifies the controller chip of a colour TFT display module that <see cref="TftController"/> emulates.
/// </summary>
public enum TftChip
{
    /// <summary>
    /// ST7735: 132 x 162 RAM, the chip of the 1.8 inch 128 x 160, 1.44 inch 128 x 128 and 0.96 inch 80 x 160 modules.
    /// </summary>
    St7735,

    /// <summary>
    /// ST7789: 240 x 320 RAM, the chip of the 240 x 320, 240 x 240 and 1.14 inch 135 x 240 modules.
    /// </summary>
    St7789,

    /// <summary>
    /// ILI9341: 240 x 320 RAM, the chip of the 2.4 inch and 2.8 inch 240 x 320 modules.
    /// </summary>
    Ili9341,
}