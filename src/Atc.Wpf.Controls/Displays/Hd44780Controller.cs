namespace Atc.Wpf.Controls.Displays;

/// <summary>
/// Emulates the instruction set of an HD44780 controller on a <see cref="CharacterDisplayBuffer"/>: the command and data bytes
/// a microcontroller sends to a display are applied to the buffer, so a desktop app can mirror a real device byte for byte.
/// </summary>
/// <remarks>
/// <para>
/// Every instance owns the address counter, the entry mode and the function set of one controller chip of the buffer.
/// A display with two controller chips (more than two rows and more than 20 columns) needs one instance per chip.
/// </para>
/// <para>
/// The display on/off state and the cursor style are shared in the <see cref="CharacterDisplayBuffer"/>, so with two
/// instances the last display control command written wins.
/// </para>
/// <para>
/// A new instance starts in two-line mode with increment and no display shift, unless the buffer has one row.
/// Unknown instruction bits are ignored and no byte value throws.
/// </para>
/// </remarks>
public sealed class Hd44780Controller
{
    private const int ControllerMax = 2;
    private const byte ClearDisplayCommand = 0x01;
    private const byte ReturnHomeCommand = 0x02;
    private const byte EntryModeCommand = 0x04;
    private const byte DisplayControlCommand = 0x08;
    private const byte ShiftCommand = 0x10;
    private const byte FunctionSetCommand = 0x20;
    private const byte SetCgramCommand = 0x40;
    private const byte SetDdramCommand = 0x80;
    private const byte IncrementBit = 0x02;
    private const byte EntryShiftBit = 0x01;
    private const byte DisplayOnBit = 0x04;
    private const byte UnderlineBit = 0x02;
    private const byte BlinkBit = 0x01;
    private const byte ShiftDisplayBit = 0x08;
    private const byte ShiftRightBit = 0x04;
    private const byte TwoLineBit = 0x08;
    private const int CgramAddressMask = 0x3F;
    private const int DdramAddressMask = 0x7F;
    private const int GlyphRowMask = 0x07;
    private const int GlyphRowBits = 3;
    private const int DdramSize = 80;
    private const int LineLength = 40;
    private const int LastLineAddress = 0x27;
    private const int SecondLineAddress = 0x40;
    private const int LastSecondLineAddress = 0x67;
    private const int LastLinearAddress = 0x4F;

    private readonly Lock sync = new();
    private readonly CharacterDisplayBuffer buffer;
    private readonly int controllerIndex;
    private int address;
    private int cgramAddress;
    private bool isCgramTarget;

    /// <summary>
    /// Initializes a new instance of the <see cref="Hd44780Controller"/> class.
    /// </summary>
    /// <param name="buffer">The buffer the instructions are applied to.</param>
    /// <param name="controllerIndex">The controller chip of the buffer this instance drives, 0 or 1.</param>
    public Hd44780Controller(
        CharacterDisplayBuffer buffer,
        int controllerIndex = 0)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentOutOfRangeException.ThrowIfNegative(controllerIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(controllerIndex, ControllerMax);

        this.buffer = buffer;
        this.controllerIndex = controllerIndex;
        IsTwoLineMode = buffer.Rows > 1;
        IsIncrement = true;
    }

