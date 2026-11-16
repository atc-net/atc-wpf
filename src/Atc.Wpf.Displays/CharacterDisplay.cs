namespace Atc.Wpf.Displays;

/// <summary>
/// A character LCD or VFD module (HD44780 and compatibles, 1 to 40 columns by 1 to 4 rows) drawn dot by dot,
/// so a screen can mirror the display of an ESP32 or Raspberry Pi project exactly.
/// </summary>
/// <remarks>
/// <para>
/// The control shows a <see cref="CharacterDisplayBuffer"/>, the model of the controller chip. The buffer is the single source of truth:
/// <see cref="Text"/>, <see cref="CursorMode"/>, <see cref="CursorRow"/>, <see cref="CursorColumn"/>, <see cref="IsDisplayOn"/>,
/// <see cref="CustomCharacters"/> and the methods write into it, one way. A <see cref="Hd44780Controller"/> that drives the same buffer
/// (e.g. one bound through <see cref="Buffer"/>) is shown as it writes, but its changes are not reflected back into these properties.
/// A buffer assigned after these properties takes over the ones that are set (see <see cref="Buffer"/>), so a view model can bind it in XAML.
/// </para>
/// <para>
/// <see cref="Columns"/> and <see cref="Rows"/> describe the physical module: a buffer set through <see cref="Buffer"/> is resized to them,
/// and the content stays at its DDRAM positions when they change; a non-empty <see cref="Text"/> is written again, so growing the module shows all of it.
/// </para>
/// <para>
/// The colours come from <see cref="Kind"/>, <see cref="Color"/> and <see cref="IsBacklightOn"/>; a brush, dot shape or ghost opacity
/// set on the control (locally, by a style or a binding) wins over the preset.
/// </para>
/// <para>
/// Buffer changes may come from any thread; they are drawn on the UI thread, at most once per frame.
/// A blinking cursor runs a 400 ms timer only while the control is loaded and visible.
/// </para>
/// </remarks>
public sealed partial class CharacterDisplay : DotMatrixDisplayBase
{
    private const int CellWidth = 5;
    private const int CellHeight = 8;
    private const int CursorDotRow = 7;
    private const int GlyphCount = 8;
    private const byte FullRow = 0x1F;
    private const byte FirstDotBit = 0x10;
    private const int FirstRomCode = 0x10;
    private const int GlyphIndexMask = 0x07;
    private const int MinColumns = 1;
    private const int MaxColumns = 40;
    private const int MinRows = 1;
    private const int MaxRows = 4;
    private const double DotGap = 0.1;
    private const double CharacterGap = 1.0;
    private const double VfdDotCornerRadius = 0.35;
    private const double VfdGlowRadius = 0.6;
    private static readonly TimeSpan BlinkInterval = TimeSpan.FromMilliseconds(400);

    private readonly CharacterDisplayBuffer ownBuffer;
    private readonly DispatcherTimer blinkTimer;
    private readonly byte[] glyphRam = new byte[GlyphCount * CellHeight];

