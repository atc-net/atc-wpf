namespace Atc.Wpf.Displays;

/// <summary>
/// The byte order of the 16-bit RGB565 pixels in a frame given as bytes.
/// </summary>
public enum Rgb565ByteOrder
{
    /// <summary>
    /// The high byte comes first: the wire order as it is sent over SPI to the panel, and the memory of a 16-bit TFT_eSPI sprite
    /// (<c>getPointer()</c>), which stores every colour byte-swapped so it can be sent as it is.
    /// </summary>
    BigEndian,

    /// <summary>
    /// The low byte comes first: a plain uint16_t array in ESP32 memory, for example an Adafruit_GFX GFXcanvas16 (<c>getBuffer()</c>)
    /// or an LVGL draw buffer without LV_COLOR_16_SWAP.
    /// </summary>
    LittleEndian,
}