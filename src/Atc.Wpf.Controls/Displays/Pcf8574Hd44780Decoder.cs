namespace Atc.Wpf.Controls.Displays;

/// <summary>
/// Decodes the byte stream an ESP32 or Raspberry Pi writes to a PCF8574 I2C "backpack" (for example through the LiquidCrystal_I2C library)
/// back into HD44780 instructions and data, and applies them to an <see cref="Hd44780Controller"/>.
/// </summary>
/// <remarks>
/// <para>
/// Every I2C byte sets the eight expander pins: RS, RW, EN, the backlight and the data nibble D4 to D7 (see <see cref="Pcf8574PinMap"/>).
/// A nibble is latched on the falling edge of EN. Before 4-bit mode is established, each nibble is the high nibble of an 8-bit instruction,
/// as the chip sees the 0x3, 0x3, 0x3, 0x2 initialization; afterwards two nibbles, high first, make one instruction or data byte, selected by RS.
/// A function set with DL = 1 (0x3x) in 4-bit mode returns the decoder to 8-bit handling, like the chip. Read cycles (RW high) are ignored.
/// No byte value throws.
/// </para>
/// <para>
/// The members are locked, but pairing nibbles depends on the byte order, so feed the decoder from one thread.
/// The decoded instructions are applied while the decoder's lock is held, so <see cref="CharacterDisplayBuffer.Changed"/> may be raised
/// under it: a handler must not block on another thread (for example with <c>Dispatcher.Invoke</c>) that also writes to the decoder.
/// </para>
/// </remarks>
public sealed class Pcf8574Hd44780Decoder
{
    private const int NibbleBits = 4;
    private const int BitCount = 8;
    private const byte NibbleMask = 0x0F;
    private const byte FunctionSetEightBitHighNibble = 0x3;
    private const byte FourBitSwitchNibble = 0x2;

    private readonly Lock sync = new();
    private readonly Hd44780Controller controller;
    private readonly int rsMask;
    private readonly int rwMask;
    private readonly int enableMask;
    private readonly int backlightMask;
    private readonly int dataShift;
    private bool previousEnable;
    private bool isFourBitMode;
    private bool hasHighNibble;
    private byte highNibble;
    private bool highNibbleIsData;
    private bool isBacklightOn = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Pcf8574Hd44780Decoder"/> class.
    /// </summary>
    /// <param name="controller">The controller the decoded instructions and data are applied to.</param>
    /// <param name="pinMap">The wiring of the expander; <see langword="null"/> for the common LiquidCrystal_I2C backpack.</param>
    /// <exception cref="ArgumentException">A bit position is outside 0 to 7 or used by two pins.</exception>
    public Pcf8574Hd44780Decoder(
        Hd44780Controller controller,
        Pcf8574PinMap? pinMap = null)
    {
        ArgumentNullException.ThrowIfNull(controller);

        pinMap ??= new Pcf8574PinMap();
        ValidatePinMap(pinMap);

        this.controller = controller;
        rsMask = 1 << pinMap.Rs;
        rwMask = 1 << pinMap.Rw;
        enableMask = 1 << pinMap.Enable;
        backlightMask = 1 << pinMap.Backlight;
        dataShift = pinMap.Data4;
    }

    /// <summary>
    /// Occurs when the backlight bit of the written bytes changes.
    /// </summary>
    public event EventHandler? BacklightChanged;

    /// <summary>
    /// Gets a value indicating whether the backlight is on, taken from the latest written byte; <see langword="true"/> until a byte says otherwise.
    /// </summary>
    /// <remarks>Read without the decoder's lock, so it can be read from a <see cref="CharacterDisplayBuffer.Changed"/> handler or the UI thread at any time.</remarks>
    public bool IsBacklightOn
        => Volatile.Read(ref isBacklightOn);

    /// <summary>
    /// Decodes one byte written to the expander.
    /// </summary>
    /// <param name="value">The byte.</param>
    public void Write(byte value)
        => Write([value]);

    /// <summary>
    /// Decodes a sequence of bytes written to the expander.
    /// </summary>
    /// <param name="values">The bytes.</param>
    public void Write(ReadOnlySpan<byte> values)
    {
        var backlightChanged = false;

        lock (sync)
        {
            foreach (var value in values)
            {
                var backlight = (value & backlightMask) != 0;
                if (backlight != isBacklightOn)
                {
                    Volatile.Write(ref isBacklightOn, backlight);
                    backlightChanged = true;
                }

                var enable = (value & enableMask) != 0;
                var fallingEdge = previousEnable && !enable;
                previousEnable = enable;

                if (fallingEdge && (value & rwMask) == 0)
                {
                    var nibble = (byte)((value >> dataShift) & NibbleMask);
                    Latch(nibble, (value & rsMask) != 0);
                }
            }
        }

        if (backlightChanged)
        {
            BacklightChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private static void ValidatePinMap(Pcf8574PinMap pinMap)
    {
        var used = 0;
        var positions = new[]
        {
            pinMap.Rs,
            pinMap.Rw,
            pinMap.Enable,
            pinMap.Backlight,
            pinMap.Data4,
            pinMap.Data4 + 1,
            pinMap.Data4 + 2,
            pinMap.Data4 + 3,
        };

        foreach (var position in positions)
        {
            if (position is < 0 or >= BitCount)
            {
                throw new ArgumentException($"The bit position {position} is outside 0 to {BitCount - 1}.", nameof(pinMap));
            }

            var bit = 1 << position;
            if ((used & bit) != 0)
            {
                throw new ArgumentException($"The bit position {position} is used by more than one pin.", nameof(pinMap));
            }

            used |= bit;
        }
    }

    private void Latch(
        byte nibble,
        bool rs)
    {
        if (!isFourBitMode)
        {
            // The chip is still in 8-bit mode: the nibble is the high half of an instruction, the low half is wired to ground.
            if (rs)
            {
                return;
            }

            controller.Command((byte)(nibble << NibbleBits));
            if (nibble == FourBitSwitchNibble)
            {
                isFourBitMode = true;
                hasHighNibble = false;
            }

            return;
        }

        if (!hasHighNibble)
        {
            highNibble = nibble;
            highNibbleIsData = rs;
            hasHighNibble = true;
            return;
        }

        hasHighNibble = false;
        var combined = (byte)((highNibble << NibbleBits) | nibble);
        if (highNibbleIsData)
        {
            controller.Data(combined);
            return;
        }

        controller.Command(combined);
        if (highNibble == FunctionSetEightBitHighNibble)
        {
            isFourBitMode = false;
        }
    }
}