    /// <summary>The number of character columns of the module, 1 to 40.</summary>
    [DependencyProperty(
        DefaultValue = 16,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnSizeChanged),
        ValidateValueCallback = nameof(IsValidColumns))]
    private int columns;

    /// <summary>The number of character rows of the module, 1 to 4.</summary>
    [DependencyProperty(
        DefaultValue = 2,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnSizeChanged),
        ValidateValueCallback = nameof(IsValidRows))]
    private int rows;

    /// <summary>The display technology: an LCD or a VFD.</summary>
    [DependencyProperty(
        DefaultValue = CharacterDisplayKind.Lcd,
        PropertyChangedCallback = nameof(OnPresetChanged))]
    private CharacterDisplayKind kind;

    /// <summary>The colour preset; a VFD shows an LCD-only preset as <see cref="CharacterDisplayColor.BlueGreen"/>.</summary>
    [DependencyProperty(
        DefaultValue = CharacterDisplayColor.YellowGreen,
        PropertyChangedCallback = nameof(OnPresetChanged))]
    private CharacterDisplayColor color;

    /// <summary>Whether the LCD backlight is on; off shows the reflective grey-green panel. A VFD ignores it.</summary>
    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnPresetChanged))]
    private bool isBacklightOn;

    /// <summary>
    /// The text to show: lines separated by <c>\n</c> (<c>\r</c> is ignored), each written from column 0.
    /// Characters past the last column and lines past the last row are dropped; missing lines are blank.
    /// </summary>
    /// <remarks>
    /// Setting it overwrites the visible characters only; custom glyphs, the cursor and the display shift are kept.
    /// It is one-way: changes made to the buffer by other means are not reflected back.
    /// </remarks>
    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnTextChanged))]
    private string text;

    /// <summary>The character ROM that maps the characters of <see cref="Text"/> to codes and draws them.</summary>
    [DependencyProperty(
        DefaultValue = CharacterRom.A00,
        PropertyChangedCallback = nameof(OnCharacterRomChanged))]
    private CharacterRom characterRom;

    /// <summary>How the cursor is shown: none, an underline (dot row 7), a blinking block, or both.</summary>
    [DependencyProperty(
        DefaultValue = CharacterDisplayCursor.None,
        PropertyChangedCallback = nameof(OnCursorModeChanged))]
    private CharacterDisplayCursor cursorMode;

    /// <summary>The row of the cursor, clamped to the rows of the module.</summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnCursorPositionChanged))]
    private int cursorRow;

    /// <summary>The column of the cursor, clamped to the columns of the module.</summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnCursorPositionChanged))]
    private int cursorColumn;

    /// <summary>Whether the display is on; off blanks the dots but keeps the panel and the backlight.</summary>
    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnIsDisplayOnChanged))]
    private bool isDisplayOn;

    /// <summary>
    /// Up to 8 custom glyphs for the codes <c>\u0000</c> to <c>\u0007</c>: 8 bytes each, row 0 at the top, the low 5 bits of each row.
    /// They are written into the glyph RAM when the property is set; later changes to the list are not followed.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnCustomCharactersChanged))]
    private IList<byte[]>? customCharacters;

    /// <summary>
    /// The display state to show. The control creates its own; set one (e.g. bound to a view model that runs a <see cref="Hd44780Controller"/>)
    /// to show it. A supplied buffer is resized to <see cref="Columns"/> by <see cref="Rows"/>; <see langword="null"/> returns to the control's own buffer.
    /// </summary>
    /// <remarks>
    /// A newly assigned buffer takes over the content properties already set on the control, whatever the order they were set in:
    /// a non-empty <see cref="Text"/>, non-null <see cref="CustomCharacters"/>, and <see cref="CursorMode"/>, <see cref="IsDisplayOn"/>,
    /// <see cref="CursorRow"/> and <see cref="CursorColumn"/> when they are set (locally, by a style or by a binding, even to their default value).
    /// Properties left on their default do not overwrite the buffer's own cursor and display state.
    /// </remarks>
    [DependencyProperty(
        PropertyChangedCallback = nameof(OnBufferChanged),
        CoerceValueCallback = nameof(CoerceBuffer))]
    private CharacterDisplayBuffer buffer;

    private DotMatrixGeometry? geometry;
    private byte[] cellRows = [];
    private int snapshotColumns;
    private bool isSnapshotDisplayOn;
    private bool isBlinkPhaseOn = true;
    private int isBlinkUpdatePending;
    private int contentVersion;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterDisplay"/> class: a 16 x 2 yellow-green LCD.
    /// </summary>
    public CharacterDisplay()
    {
        ownBuffer = new CharacterDisplayBuffer(Columns, Rows);
        blinkTimer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher) { Interval = BlinkInterval };
        blinkTimer.Tick += OnBlinkTimerTick;

        CoerceValue(BufferProperty);
        ApplyPreset();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        IsVisibleChanged += OnIsVisibleChanged;
    }

    /// <summary>Gets a value indicating whether the cursor blink timer runs.</summary>
    internal bool IsBlinkTimerRunning => blinkTimer.IsEnabled;

    /// <summary>Gets or sets an action run between the snapshot of the buffer and the drawing of the dots; tests only.</summary>
    internal Action? AfterSnapshotForTests { get; set; }

    /// <summary>Gets the dot grid the control draws.</summary>
    internal DotMatrixGeometry CurrentGeometry => GetGeometry();

    /// <summary>
    /// Clears the display to spaces and resets the display shift and the cursor address, as the chip's clear command does.
    /// <see cref="Text"/> is not changed.
    /// </summary>
    public void Clear()
        => Buffer.Clear();

    /// <summary>
    /// Writes text into a visible row from a column; characters past the last column are dropped.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column of the first character.</param>
    /// <param name="text">The text, mapped through <see cref="CharacterRom"/>.</param>
    public void Write(
        int row,
        int column,
        string text)
        => Buffer.WriteVisible(row, column, text, CharacterRom);

    /// <summary>
    /// Defines a custom glyph, shown for the character code <paramref name="index"/> (<c>\u0000</c> to <c>\u0007</c>).
    /// </summary>
    /// <param name="index">The custom glyph, 0 to 7.</param>
    /// <param name="glyphRows">Up to 8 rows, row 0 at the top, using the low 5 bits of each row; missing rows are blank.</param>
    public void DefineCharacter(
        int index,
        IReadOnlyList<byte> glyphRows)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, GlyphCount);
        ArgumentNullException.ThrowIfNull(glyphRows);
        if (glyphRows.Count > CellHeight)
        {
            throw new ArgumentException("A glyph has at most 8 rows.", nameof(glyphRows));
        }

        WriteGlyph(Buffer, index, glyphRows);
    }

    /// <summary>
    /// Gets the visible text for UI Automation: the rows joined by a new line, with custom glyphs shown as spaces.
    /// </summary>
    /// <returns>The visible text.</returns>
    internal string GetDisplayedText()
    {
        var visible = Buffer.GetVisibleText(CharacterRom).ToCharArray();
        for (var i = 0; i < visible.Length; i++)
        {
            if (visible[i] < GlyphCount)
            {
                visible[i] = ' ';
            }
        }

        return new string(visible);
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new CharacterDisplayAutomationPeer(this);

    /// <inheritdoc />
    protected override DotMatrixGeometry GetGeometry()
    {
        var columnCount = Columns;
        var rowCount = Rows;
        if (geometry is null ||
            geometry.DotColumns != columnCount * CellWidth ||
            geometry.DotRows != rowCount * CellHeight)
        {
            geometry = new DotMatrixGeometry(
                columnCount * CellWidth,
                rowCount * CellHeight,
                CellWidth,
                CellHeight,
                DotGap,
                CharacterGap,
                CharacterGap);
        }

        return geometry;
    }

    /// <inheritdoc />
    protected override void OnRender(DrawingContext drawingContext)
    {
        // A buffer change that lands after the snapshot sets the base's dirty flag, which the base then clears while it draws
        // the older snapshot; the version check marks the dots dirty again so the next frame draws the change.
        var version = Volatile.Read(ref contentVersion);
        TakeSnapshot();
        AfterSnapshotForTests?.Invoke();
        base.OnRender(drawingContext);

        if (Volatile.Read(ref contentVersion) != version)
        {
            InvalidateDots();
        }
    }

    /// <inheritdoc />
    protected override bool IsDotLit(
        int x,
        int y)
    {
        if (!isSnapshotDisplayOn || snapshotColumns == 0)
        {
            return false;
        }

        var index = ((((y / CellHeight) * snapshotColumns) + (x / CellWidth)) * CellHeight) + (y % CellHeight);
        return index < cellRows.Length &&
               (cellRows[index] & (FirstDotBit >> (x % CellWidth))) != 0;
    }

    private static bool IsValidColumns(object? value)
        => value is >= MinColumns and <= MaxColumns;

    private static bool IsValidRows(object? value)
        => value is >= MinRows and <= MaxRows;

    private static bool IsBlinking(CharacterDisplayCursor cursor)
        => cursor is CharacterDisplayCursor.Blink or CharacterDisplayCursor.UnderlineAndBlink;

    private static bool HasUnderline(CharacterDisplayCursor cursor)
        => cursor is CharacterDisplayCursor.Underline or CharacterDisplayCursor.UnderlineAndBlink;

    private static void WriteGlyph(
        CharacterDisplayBuffer target,
        int index,
        IReadOnlyList<byte> glyphRows)
    {
        using (target.BeginBatch())
        {
            for (var row = 0; row < CellHeight; row++)
            {
                target.SetCgram(index, row, row < glyphRows.Count ? glyphRows[row] : (byte)0);
            }
        }
    }

    private static object CoerceBuffer(
        DependencyObject d,
        object? value)
        => value ?? ((CharacterDisplay)d).ownBuffer;

    private static void OnSizeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (CharacterDisplay)d;
        display.Buffer.Resize(display.Columns, display.Rows);

        // Lines and characters past the old size were never written, so growing the module writes the text again.
        // An empty text is not re-applied, so a resize never blanks an emulator's content.
        if (!string.IsNullOrEmpty(display.Text))
        {
            display.ApplyText();
        }

        // A cursor position set before the size (e.g. earlier in XAML) was clamped to the old size, so it is applied again.
        if (display.IsSetLocally(CursorRowProperty) || display.IsSetLocally(CursorColumnProperty))
        {
            display.ApplyCursorPosition();
        }

        display.InvalidateDots();
    }

    private static void OnPresetChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((CharacterDisplay)d).ApplyPreset();

    private static void OnTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((CharacterDisplay)d).ApplyText();

    private static void OnCharacterRomChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (CharacterDisplay)d;

        // The codes of the text depend on the ROM. An empty text is not re-applied, so a ROM change never blanks an emulator's content.
        if (!string.IsNullOrEmpty(display.Text))
        {
            display.ApplyText();
        }

        display.InvalidateDots();
    }

    private static void OnCursorModeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (CharacterDisplay)d;
        display.Buffer.CursorMode = display.CursorMode;
        display.UpdateBlinkTimer();
    }

    private static void OnCursorPositionChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((CharacterDisplay)d).ApplyCursorPosition();

    private static void OnIsDisplayOnChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((CharacterDisplay)d).Buffer.IsDisplayOn = (bool)e.NewValue;

    private static void OnCustomCharactersChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((CharacterDisplay)d).ApplyCustomCharacters();

    private static void OnBufferChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (CharacterDisplay)d;

        if (e.OldValue is CharacterDisplayBuffer oldBuffer)
        {
            WeakEventManager<CharacterDisplayBuffer, EventArgs>.RemoveHandler(
                oldBuffer,
                nameof(CharacterDisplayBuffer.Changed),
                display.OnBufferContentChanged);
        }

        if (e.NewValue is CharacterDisplayBuffer newBuffer)
        {
            WeakEventManager<CharacterDisplayBuffer, EventArgs>.AddHandler(
                newBuffer,
                nameof(CharacterDisplayBuffer.Changed),
                display.OnBufferContentChanged);

            using (newBuffer.BeginBatch())
            {
                if (newBuffer.Columns != display.Columns ||
                    newBuffer.Rows != display.Rows)
                {
                    newBuffer.Resize(display.Columns, display.Rows);
                }

                // A buffer that arrives after the content properties (e.g. through a binding that resolves late) takes them over.
                display.ApplyContentProperties();
            }
        }

        display.InvalidateDots();
        display.UpdateBlinkTimer();
    }

    private void ApplyPreset()
    {
        var (panel, ink, unlit, ghostOpacity) = CharacterDisplayPalette.Get(Kind, Color, IsBacklightOn);
        var isVfd = Kind == CharacterDisplayKind.Vfd;

        SetPresetValue(PanelBrushProperty, CharacterDisplayPalette.CreateBrush(panel));
        SetPresetValue(LitDotBrushProperty, CharacterDisplayPalette.CreateBrush(ink));
        SetPresetValue(UnlitDotBrushProperty, CharacterDisplayPalette.CreateBrush(unlit));
        SetPresetValue(GhostOpacityProperty, ghostOpacity);
        SetPresetValue(DotCornerRadiusProperty, isVfd ? VfdDotCornerRadius : 0.0);
        SetPresetValue(GlowRadiusProperty, isVfd ? VfdGlowRadius : 0.0);

        InvalidateDots();
    }

    // A value set locally, by a style or by a binding wins: only a property still on its default (or on a preset value) is changed.
    private void SetPresetValue(
        DependencyProperty property,
        object value)
    {
        if (DependencyPropertyHelper.GetValueSource(this, property).BaseValueSource == BaseValueSource.Default)
        {
            SetCurrentValue(property, value);
        }
    }

    private bool IsSetLocally(DependencyProperty property)
        => DependencyPropertyHelper.GetValueSource(this, property).BaseValueSource != BaseValueSource.Default;

    private void ApplyText()
    {
        var target = Buffer;
        var rom = CharacterRom;
        var lines = (Text ?? string.Empty)
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Split('\n');

        using (target.BeginBatch())
        {
            var columnCount = target.Columns;
            var blank = new string(' ', columnCount);
            for (var row = 0; row < target.Rows; row++)
            {
                target.WriteVisible(row, 0, blank, rom);
                if (row < lines.Length && lines[row].Length > 0)
                {
                    target.WriteVisible(row, 0, lines[row], rom);
                }
            }
        }
    }

    private void ApplyCustomCharacters()
    {
        if (CustomCharacters is not { } glyphs)
        {
            return;
        }

        var target = Buffer;
        using (target.BeginBatch())
        {
            for (var i = 0; i < glyphs.Count && i < GlyphCount; i++)
            {
                if (glyphs[i] is { } glyph)
                {
                    WriteGlyph(target, i, glyph);
                }
            }
        }
    }

    // Pushes the content properties into a newly assigned buffer. A property still on its default is left alone,
    // so an emulator's own cursor, display state and screen are kept; a value set locally, by a style or by a binding
    // (even one equal to the default) is applied. A non-empty Text and non-null CustomCharacters are always applied.
    private void ApplyContentProperties()
    {
        var target = Buffer;

        ApplyCustomCharacters();

        if (!string.IsNullOrEmpty(Text))
        {
            ApplyText();
        }

        if (IsSetLocally(CursorModeProperty))
        {
            target.CursorMode = CursorMode;
        }

        if (IsSetLocally(IsDisplayOnProperty))
        {
            target.IsDisplayOn = IsDisplayOn;
        }

        if (IsSetLocally(CursorRowProperty) || IsSetLocally(CursorColumnProperty))
        {
            ApplyCursorPosition();
        }
    }

    private void ApplyCursorPosition()
    {
        var target = Buffer;
        var columnCount = target.Columns;
        var rowCount = target.Rows;
        var row = System.Math.Clamp(CursorRow, 0, rowCount - 1);
        var column = System.Math.Clamp(CursorColumn, 0, columnCount - 1);
        var address = CharacterDisplayBuffer.AddressOf(row, column, columnCount, rowCount, out var controller);

        using (target.BeginBatch())
        {
            target.CursorController = controller;
            target.CursorAddress = address;
        }
    }

    // Reads the buffer once per render, so IsDotLit answers from a consistent copy without locking per dot.
    private void TakeSnapshot()
    {
        var source = Buffer;
        var rom = CharacterRom;
        var columnCount = System.Math.Min(Columns, source.Columns);
        var rowCount = System.Math.Min(Rows, source.Rows);
        var cursor = source.CursorMode;
        var cursorCell = source.GetCursorCell();

        isSnapshotDisplayOn = source.IsDisplayOn;
        snapshotColumns = Columns;

        var size = Columns * Rows * CellHeight;
        if (cellRows.Length != size)
        {
            cellRows = new byte[size];
        }
        else
        {
            Array.Clear(cellRows);
        }

        var loadedGlyphs = 0;
        for (var row = 0; row < rowCount; row++)
        {
            for (var column = 0; column < columnCount; column++)
            {
                var code = source.GetCode(row, column);
                var glyph = GetSnapshotGlyph(source, rom, code, ref loadedGlyphs);

                var start = ((row * snapshotColumns) + column) * CellHeight;
                glyph[..CellHeight].CopyTo(cellRows.AsSpan(start, CellHeight));
            }
        }

        if (cursorCell is not { } cell ||
            cell.Row >= rowCount ||
            cell.Column >= columnCount)
        {
            return;
        }

        var cursorStart = ((cell.Row * snapshotColumns) + cell.Column) * CellHeight;
        if (IsBlinking(cursor) && isBlinkPhaseOn)
        {
            Array.Fill(cellRows, FullRow, cursorStart, CellHeight);
        }
        else if (HasUnderline(cursor))
        {
            cellRows[cursorStart + CursorDotRow] = FullRow;
        }
    }

    // Custom glyphs are copied once per snapshot into a reused array; ROM glyphs come from the tables.
    private ReadOnlySpan<byte> GetSnapshotGlyph(
        CharacterDisplayBuffer source,
        CharacterRom rom,
        byte code,
        ref int loadedGlyphs)
    {
        if (code >= FirstRomCode)
        {
            return CharacterRomTables.GetGlyph(rom, code);
        }

        var slot = code & GlyphIndexMask;
        var glyph = glyphRam.AsSpan(slot * CellHeight, CellHeight);
        if ((loadedGlyphs & (1 << slot)) == 0)
        {
            source.GetCustomGlyph(slot).CopyTo(glyph);
            loadedGlyphs |= 1 << slot;
        }

        return glyph;
    }

    // Raised on the thread that changed the buffer.
    private void OnBufferContentChanged(
        object? sender,
        EventArgs e)
    {
        Interlocked.Increment(ref contentVersion);
        InvalidateDots();

        if (Dispatcher.CheckAccess())
        {
            UpdateBlinkTimer();
            return;
        }

        if (Interlocked.CompareExchange(ref isBlinkUpdatePending, 1, 0) != 0 ||
            Dispatcher.HasShutdownStarted)
        {
            return;
        }

        _ = Dispatcher.BeginInvoke(
            DispatcherPriority.Normal,
            new Action(() =>
            {
                Interlocked.Exchange(ref isBlinkUpdatePending, 0);
                UpdateBlinkTimer();
            }));
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
        => UpdateBlinkTimer();

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
        => UpdateBlinkTimer();

    private void OnIsVisibleChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
        => UpdateBlinkTimer();

    private void OnBlinkTimerTick(
        object? sender,
        EventArgs e)
    {
        isBlinkPhaseOn = !isBlinkPhaseOn;
        InvalidateDots();
    }

    // IsLoaded is already false inside a genuine Unloaded, and stays true when a re-parented control's old Unloaded arrives late.
    private void UpdateBlinkTimer()
    {
        var shouldRun = IsLoaded &&
                        IsVisible &&
                        IsBlinking(Buffer.CursorMode);

        if (shouldRun == blinkTimer.IsEnabled)
        {
            return;
        }

        // Start (and stop) with the block shown, as the chip does when the blink is switched on.
        isBlinkPhaseOn = true;
        if (shouldRun)
        {
            blinkTimer.Start();
        }
        else
        {
            blinkTimer.Stop();
            InvalidateDots();
        }
    }
}