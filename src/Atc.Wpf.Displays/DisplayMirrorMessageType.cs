namespace Atc.Wpf.Displays;

/// <summary>
/// The message types of the display mirroring wire protocol (version 1).
/// </summary>
public enum DisplayMirrorMessageType
{
    /// <summary>
    /// Not a valid message type (the value 0); never sent.
    /// </summary>
    None = 0,

    /// <summary>
    /// Advisory description of a display: kind, chip, size and name.
    /// </summary>
    Describe = 0x01,

    /// <summary>
    /// A whole monochrome frame as controller pages.
    /// </summary>
    MonoFrame = 0x10,

    /// <summary>
    /// A whole colour frame as big-endian RGB565.
    /// </summary>
    ColorFrame = 0x11,

    /// <summary>
    /// A whole e-paper frame in the native orientation of the panel.
    /// </summary>
    EPaperFrame = 0x12,

    /// <summary>
    /// Command bytes as a chip receives them on a bus with a D/C line.
    /// </summary>
    Commands = 0x20,

    /// <summary>
    /// Data bytes as a chip receives them on a bus with a D/C line.
    /// </summary>
    Data = 0x21,

    /// <summary>
    /// One I2C write transaction: the 7-bit address followed by the bytes.
    /// </summary>
    I2cWrite = 0x22,

    /// <summary>
    /// A hardware reset of the display (empty payload).
    /// </summary>
    Reset = 0x23,

    /// <summary>
    /// One chip-select frame (for example one MAX7219 latch).
    /// </summary>
    Latch = 0x24,
}