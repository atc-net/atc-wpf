namespace Atc.Wpf.Displays;

/// <summary>
/// A monochrome graphic OLED module (SSD1306, SSD1309, SH1106 and compatibles, up to 132 x 64 pixels) drawn pixel by pixel,
/// so a screen can mirror the display of an ESP32 or Raspberry Pi project exactly.
/// </summary>
/// <remarks>
/// <para>
/// The control shows a <see cref="MonochromeFrameBuffer"/>, the picture and display state of the chip. The buffer is the single source of truth:
/// <see cref="Text"/> writes into it, one way. An emulator that drives the same buffer (e.g. one bound through <see cref="Buffer"/>)
/// is shown as it writes.
/// </para>
/// <para>
/// A pixel is lit when the display is on and either the entire display is on or the pixel, XOR the buffer's
/// <see cref="MonochromeFrameBuffer.IsInverted"/>, XOR <see cref="IsInverted"/>, is on.
/// </para>
/// <para>
/// The control drives <see cref="DotMatrixDisplayBase.Brightness"/> from the buffer's <see cref="MonochromeFrameBuffer.Contrast"/>:
/// <c>0.4 + 0.6 * Contrast / 255</c>, so contrast 0 is dim but visible and 255 is full. It is set with
/// <see cref="DependencyObject.SetCurrentValue"/> whenever the contrast changes or a new buffer is assigned, so a <see cref="DotMatrixDisplayBase.Brightness"/>
/// set on the control is replaced on the next contrast change.
/// </para>
/// <para>
/// The colours come from <see cref="Color"/>; a brush, dot shape or ghost opacity set on the control (locally, by a style or a binding)
/// wins over the preset. The top <see cref="YellowZoneRows"/> rows of a <see cref="OledDisplayColor.YellowBlue"/> module are drawn yellow.
/// </para>
/// <para>
/// Buffer changes may come from any thread; they are drawn on the UI thread, at most once per frame.
/// </para>
/// </remarks>
public sealed partial class OledDisplay : DotMatrixDisplayBase
{
    private const int MinPixelColumns = 1;
    private const int MinPixelRows = 1;
    private const int PageHeight = 8;
    private const int MaxSnapshotAttempts = 4;
    private const double DotGap = 0.15;
    private const double OledDotCornerRadius = 0.1;
    private const double MinContrastBrightness = 0.4;
    private const double ContrastBrightnessRange = 0.6;
    private const double MaxContrast = 255.0;
    private const int NoContrast = -1;
    private const int MaxBrightnessAttempts = 3;
    private static readonly SolidColorBrush YellowZoneBrush = OledDisplayPalette.CreateBrush(OledDisplayPalette.YellowZoneInk);

    private readonly MonochromeFrameBuffer ownBuffer;

