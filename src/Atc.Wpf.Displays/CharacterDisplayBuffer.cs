namespace Atc.Wpf.Displays;

/// <summary>
/// The thread-safe model of the memory of an HD44780 character display: the display data RAM (DDRAM)
/// of one or two controller chips, the custom glyph RAM (CGRAM) and the display state (shift, cursor, display on/off).
/// </summary>
/// <remarks>
/// A DDRAM is 80 bytes per controller, laid out as two lines of 40 bytes at the addresses 0x00 and 0x40.
/// An address is 0x00 to 0x7F, like the chip's 7-bit address counter, and wraps at 0x28 within its line;
/// a member given an address outside 0x00 to 0x7F throws. All members are thread-safe; <see cref="Changed"/> is raised on the thread that made the change.
/// </remarks>
public sealed class CharacterDisplayBuffer
{
    private const int MinColumns = 1;
    private const int MaxColumns = 40;
    private const int MinRows = 1;
    private const int MaxRows = 4;
    private const int LineLength = 40;
    private const int DdramSize = 80;
    private const int ControllerMax = 2;
    private const int SecondLineAddress = 0x40;
    private const int LineOffsetMask = 0x3F;
    private const int MaxAddress = 0x7F;
    private const int GlyphCount = 8;
    private const int GlyphRows = 8;
    private const byte GlyphBitsMask = 0x1F;
    private const byte BlankCode = 0x20;

    private readonly Lock sync = new();
    private readonly byte[][] ddram = [new byte[DdramSize], new byte[DdramSize]];
    private readonly byte[] cgram = new byte[GlyphCount * GlyphRows];
    private int columns;
    private int rows;
    private int shiftOffset;
    private bool isDisplayOn = true;
    private CharacterDisplayCursor cursorMode;
    private int cursorAddress;
    private int cursorController;
    private int batchDepth;
    private bool changedPending;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterDisplayBuffer"/> class with blank DDRAM.
    /// </summary>
    /// <param name="columns">The number of columns, 1 to 40.</param>
    /// <param name="rows">The number of rows, 1 to 4.</param>
    public CharacterDisplayBuffer(
        int columns,
        int rows)
    {
        ValidateSize(columns, rows);

        this.columns = columns;
        this.rows = rows;

        foreach (var controllerRam in ddram)
        {
            Array.Fill(controllerRam, BlankCode);
        }
    }

    /// <summary>
    /// Occurs when the content or the state of the buffer has changed; once per batch, or once per change outside a batch.
    /// </summary>
    public event EventHandler? Changed;

    /// <summary>
    /// Gets the number of columns.
    /// </summary>
    public int Columns
    {
        get
        {
            lock (sync)
            {
                return columns;
            }
        }
    }

    /// <summary>
    /// Gets the number of rows.
    /// </summary>
    public int Rows
    {
        get
        {
            lock (sync)
            {
                return rows;
            }
        }
    }

    /// <summary>
    /// Gets the number of controller chips: 2 for a display of more than two rows wider than 20 columns, otherwise 1.
    /// </summary>
    public int ControllerCount
    {
        get
        {
            lock (sync)
            {
                return ControllerCountOf(columns, rows);
            }
        }
    }

