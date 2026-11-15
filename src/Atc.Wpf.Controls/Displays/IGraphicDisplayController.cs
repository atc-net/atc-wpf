namespace Atc.Wpf.Controls.Displays;

/// <summary>
/// The feed of a graphic display chip emulator: the bytes a host (an ESP32 or a Raspberry Pi) sends to the chip.
/// </summary>
/// <remarks>
/// The members map to the D/C (data/command) line of the chip: a command is a byte sent with D/C low,
/// data is one or more bytes sent with D/C high.
/// Disposing a controller detaches it from its buffer (the buffer keeps the last picture), stops any timer it runs and makes it inert:
/// every later write, reset or setting is ignored and draws nothing.
/// </remarks>
public interface IGraphicDisplayController : IDisposable
{
    /// <summary>
    /// Writes one command byte (D/C low) to the chip.
    /// </summary>
    /// <param name="command">The command byte, or an argument byte of the command before it.</param>
    void WriteCommand(byte command);

    /// <summary>
    /// Writes several command bytes (D/C low) to the chip; equivalent to calling <see cref="WriteCommand"/> for each byte.
    /// </summary>
    /// <param name="commands">The command bytes.</param>
    void WriteCommands(ReadOnlySpan<byte> commands);

    /// <summary>
    /// Writes data bytes (D/C high) to the chip's display RAM.
    /// </summary>
    /// <param name="data">The data bytes.</param>
    void WriteData(ReadOnlySpan<byte> data);

    /// <summary>
    /// Resets the chip to its power-on state.
    /// </summary>
    void Reset();
}