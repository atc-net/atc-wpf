namespace Atc.Wpf.Displays;

/// <summary>
/// The display controller chip a <see cref="DisplayMirrorDescribe"/> message names. The value is advisory:
/// the target registered for a display id decides how bytes are interpreted.
/// </summary>
public enum DisplayMirrorChip
{
    /// <summary>
    /// Unknown or not stated.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// HD44780 character controller.
    /// </summary>
    Hd44780 = 0x01,

    /// <summary>
    /// SSD1306 monochrome OLED controller.
    /// </summary>
    Ssd1306 = 0x10,

    /// <summary>
    /// SSD1309 monochrome OLED controller.
    /// </summary>
    Ssd1309 = 0x11,

    /// <summary>
    /// SH1106 monochrome OLED controller.
    /// </summary>
    Sh1106 = 0x12,

    /// <summary>
    /// ST7735 colour TFT controller.
    /// </summary>
    St7735 = 0x20,

    /// <summary>
    /// ST7789 colour TFT controller.
    /// </summary>
    St7789 = 0x21,

    /// <summary>
    /// ILI9341 colour TFT controller.
    /// </summary>
    Ili9341 = 0x22,

    /// <summary>
    /// SSD1331 colour OLED controller.
    /// </summary>
    Ssd1331 = 0x23,

    /// <summary>
    /// SSD1351 colour OLED controller.
    /// </summary>
    Ssd1351 = 0x24,

    /// <summary>
    /// SSD1680 e-paper controller.
    /// </summary>
    Ssd1680 = 0x30,

    /// <summary>
    /// MAX7219 LED matrix driver.
    /// </summary>
    Max7219 = 0x40,

    /// <summary>
    /// ST7920 graphic LCD controller.
    /// </summary>
    St7920 = 0x50,
}