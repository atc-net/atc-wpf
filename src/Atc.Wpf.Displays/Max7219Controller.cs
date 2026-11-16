namespace Atc.Wpf.Displays;

/// <summary>
/// Emulates a daisy chain of MAX7219 LED display drivers on a <see cref="Max7219ChainBuffer"/>: the bytes a microcontroller shifts out
/// (as LedControl, MD_MAX72XX and U8g2 do) are applied to the registers of the devices in the chain.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="WriteData"/> takes one latched frame, the bytes between chip select going low and going high. Each 16-bit word is two bytes,
/// the address byte first (only its low nibble counts) and the data byte second. The chain is one shift register (§MAX7219.6): the
/// first word shifted out ends up in the farthest device, the last word in device 0. A frame of <c>2 * N</c> bytes sets every device;
/// when the latch goes high every device executes the word it holds: address 0 is a no-op, 1 to 8 the digits 0 to 7, 9 the decode mode,
/// 10 the intensity (low 4 bits), 11 the scan limit (low 3 bits), 12 the shutdown register (bit 0: 0 = shutdown), 15 the display test
/// (bit 0). Any other address is ignored and written to <see cref="Trace"/> once per value.
/// </para>
/// <para>
/// ⚠️ A frame shorter or longer than the chain is derived from the shift-register description, not stated by the datasheet: a frame of
/// <c>k</c> words (k less than N) sets the nearest k devices, and the other devices execute the words that the nearest N - k devices held
/// before the frame (normally a repeat of the previous frame's writes); a frame of more than N words loses its first words, which fall
/// out of the end of the chain. The words held between frames are kept for this. An odd trailing byte is ignored and written to
/// <see cref="Trace"/> once.
/// </para>
/// <para>
/// A new instance and <see cref="Reset"/> apply the power-on state to the buffer: every device is shut down with blank digits, decode off,
/// intensity 0, scan limit 0 and display test off (§MAX7219.2; the digit content is ⚠️ not stated and is cleared). The chip has no
/// command line, so <see cref="WriteCommand"/> and <see cref="WriteCommands"/> send nothing the chip understands: each distinct byte is
/// ignored and written to <see cref="Trace"/> once. When the buffer is resized the words held are kept for the devices that remain.
/// </para>
/// <para>
/// Feed the controller from one thread at a time. The calls are serialized by an internal lock; the buffer raises
/// <see cref="Max7219ChainBuffer.Changed"/> once per frame, after that lock is released. A disposed controller ignores everything sent
/// to it.
/// </para>
/// </remarks>
public sealed class Max7219Controller : IGraphicDisplayController
{
    private const int WordBytes = 2;
    private const int AddressMask = 0x0F;

    private readonly Lock sync = new();
    private readonly Max7219ChainBuffer buffer;
    private readonly bool[] tracedCommands = new bool[byte.MaxValue + 1];
    private readonly bool[] tracedAddresses = new bool[AddressMask + 1];
    private ushort[] words;
    private bool isOddByteTraced;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Max7219Controller"/> class and puts the chain of the buffer in the power-on state.
    /// </summary>
    /// <param name="buffer">The buffer that holds the registers of the chain.</param>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
    public Max7219Controller(Max7219ChainBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        this.buffer = buffer;
        words = new ushort[buffer.DeviceCount];

        using (buffer.BeginBatch())
        {
            lock (sync)
            {
                buffer.ResetToPowerOn();
            }
        }
    }