    /// <summary>
    /// Gets a value indicating whether the function set selects two-line mode (the DDRAM is addressed as two lines at 0x00 and 0x40).
    /// </summary>
    public bool IsTwoLineMode { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the entry mode increments the address counter after a write; otherwise it decrements.
    /// </summary>
    public bool IsIncrement { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the entry mode shifts the display on each DDRAM write.
    /// </summary>
    public bool IsDisplayShiftOnEntry { get; private set; }

    /// <summary>
    /// Applies an instruction byte (RS low).
    /// </summary>
    /// <param name="instruction">The instruction.</param>
    public void Command(byte instruction)
        => Write([instruction], isData: false);

    /// <summary>
    /// Applies a data byte (RS high): to the DDRAM, or to the CGRAM after a set CGRAM address instruction.
    /// </summary>
    /// <param name="value">The data byte.</param>
    public void Data(byte value)
        => Write([value], isData: true);

    /// <summary>
    /// Applies a sequence of bytes of the same kind; the buffer raises <see cref="CharacterDisplayBuffer.Changed"/> once for the sequence.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <param name="isData"><see langword="true"/> for data bytes (RS high); <see langword="false"/> for instructions (RS low).</param>
    public void Write(
        ReadOnlySpan<byte> bytes,
        bool isData)
    {
        using var batch = buffer.BeginBatch();

        lock (sync)
        {
            foreach (var value in bytes)
            {
                if (isData)
                {
                    ApplyData(value);
                }
                else
                {
                    ApplyCommand(value);
                }
            }
        }
    }

    private void ApplyCommand(byte instruction)
    {
        if ((instruction & SetDdramCommand) != 0)
        {
            isCgramTarget = false;
            address = instruction & DdramAddressMask;
            UpdateCursor();
        }
        else if ((instruction & SetCgramCommand) != 0)
        {
            isCgramTarget = true;
            cgramAddress = instruction & CgramAddressMask;
        }
        else if ((instruction & FunctionSetCommand) != 0)
        {
            IsTwoLineMode = (instruction & TwoLineBit) != 0;
        }
        else if ((instruction & ShiftCommand) != 0)
        {
            ApplyShift(instruction);
        }
        else if ((instruction & DisplayControlCommand) != 0)
        {
            ApplyDisplayControl(instruction);
        }
        else if ((instruction & EntryModeCommand) != 0)
        {
            IsIncrement = (instruction & IncrementBit) != 0;
            IsDisplayShiftOnEntry = (instruction & EntryShiftBit) != 0;
        }
        else if ((instruction & ReturnHomeCommand) != 0)
        {
            ReturnHome();
        }
        else if ((instruction & ClearDisplayCommand) != 0)
        {
            ClearDisplay();
        }
    }

    private void ApplyData(byte value)
    {
        if (isCgramTarget)
        {
            buffer.SetCgram(cgramAddress >> GlyphRowBits, cgramAddress & GlyphRowMask, value);
            cgramAddress = (cgramAddress + Step()) & CgramAddressMask;
            return;
        }

        buffer.SetDdram(controllerIndex, BufferAddress(address), value);
        address = Move(address, IsIncrement);

        if (IsDisplayShiftOnEntry)
        {
            buffer.ShiftOffset += Step();
        }

        UpdateCursor();
    }

    private void ApplyShift(byte instruction)
    {
        var right = (instruction & ShiftRightBit) != 0;

        if ((instruction & ShiftDisplayBit) != 0)
        {
            // Shifting the display to the left moves the window towards higher addresses.
            buffer.ShiftOffset += right
                ? -1
                : 1;
            return;
        }

        isCgramTarget = false;
        address = Move(address, right);
        UpdateCursor();
    }

    private void ApplyDisplayControl(byte instruction)
    {
        buffer.IsDisplayOn = (instruction & DisplayOnBit) != 0;

        var underline = (instruction & UnderlineBit) != 0;
        var blink = (instruction & BlinkBit) != 0;

        buffer.CursorMode = (underline, blink) switch
        {
            (true, true) => CharacterDisplayCursor.UnderlineAndBlink,
            (true, false) => CharacterDisplayCursor.Underline,
            (false, true) => CharacterDisplayCursor.Blink,
            _ => CharacterDisplayCursor.None,
        };
    }

    private void ReturnHome()
    {
        isCgramTarget = false;
        address = 0;
        buffer.ShiftOffset = 0;
        UpdateCursor();
    }

    private void ClearDisplay()
    {
        for (var i = 0; i < DdramSize; i++)
        {
            var ramAddress = i < LineLength
                ? i
                : SecondLineAddress + (i - LineLength);
            buffer.SetDdram(controllerIndex, ramAddress, (byte)' ');
        }

        IsIncrement = true;
        ReturnHome();
    }

    private int Step()
        => IsIncrement
            ? 1
            : -1;

    private int Move(
        int current,
        bool forward)
    {
        if (IsTwoLineMode)
        {
            if (forward)
            {
                return current switch
                {
                    LastLineAddress => SecondLineAddress,
                    LastSecondLineAddress => 0,
                    _ => (current + 1) & DdramAddressMask,
                };
            }

            return current switch
            {
                SecondLineAddress => LastLineAddress,
                0 => LastSecondLineAddress,
                _ => (current - 1) & DdramAddressMask,
            };
        }

        if (forward)
        {
            return current == LastLinearAddress
                ? 0
                : (current + 1) & DdramAddressMask;
        }

        return current == 0
            ? LastLinearAddress
            : (current - 1) & DdramAddressMask;
    }

    // In one-line mode the 80 bytes of the DDRAM are linear; the buffer lays them out as two lines of 40 bytes.
    private int BufferAddress(int chipAddress)
        => IsTwoLineMode || chipAddress < LineLength
            ? chipAddress
            : (SecondLineAddress + (chipAddress - LineLength)) & DdramAddressMask;

    private void UpdateCursor()
    {
        buffer.CursorController = controllerIndex;
        buffer.CursorAddress = BufferAddress(address);
    }
}