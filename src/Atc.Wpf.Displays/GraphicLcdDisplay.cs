namespace Atc.Wpf.Displays;

/// <summary>
/// A 128 x 64 monochrome graphic LCD module (the ST7920 "12864" modules) drawn pixel by pixel, so a screen can mirror the display of an
/// ESP32 or Raspberry Pi project exactly.
/// </summary>
/// <remarks>
/// <para>
/// The control shows a <see cref="MonochromeFrameBuffer"/> of 128 x 64 pixels, the picture of the module. The buffer is the single source
/// of truth: <see cref="Text"/> writes into it, one way. An emulator that drives the same buffer (a <see cref="St7920Controller"/>, fed
/// directly or through a <see cref="St7920SerialDecoder"/>, bound through <see cref="Buffer"/>) is shown as it writes.
/// </para>
/// <para>
/// A pixel is lit when the buffer's display is on and either its entire display is on or the pixel, XOR the buffer's
/// <see cref="MonochromeFrameBuffer.IsInverted"/>, is on. The buffer's contrast is not used: the module has no contrast command.
/// </para>
/// <para>
/// The colours come from <see cref="Color"/> and <see cref="IsBacklightOn"/>, the same presets as an LCD <see cref="CharacterDisplay"/>:
/// a module with the backlight off shows the grey-green reflective look. The VFD-only <see cref="CharacterDisplayColor.BlueGreen"/> falls
/// back to <see cref="CharacterDisplayColor.YellowGreen"/>. A brush, dot shape or ghost opacity set on the control (locally, by a style
/// or a binding) wins over the preset.
/// </para>
/// <para>
/// Buffer changes may come from any thread; they are drawn on the UI thread, at most once per frame.
/// </para>
/// </remarks>
public sealed partial class GraphicLcdDisplay : DotMatrixDisplayBase
{
    /// <summary>The number of pixel columns of the module.</summary>
    public const int PixelColumns = 128;

    /// <summary>The number of pixel rows of the module.</summary>
    public const int PixelRows = 64;

    private const int PageHeight = 8;
    private const int MaxSnapshotAttempts = 4;
    private const double DotGap = 0.1;

    private readonly MonochromeFrameBuffer ownBuffer;
    private readonly DotMatrixGeometry geometry = new(
        PixelColumns,
        PixelRows,
        CellWidth: 0,
        CellHeight: 0,
        DotGap,
        CellGapX: 0,
        CellGapY: 0);

    /// <summary>The colour preset of the module; <see cref="CharacterDisplayColor.BlueGreen"/> (VFD only) shows as <see cref="CharacterDisplayColor.YellowGreen"/>.</summary>
    [DependencyProperty(
        DefaultValue = CharacterDisplayColor.Blue,
        PropertyChangedCallback = nameof(OnPresetChanged))]
    private CharacterDisplayColor color;

    /// <summary>Whether the backlight is on; off shows the grey-green reflective look of an unlit LCD.</summary>
    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnPresetChanged))]
    private bool isBacklightOn;

    /// <summary>
    /// The text to show: the buffer is cleared, then the text is drawn from the top-left with the 5 x 8 glyphs of <see cref="CharacterRom"/>,
    /// a glyph 6 pixels wide and lines separated by <c>\n</c> 8 rows apart (<c>\r</c> is ignored). The part outside the module is clipped.
    /// </summary>
    /// <remarks>
    /// It is one-way: changes made to the buffer by other means are not reflected back. It does not use the ST7920 text layer
    /// (whose 8 x 16 font a <see cref="St7920Controller"/> draws).
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
    /// The picture to show. The control creates its own; set one (e.g. bound to a view model that runs a <see cref="St7920Controller"/>)
    /// to show it. A supplied buffer is resized to 128 x 64; <see langword="null"/> returns to the control's own buffer.
    /// </summary>
    /// <remarks>
    /// A newly assigned buffer takes over a non-empty <see cref="Text"/>, whatever the order the two were set in; an empty text leaves the buffer's picture alone.
    /// </remarks>
    [DependencyProperty(
        PropertyChangedCallback = nameof(OnBufferChanged),
        CoerceValueCallback = nameof(CoerceBuffer))]
    private MonochromeFrameBuffer buffer;

    private MonochromeFrameBuffer? subscribedBuffer;
    private byte[] snapshotFrame = [];
    private int snapshotWidth;
    private int snapshotHeight;
    private bool isSnapshotDisplayOn;
    private bool isSnapshotEntireDisplayOn;
    private bool isSnapshotInverted;
    private int contentVersion;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicLcdDisplay"/> class: a 128 x 64 blue LCD with the backlight on.
    /// </summary>
    public GraphicLcdDisplay()
    {
        ownBuffer = new MonochromeFrameBuffer(PixelColumns, PixelRows);

        CoerceValue(BufferProperty);
        ApplyPreset();
    }

    /// <summary>Gets the dot grid the control draws.</summary>
    internal DotMatrixGeometry CurrentGeometry => geometry;

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new GraphicLcdDisplayAutomationPeer(this);

    /// <inheritdoc />
    protected override DotMatrixGeometry GetGeometry()
        => geometry;

    /// <inheritdoc />
    protected override void OnRender(DrawingContext drawingContext)
    {
        // A buffer change that lands after the snapshot sets the base's dirty flag, which the base then clears while it draws
        // the older snapshot; the version check marks the dots dirty again so the next frame draws the change.
        var version = Volatile.Read(ref contentVersion);
        TakeSnapshot();
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

    private static object CoerceBuffer(
        DependencyObject d,
        object? value)
        => value ?? ((GraphicLcdDisplay)d).ownBuffer;

    private static void OnPresetChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((GraphicLcdDisplay)d).ApplyPreset();

    private static void OnTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((GraphicLcdDisplay)d).ApplyText();

    private static void OnCharacterRomChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (GraphicLcdDisplay)d;

        // An empty text is not re-applied, so a ROM change never blanks an emulator's content.
        if (!string.IsNullOrEmpty(display.Text))
        {
            display.ApplyText();
        }
    }

    private static void OnBufferChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (GraphicLcdDisplay)d;

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
                newBuffer.Resize(PixelColumns, PixelRows);

                // A buffer that arrives after the text (e.g. through a binding that resolves late) takes it over.
                if (!string.IsNullOrEmpty(display.Text))
                {
                    display.ApplyText();
                }
            }
        }

        display.InvalidateDots();
    }

    private void ApplyPreset()
    {
        // ref: the LCD presets of CharacterDisplay; BlueGreen is VFD only and falls back to YellowGreen on an LCD.
        var (panel, ink, unlit, ghostOpacity) = CharacterDisplayPalette.Get(CharacterDisplayKind.Lcd, Color, IsBacklightOn);

        SetPresetValue(PanelBrushProperty, CharacterDisplayPalette.CreateBrush(panel));
        SetPresetValue(LitDotBrushProperty, CharacterDisplayPalette.CreateBrush(ink));
        SetPresetValue(UnlitDotBrushProperty, CharacterDisplayPalette.CreateBrush(unlit));
        SetPresetValue(GhostOpacityProperty, ghostOpacity);
        SetPresetValue(DotCornerRadiusProperty, 0.0);
        SetPresetValue(GlowRadiusProperty, 0.0);

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
        isSnapshotInverted = source.IsInverted;
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
    }
}