    /// <summary>
    /// Gets or sets the display shift, in columns; a positive value moves the window towards higher DDRAM addresses.
    /// </summary>
    public int ShiftOffset
    {
        get
        {
            lock (sync)
            {
                return shiftOffset;
            }
        }

        set
        {
            lock (sync)
            {
                if (shiftOffset == value)
                {
                    return;
                }

                shiftOffset = value;
            }

            NotifyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the display is on.
    /// </summary>
    public bool IsDisplayOn
    {
        get
        {
            lock (sync)
            {
                return isDisplayOn;
            }
        }

        set
        {
            lock (sync)
            {
                if (isDisplayOn == value)
                {
                    return;
                }

                isDisplayOn = value;
            }

            NotifyChanged();
        }
    }

    /// <summary>
    /// Gets or sets how the cursor is shown.
    /// </summary>
    public CharacterDisplayCursor CursorMode
    {
        get
        {
            lock (sync)
            {
                return cursorMode;
            }
        }

        set
        {
            lock (sync)
            {
                if (cursorMode == value)
                {
                    return;
                }

                cursorMode = value;
            }

            NotifyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the DDRAM address of the cursor (the address counter), 0x00 to 0x7F.
    /// </summary>
    /// <remarks>An address wraps at 0x28 within its line, so 0x28 to 0x3F show as 0x00 to 0x17 and 0x68 to 0x7F as 0x40 to 0x57.</remarks>
    /// <exception cref="ArgumentOutOfRangeException">The address is outside 0x00 to 0x7F.</exception>
    public int CursorAddress
    {
        get
        {
            lock (sync)
            {
                return cursorAddress;
            }
        }

        set
        {
            ValidateAddress(value);

            lock (sync)
            {
                if (cursorAddress == value)
                {
                    return;
                }

                cursorAddress = value;
            }

            NotifyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the controller chip (0 or 1) the cursor is on.
    /// </summary>
    public int CursorController
    {
        get
        {
            lock (sync)
            {
                return cursorController;
            }
        }

        set
        {
            ValidateController(value);

            lock (sync)
            {
                if (cursorController == value)
                {
                    return;
                }

                cursorController = value;
            }

            NotifyChanged();
        }
    }

    /// <summary>
    /// Gets the DDRAM address of a visible cell in the module line layout (the display shift is not applied).
    /// </summary>
    /// <param name="row">The row, 0 to <paramref name="rows"/> - 1.</param>
    /// <param name="column">The column, 0 to <paramref name="columns"/> - 1.</param>
    /// <param name="columns">The number of columns of the display, 1 to 40.</param>
    /// <param name="rows">The number of rows of the display, 1 to 4.</param>
    /// <param name="controller">The controller chip (0 or 1) the cell is on.</param>
    /// <returns>The DDRAM address, 0x00 to 0x67.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The size is not a valid module size, or the cell is outside it.</exception>
    public static int AddressOf(
        int row,
        int column,
        int columns,
        int rows,
        out int controller)
    {
        ValidateSize(columns, rows);
        ArgumentOutOfRangeException.ThrowIfNegative(row);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(row, rows);
        ArgumentOutOfRangeException.ThrowIfNegative(column);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(column, columns);

        return CellAddress(row, column, columns, rows, out controller);
    }

    /// <summary>
    /// Changes the size of the display; the DDRAM content is kept.
    /// </summary>
    /// <param name="columns">The number of columns, 1 to 40.</param>
    /// <param name="rows">The number of rows, 1 to 4.</param>
    public void Resize(
        int columns,
        int rows)
    {
        ValidateSize(columns, rows);

        lock (sync)
        {
            this.columns = columns;
            this.rows = rows;
        }

        NotifyChanged();
    }

    /// <summary>
    /// Starts a batch: <see cref="Changed"/> is raised once, if anything changed, when the last open batch is disposed.
    /// </summary>
    /// <returns>The scope to dispose to end the batch.</returns>
    public IDisposable BeginBatch()
    {
        lock (sync)
        {
            batchDepth++;
        }

        return new BatchScope(this);
    }

    /// <summary>
    /// Gets the character code of a visible cell, with the display shift applied.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    /// <returns>The character code.</returns>
    public byte GetCode(
        int row,
        int column)
    {
        lock (sync)
        {
            ValidateCell(row, column);
            var (controller, address) = VisibleAddress(row, column);
            return ddram[controller][IndexOf(address)];
        }
    }

    /// <summary>
    /// Writes text into the visible cells of a row, with the display shift applied; text past the last column is dropped.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column the first character goes to.</param>
    /// <param name="text">The text.</param>
    /// <param name="rom">The character ROM that maps the characters to codes.</param>
    public void WriteVisible(
        int row,
        int column,
        string text,
        CharacterRom rom)
    {
        ArgumentNullException.ThrowIfNull(text);

        lock (sync)
        {
            ValidateCell(row, column);

            for (var i = 0; i < text.Length && column + i < columns; i++)
            {
                var (controller, address) = VisibleAddress(row, column + i);
                ddram[controller][IndexOf(address)] = CharacterRomTables.Map(rom, text[i]);
            }
        }

        NotifyChanged();
    }

    /// <summary>
    /// Clears the DDRAM to spaces and resets the display shift and the cursor address, as the chip's clear command does;
    /// the cursor returns to address 0x00 of controller 0.
    /// </summary>
    public void Clear()
    {
        lock (sync)
        {
            foreach (var controllerRam in ddram)
            {
                Array.Fill(controllerRam, BlankCode);
            }

            shiftOffset = 0;
            cursorAddress = 0;
            cursorController = 0;
        }

        NotifyChanged();
    }

    /// <summary>
    /// Gets the visible text, with the display shift applied.
    /// </summary>
    /// <param name="rom">The character ROM that maps the codes back to characters.</param>
    /// <returns>The rows joined by a new line; the custom glyph codes are <c>\u0000</c> to <c>\u0007</c>.</returns>
    public string GetVisibleText(CharacterRom rom)
    {
        lock (sync)
        {
            var builder = new StringBuilder((columns + 1) * rows);
            for (var row = 0; row < rows; row++)
            {
                if (row > 0)
                {
                    builder.Append('\n');
                }

                for (var column = 0; column < columns; column++)
                {
                    var (controller, address) = VisibleAddress(row, column);
                    builder.Append(CharacterRomTables.ToChar(rom, ddram[controller][IndexOf(address)]));
                }
            }

            return builder.ToString();
        }
    }

    /// <summary>
    /// Gets a copy of the DDRAM of a controller.
    /// </summary>
    /// <param name="controller">The controller chip, 0 or 1.</param>
    /// <returns>80 bytes: the 40 bytes of the line at 0x00, then the 40 bytes of the line at 0x40.</returns>
    public byte[] GetDdram(int controller)
    {
        ValidateController(controller);

        lock (sync)
        {
            return (byte[])ddram[controller].Clone();
        }
    }

    /// <summary>
    /// Sets one DDRAM byte.
    /// </summary>
    /// <param name="controller">The controller chip, 0 or 1.</param>
    /// <param name="address">
    /// The DDRAM address, 0x00 to 0x7F (the 7-bit address counter); an address wraps at 0x28 within its line,
    /// so 0x28 to 0x3F write 0x00 to 0x17 and 0x68 to 0x7F write 0x40 to 0x57.
    /// </param>
    /// <param name="code">The character code.</param>
    /// <exception cref="ArgumentOutOfRangeException">The controller is not 0 or 1, or the address is outside 0x00 to 0x7F.</exception>
    public void SetDdram(
        int controller,
        int address,
        byte code)
    {
        ValidateController(controller);
        ValidateAddress(address);

        lock (sync)
        {
            ddram[controller][IndexOf(address)] = code;
        }

        NotifyChanged();
    }

    /// <summary>
    /// Sets one row of a custom glyph.
    /// </summary>
    /// <param name="index">The custom glyph, 0 to 7.</param>
    /// <param name="row">The glyph row, 0 to 7.</param>
    /// <param name="bits">The dots of the row; only the low 5 bits are kept.</param>
    public void SetCgram(
        int index,
        int row,
        byte bits)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, GlyphCount);
        ArgumentOutOfRangeException.ThrowIfNegative(row);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(row, GlyphRows);

        lock (sync)
        {
            cgram[(index * GlyphRows) + row] = (byte)(bits & GlyphBitsMask);
        }

        NotifyChanged();
    }

    /// <summary>
    /// Gets a copy of a custom glyph.
    /// </summary>
    /// <param name="index">The custom glyph, 0 to 7.</param>
    /// <returns>A new array of 8 rows, row 0 at the top, using the low 5 bits of each row.</returns>
    public byte[] GetCustomGlyph(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, GlyphCount);

        lock (sync)
        {
            return cgram.AsSpan(index * GlyphRows, GlyphRows).ToArray();
        }
    }

    /// <summary>
    /// Gets the visible cell the cursor is on, with the display shift applied.
    /// </summary>
    /// <returns>The row and column, or <see langword="null"/> when the cursor is off-screen.</returns>
    public (int Row, int Column)? GetCursorCell()
    {
        lock (sync)
        {
            var cursorIndex = IndexOf(cursorAddress);
            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    var (controller, address) = VisibleAddress(row, column);
                    if (controller == cursorController &&
                        IndexOf(address) == cursorIndex)
                    {
                        return (row, column);
                    }
                }
            }

            return null;
        }
    }

    // Unchecked: the callers pass a cell inside a valid size.
    private static int CellAddress(
        int row,
        int column,
        int columns,
        int rows,
        out int controller)
    {
        var secondLine = row % 2 == 0
            ? 0
            : SecondLineAddress;

        if (row < 2)
        {
            controller = 0;
            return secondLine + column;
        }

        if (ControllerCountOf(columns, rows) == ControllerMax)
        {
            controller = 1;
            return secondLine + column;
        }

        controller = 0;
        return secondLine + columns + column;
    }

    private static int ControllerCountOf(
        int columns,
        int rows)
        => rows > 2 && columns > 20
            ? ControllerMax
            : 1;

    private static int IndexOf(int address)
    {
        var lineStart = (address & SecondLineAddress) != 0
            ? LineLength
            : 0;

        return lineStart + ((address & LineOffsetMask) % LineLength);
    }

    private static void ValidateSize(
        int columns,
        int rows)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(columns, MinColumns);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(columns, MaxColumns);
        ArgumentOutOfRangeException.ThrowIfLessThan(rows, MinRows);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(rows, MaxRows);
    }

