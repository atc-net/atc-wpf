namespace Atc.Wpf.Displays;

/// <summary>
/// Decodes the bytes an ESP32 or Raspberry Pi sends to an ST7920 module over its serial (SPI) interface into the instruction and
/// data feed of a <see cref="St7920Controller"/>.
/// </summary>
/// <remarks>
/// <para>
/// A start byte <c>11111 RW RS 0</c> opens a transfer: <c>0xF8</c> for instructions (RS = 0), <c>0xFA</c> for data (RS = 1). Every
/// instruction or data byte follows as two bytes, the high nibble in the upper half of the first and the low nibble in the upper half
/// of the second. One start byte may be followed by many pairs (as U8g2 sends a data run) or be repeated before every pair (as ESPHome
/// does); the mode lasts until the next start byte (⚠️ derived from both libraries).
/// </para>
/// <para>
/// The decoder resynchronises on every start byte: payload bytes always have a zero low nibble, so any byte matching
/// <c>0b11111xx0</c> is a start byte, and it drops a pending half pair. Bytes before the first start byte are dropped, and so are the
/// bytes after a read-form start byte (RW = 1) up to the next start byte. A payload byte with a non-zero low nibble is masked to its
/// upper half (⚠️ unverified). The framing state carries over from one <see cref="Write"/> to the next; <see cref="ResetFraming"/>
/// forgets it (for example when the host resets the module).
/// </para>
/// <para>
/// Consecutive bytes of the same mode are passed on in one call, and one <see cref="Write"/> raises
/// <see cref="MonochromeFrameBuffer.Changed"/> at most once. The decoder adds no locking: feed it from one thread.
/// </para>
/// </remarks>
public sealed class St7920SerialDecoder
{
    private const byte StartMask = 0xF9;
    private const byte StartPattern = 0xF8;
    private const byte ReadBit = 0x04;
    private const byte DataBit = 0x02;
    private const byte NibbleMask = 0xF0;

    private readonly St7920Controller controller;
    private bool hasStart;
    private bool isRead;
    private bool isData;
    private bool hasHighNibble;
    private byte highNibble;

    /// <summary>
    /// Initializes a new instance of the <see cref="St7920SerialDecoder"/> class.
    /// </summary>
    /// <param name="controller">The controller the decoded instructions and data are applied to.</param>
    /// <exception cref="ArgumentNullException"><paramref name="controller"/> is <see langword="null"/>.</exception>
    public St7920SerialDecoder(St7920Controller controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        this.controller = controller;
    }

    /// <summary>Gets the controller the decoded instructions and data are applied to.</summary>
    internal St7920Controller Controller => controller;

    /// <summary>
    /// Decodes bytes of the serial stream, in the order they were sent.
    /// </summary>
    /// <param name="bytes">The bytes; a transfer may be split over several calls.</param>
    public void Write(ReadOnlySpan<byte> bytes)
    {
        if (bytes.IsEmpty)
        {
            return;
        }

        var rented = System.Buffers.ArrayPool<byte>.Shared.Rent((bytes.Length / 2) + 1);
        try
        {
            using var batch = controller.BeginBatch();

            var runLength = 0;
            var runIsData = false;

            foreach (var value in bytes)
            {
                // ref: §ST7920.6: a start byte has bit 3 set, which a payload byte never has.
                if ((value & StartMask) == StartPattern)
                {
                    hasStart = true;
                    isRead = (value & ReadBit) != 0;
                    isData = (value & DataBit) != 0;
                    hasHighNibble = false;
                    continue;
                }

                if (!hasStart || isRead)
                {
                    continue;
                }

                // ⚠️ unverified (§ST7920.6): a non-zero low nibble is masked off.
                var nibble = (byte)(value & NibbleMask);
                if (!hasHighNibble)
                {
                    highNibble = nibble;
                    hasHighNibble = true;
                    continue;
                }

                hasHighNibble = false;
                if (runLength > 0 && runIsData != isData)
                {
                    Flush(rented.AsSpan(0, runLength), runIsData);
                    runLength = 0;
                }

                runIsData = isData;
                rented[runLength++] = (byte)(highNibble | (nibble >> 4));
            }

            if (runLength > 0)
            {
                Flush(rented.AsSpan(0, runLength), runIsData);
            }
        }
        finally
        {
            System.Buffers.ArrayPool<byte>.Shared.Return(rented);
        }
    }

    /// <summary>
    /// Forgets the framing state: the next bytes are dropped until a start byte arrives.
    /// </summary>
    public void ResetFraming()
    {
        hasStart = false;
        isRead = false;
        isData = false;
        hasHighNibble = false;
        highNibble = 0;
    }

    private void Flush(
        ReadOnlySpan<byte> run,
        bool isDataRun)
    {
        if (isDataRun)
        {
            controller.WriteData(run);
        }
        else
        {
            controller.WriteCommands(run);
        }
    }
}