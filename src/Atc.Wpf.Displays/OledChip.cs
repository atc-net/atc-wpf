namespace Atc.Wpf.Displays;

/// <summary>
/// Specifies the controller chip of an OLED display module.
/// </summary>
public enum OledChip
{
    /// <summary>
    /// SSD1306: 128 x 64 RAM, 8 pages of 128 columns, the chip of the common 0.96 inch and 0.91 inch modules.
    /// </summary>
    Ssd1306,

    /// <summary>
    /// SSD1309: a SSD1306-compatible command set for larger 128 x 64 panels.
    /// </summary>
    Ssd1309,

    /// <summary>
    /// SH1106: 132-column RAM, a 128-wide panel shows columns 2 to 129; it has no horizontal or vertical addressing mode, only page addressing.
    /// </summary>
    Sh1106,
}