    /// <summary>The number of pixel columns of the module, 1 to 132.</summary>
    [DependencyProperty(
        DefaultValue = 128,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnPixelSizeChanged),
        CoerceValueCallback = nameof(CoercePixelColumns))]
    private int pixelColumns;

    /// <summary>The number of pixel rows of the module, 1 to 64.</summary>
    [DependencyProperty(
        DefaultValue = 64,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnPixelSizeChanged),
        CoerceValueCallback = nameof(CoercePixelRows))]
    private int pixelRows;

    /// <summary>The colour preset of the module.</summary>
    [DependencyProperty(
        DefaultValue = OledDisplayColor.White,
        PropertyChangedCallback = nameof(OnColorChanged))]
    private OledDisplayColor color;

    /// <summary>The number of rows drawn yellow at the top of a <see cref="OledDisplayColor.YellowBlue"/> module, 0 to <see cref="PixelRows"/>; real modules use 16.</summary>
    [DependencyProperty(
        DefaultValue = 16,
        PropertyChangedCallback = nameof(OnDotsAffectingChanged),
        CoerceValueCallback = nameof(CoerceYellowZoneRows))]
    private int yellowZoneRows;

    /// <summary>Whether the picture is shown inverted; XORed with the buffer's <see cref="MonochromeFrameBuffer.IsInverted"/>.</summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnDotsAffectingChanged))]
    private bool isInverted;

    /// <summary>
    /// The text to show: the buffer is cleared, then the text is drawn from the top-left with the 5 x 8 glyphs of <see cref="CharacterRom"/>,
    /// a glyph 6 pixels wide and lines separated by <c>\n</c> 8 rows apart (<c>\r</c> is ignored). The part outside the module is clipped.
    /// </summary>
    /// <remarks>
    /// It is one-way: changes made to the buffer by other means are not reflected back.
    /// </remarks>
    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnTextChanged))]
    private string text;

    /// <summary>The character ROM that maps the characters of <see cref="Text"/> to glyphs.</summary>
    [DependencyProperty(
        DefaultValue = CharacterRom.A00,
        PropertyChangedCallback = nameof(OnCharacterRomChanged))]
    private CharacterRom characterRom;

    /// <summary>
    /// The picture to show. The control creates its own; set one (e.g. bound to a view model that runs an emulator) to show it.
    /// A supplied buffer is resized to <see cref="PixelColumns"/> by <see cref="PixelRows"/>; <see langword="null"/> returns to the control's own buffer.
    /// </summary>
    /// <remarks>
    /// A newly assigned buffer takes over a non-empty <see cref="Text"/>, whatever the order the two were set in; an empty text leaves the buffer's picture alone.
    /// </remarks>
    [DependencyProperty(
        PropertyChangedCallback = nameof(OnBufferChanged),
        CoerceValueCallback = nameof(CoerceBuffer))]
    private MonochromeFrameBuffer buffer;

    private DotMatrixGeometry? geometry;
    private MonochromeFrameBuffer? subscribedBuffer;
    private byte[] snapshotFrame = [];
    private int snapshotWidth;
    private int snapshotHeight;
    private bool isSnapshotDisplayOn;
    private bool isSnapshotEntireDisplayOn;
    private bool isSnapshotInverted;
    private int snapshotYellowZoneRows;
    private int contentVersion;
    private int appliedContrast = NoContrast;
    private int isBrightnessUpdatePending;

    /// <summary>
    /// Initializes a new instance of the <see cref="OledDisplay"/> class: a 128 x 64 white OLED.
    /// </summary>
    public OledDisplay()
    {
        ownBuffer = new MonochromeFrameBuffer(PixelColumns, PixelRows);

        CoerceValue(BufferProperty);
        ApplyPreset();
    }

    /// <summary>Gets or sets an action run after the snapshot of the buffer is taken, once per redraw; tests only.</summary>
    internal Action? SnapshotTakenForTests { get; set; }

    /// <summary>Gets or sets an action run on the UI thread right after the brightness update has read the buffer's contrast; tests only.</summary>
    internal Action? ContrastReadForTests { get; set; }

    /// <summary>Gets the dot grid the control draws.</summary>
    internal DotMatrixGeometry CurrentGeometry => GetGeometry();

    /// <inheritdoc />
    protected override int ColorZoneCount
        => Color == OledDisplayColor.YellowBlue ? 2 : 1;

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new OledDisplayAutomationPeer(this);

    /// <inheritdoc />
    protected override DotMatrixGeometry GetGeometry()
    {
        var columnCount = PixelColumns;
        var rowCount = PixelRows;
        if (geometry is null ||
            geometry.DotColumns != columnCount ||
            geometry.DotRows != rowCount)
        {
            geometry = new DotMatrixGeometry(
                columnCount,
                rowCount,
                CellWidth: 0,
                CellHeight: 0,
                DotGap,
                CellGapX: 0,
                CellGapY: 0);
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
        SnapshotTakenForTests?.Invoke();
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
        if (!isSnapshotDisplayOn)
        {
            return false;
        }

        if (isSnapshotEntireDisplayOn)
        {
            return true;
        }

        var isPixelOn = x < snapshotWidth &&
                        y < snapshotHeight &&
                        (snapshotFrame[((y / PageHeight) * snapshotWidth) + x] & (1 << (y % PageHeight))) != 0;

        return isPixelOn ^ isSnapshotInverted;
    }

    /// <inheritdoc />
    protected override int GetDotColorZone(
        int x,
        int y)
        => y < snapshotYellowZoneRows ? 1 : 0;

    /// <inheritdoc />
    protected override Brush? GetZoneLitDotBrush(int zone)
        => zone == 1
            ? YellowZoneBrush
            : LitDotBrush;

    private static object CoercePixelColumns(
        DependencyObject d,
        object? value)
        => System.Math.Clamp((int)value!, MinPixelColumns, MonochromeFrameBuffer.MaxWidth);

    private static object CoercePixelRows(
        DependencyObject d,
        object? value)
        => System.Math.Clamp((int)value!, MinPixelRows, MonochromeFrameBuffer.MaxHeight);

    private static object CoerceYellowZoneRows(
        DependencyObject d,
        object? value)
        => System.Math.Clamp((int)value!, 0, ((OledDisplay)d).PixelRows);

    private static object CoerceBuffer(
        DependencyObject d,
        object? value)
        => value ?? ((OledDisplay)d).ownBuffer;

    private static void OnPixelSizeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (OledDisplay)d;
        display.CoerceValue(YellowZoneRowsProperty);
        display.Buffer.Resize(display.PixelColumns, display.PixelRows);

        // Text past the old size was clipped, so growing the module draws the text again.
        // An empty text is not re-applied, so a resize never blanks an emulator's content.
        if (!string.IsNullOrEmpty(display.Text))
        {
            display.ApplyText();
        }

        display.InvalidateDots();
    }

    private static void OnColorChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((OledDisplay)d).ApplyPreset();

    private static void OnDotsAffectingChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((OledDisplay)d).InvalidateDots();

    private static void OnTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((OledDisplay)d).ApplyText();

    private static void OnCharacterRomChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (OledDisplay)d;

        // The glyphs of the text depend on the ROM. An empty text is not re-applied, so a ROM change never blanks an emulator's content.
        if (!string.IsNullOrEmpty(display.Text))
        {
            display.ApplyText();
        }
    }

    private static void OnBufferChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (OledDisplay)d;

        if (e.OldValue is MonochromeFrameBuffer oldBuffer)
        {
            WeakEventManager<MonochromeFrameBuffer, EventArgs>.RemoveHandler(
                oldBuffer,
                nameof(MonochromeFrameBuffer.Changed),
                display.OnBufferContentChanged);
        }

        if (e.NewValue is MonochromeFrameBuffer newBuffer)
        {
            Volatile.Write(ref display.subscribedBuffer, newBuffer);
            WeakEventManager<MonochromeFrameBuffer, EventArgs>.AddHandler(
                newBuffer,
                nameof(MonochromeFrameBuffer.Changed),
                display.OnBufferContentChanged);

            using (newBuffer.BeginBatch())
            {
                newBuffer.Resize(display.PixelColumns, display.PixelRows);

                // A buffer that arrives after the text (e.g. through a binding that resolves late) takes it over.
                if (!string.IsNullOrEmpty(display.Text))
                {
                    display.ApplyText();
                }
            }
        }

        display.UpdateBrightness();
        display.InvalidateDots();
    }

    private void ApplyPreset()
    {
        var (panel, ink) = OledDisplayPalette.Get(Color);

        SetPresetValue(PanelBrushProperty, OledDisplayPalette.CreateBrush(panel));
        SetPresetValue(LitDotBrushProperty, OledDisplayPalette.CreateBrush(ink));
        SetPresetValue(GhostOpacityProperty, 0.0);
        SetPresetValue(DotCornerRadiusProperty, OledDotCornerRadius);

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

    private void ApplyText()
    {
        var target = Buffer;
        var value = Text ?? string.Empty;

        using (target.BeginBatch())
        {
            target.Clear();
            if (value.Length > 0)
            {
                target.DrawText(0, 0, value, CharacterRom);
            }
        }
    }

    // UI thread only. The contrast is read again after it was recorded as applied: a worker that set the contrast back to the recorded
    // value between the first read and the record compared it with the old record, saw nothing to update and posted nothing, so the
    // change is caught here. (The record is a full fence and the buffer reads and writes the contrast under its lock, so either this
    // read sees the worker's value or the worker sees the new record and posts an update.)
    private void UpdateBrightness()
    {
        for (var attempt = 0; attempt < MaxBrightnessAttempts; attempt++)
        {
            int contrast = Buffer.Contrast;
            ContrastReadForTests?.Invoke();
            Interlocked.Exchange(ref appliedContrast, contrast);
            SetCurrentValue(
                BrightnessProperty,
                MinContrastBrightness + (ContrastBrightnessRange * contrast / MaxContrast));

            if (Buffer.Contrast == contrast)
            {
                return;
            }
        }

        // The contrast keeps changing: apply it again from the dispatcher.
        PostBrightnessUpdate();
    }

    // Reads the buffer once per render, so IsDotLit answers from a consistent copy without locking per dot.
    private void TakeSnapshot()
    {
        var source = Buffer;
        var frame = Array.Empty<byte>();
        var frameWidth = 0;
        var frameHeight = 0;

        // The size and the frame are read under separate locks; an emulator may resize in between, so read again until they match.
        for (var attempt = 0; attempt < MaxSnapshotAttempts; attempt++)
        {
            frameWidth = source.Width;
            frameHeight = source.Height;
            frame = source.GetFrame();
            if (frame.Length == frameWidth * ((frameHeight + PageHeight - 1) / PageHeight))
            {
                break;
            }

            frame = [];
            frameWidth = 0;
            frameHeight = 0;
        }

        snapshotFrame = frame;
        snapshotWidth = frameWidth;
        snapshotHeight = frameHeight;
        isSnapshotDisplayOn = source.IsDisplayOn;
        isSnapshotEntireDisplayOn = source.IsEntireDisplayOn;
        isSnapshotInverted = source.IsInverted ^ IsInverted;
        snapshotYellowZoneRows = Color == OledDisplayColor.YellowBlue ? YellowZoneRows : 0;
    }

    // Raised on the thread that changed the buffer.
    private void OnBufferContentChanged(
        object? sender,
        EventArgs e)
    {
        if (sender is not MonochromeFrameBuffer source ||
            !ReferenceEquals(source, Volatile.Read(ref subscribedBuffer)))
        {
            return;
        }

        Interlocked.Increment(ref contentVersion);
        InvalidateDots();

        if (source.Contrast == Volatile.Read(ref appliedContrast))
        {
            return;
        }

        if (Dispatcher.CheckAccess())
        {
            UpdateBrightness();
            return;
        }

        PostBrightnessUpdate();
    }

    // Any thread: one brightness update at a time is queued on the dispatcher; it reads the latest contrast when it runs.
    private void PostBrightnessUpdate()
    {
        if (Interlocked.CompareExchange(ref isBrightnessUpdatePending, 1, 0) != 0 ||
            Dispatcher.HasShutdownStarted)
        {
            return;
        }

        _ = Dispatcher.BeginInvoke(
            DispatcherPriority.Render,
            new Action(() =>
            {
                Interlocked.Exchange(ref isBrightnessUpdatePending, 0);
                UpdateBrightness();
            }));
    }
}