    /// <inheritdoc />
    /// <remarks>The chip has no command line: the byte is ignored and written to <see cref="Trace"/> once per distinct value.</remarks>
    public void WriteCommand(byte command)
    {
        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            TraceCommandOnce(command);
        }
    }

    /// <inheritdoc />
    /// <remarks>The chip has no command line: each byte is ignored and written to <see cref="Trace"/> once per distinct value.</remarks>
    public void WriteCommands(ReadOnlySpan<byte> commands)
    {
        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            foreach (var command in commands)
            {
                TraceCommandOnce(command);
            }
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// The bytes are one latched frame: pairs of address and data bytes, the first pair for the farthest device. An empty frame does
    /// nothing; an odd trailing byte is ignored.
    /// </remarks>
    public void WriteData(ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty)
        {
            return;
        }

        using var batch = buffer.BeginBatch();

        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            EnsureWords();

            var count = data.Length / WordBytes;
            if ((data.Length % WordBytes) != 0)
            {
                TraceOddByteOnce();
            }

            // ref: §MAX7219.6: every word shifts the chain by one device, so device j holds the word j places from the last one.
            for (var i = 0; i < count; i++)
            {
                Array.Copy(words, 0, words, 1, words.Length - 1);
                words[0] = (ushort)((data[i * WordBytes] << 8) | data[(i * WordBytes) + 1]);
            }

            // The latch: every device executes the word it holds, also a stale one.
            for (var device = 0; device < words.Length; device++)
            {
                Execute(device, words[device]);
            }
        }
    }

    /// <inheritdoc />
    /// <remarks>The power-on reset: every device is shut down with blank digits and the words held in the chain are cleared.</remarks>
    public void Reset()
    {
        using var batch = buffer.BeginBatch();

        lock (sync)
        {
            if (isDisposed)
            {
                return;
            }

            EnsureWords();
            Array.Clear(words);
            buffer.ResetToPowerOn();
        }
    }

    /// <summary>
    /// Makes the controller inert: the buffer keeps its registers, and every later write or reset is ignored.
    /// </summary>
    public void Dispose()
    {
        lock (sync)
        {
            isDisposed = true;
        }
    }

    // Must be called with the lock held. Maps the held words onto a resized chain: the devices that remain keep their word.
    private void EnsureWords()
    {
        var count = buffer.DeviceCount;
        if (words.Length == count)
        {
            return;
        }

        var resized = new ushort[count];
        Array.Copy(words, resized, System.Math.Min(words.Length, count));
        words = resized;
    }

    // ref: §MAX7219.1, §MAX7219.9 item 2.
    private void Execute(
        int device,
        ushort word)
    {
        var address = (word >> 8) & AddressMask;
        var value = (byte)(word & 0xFF);
        switch (address)
        {
            case 0x0:
                break;
            case >= 0x1 and <= 0x8:
                buffer.SetDigit(device, address - 1, value);
                break;
            case 0x9:
                buffer.SetDecodeMode(device, value);
                break;
            case 0xA:
                buffer.SetIntensity(device, (byte)(value & 0x0F));
                break;
            case 0xB:
                buffer.SetScanLimit(device, (byte)(value & 0x07));
                break;
            case 0xC:
                buffer.SetShutdown(device, (value & 0x01) == 0);
                break;
            case 0xF:
                buffer.SetDisplayTest(device, (value & 0x01) != 0);
                break;
            default:
                // ref: §MAX7219.1: 0xD and 0xE are not registers (⚠️ unverified: ignored).
                TraceAddressOnce(address);
                break;
        }
    }

    private void TraceCommandOnce(byte value)
    {
        if (tracedCommands[value])
        {
            return;
        }

        tracedCommands[value] = true;
        Trace.TraceWarning(
            string.Create(
                CultureInfo.InvariantCulture,
                $"{nameof(Max7219Controller)}: command byte 0x{value:X2} is ignored; the chip has no command line, send frames with WriteData."));
    }

    private void TraceAddressOnce(int address)
    {
        if (tracedAddresses[address])
        {
            return;
        }

        tracedAddresses[address] = true;
        Trace.TraceWarning(
            string.Create(
                CultureInfo.InvariantCulture,
                $"{nameof(Max7219Controller)}: register address 0x{address:X} is not a register of the chip and is ignored."));
    }

    private void TraceOddByteOnce()
    {
        if (isOddByteTraced)
        {
            return;
        }

        isOddByteTraced = true;
        Trace.TraceWarning($"{nameof(Max7219Controller)}: a frame has an odd number of bytes; the trailing byte is ignored.");
    }
}