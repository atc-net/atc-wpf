namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

/// <summary>
/// How a graphic display sample feeds its display.
/// </summary>
public enum GraphicDisplayFeedMode
{
    /// <summary>
    /// The control's own buffer shows the <c>Text</c> property.
    /// </summary>
    Text,

    /// <summary>
    /// Whole frames are streamed into the buffer with <c>LoadFrame</c>, as a device that sends its frame buffer.
    /// </summary>
    Frame,

    /// <summary>
    /// A captured byte stream (I2C or SPI) is replayed into the chip emulator.
    /// </summary>
    Replay,
}