    private static void ValidateAddress(int address)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(address);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(address, MaxAddress);
    }

    private static void ValidateController(int controller)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(controller);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(controller, ControllerMax);
    }

    private void ValidateCell(
        int row,
        int column)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(row);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(row, rows);
        ArgumentOutOfRangeException.ThrowIfNegative(column);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(column, columns);
    }

    // Must be called with the lock held.
    private (int Controller, int Address) VisibleAddress(
        int row,
        int column)
    {
        var address = CellAddress(row, column, columns, rows, out var controller);
        var lineStart = address & SecondLineAddress;
        var shift = ((shiftOffset % LineLength) + LineLength) % LineLength;
        var offset = ((address - lineStart) + shift) % LineLength;

        return (controller, lineStart | offset);
    }

    private void NotifyChanged()
    {
        lock (sync)
        {
            if (batchDepth > 0)
            {
                changedPending = true;
                return;
            }
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }

    private void EndBatch()
    {
        lock (sync)
        {
            batchDepth--;
            if (batchDepth > 0 || !changedPending)
            {
                return;
            }

            changedPending = false;
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }

    private sealed class BatchScope : IDisposable
    {
        private CharacterDisplayBuffer? buffer;

        public BatchScope(CharacterDisplayBuffer buffer)
            => this.buffer = buffer;

        public void Dispose()
            => Interlocked.Exchange(ref buffer, null)?.EndBatch();
    }
}