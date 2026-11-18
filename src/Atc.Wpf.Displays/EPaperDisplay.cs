namespace Atc.Wpf.Displays;

/// <summary>
/// An e-paper display module (an SSD1680-style black/white or black/white/red panel such as the 2.9" 128 x 296 or the 2.13" 122 x 250)
/// drawn pixel by pixel, so a screen can mirror the e-paper of an ESP32 or Raspberry Pi project exactly, including its refresh flash.
/// </summary>
/// <remarks>
/// <para>
/// The control shows an <see cref="EPaperFrameBuffer"/>: the controller RAM and, separately, the shown image, which changes only at a
/// refresh. Writes to the RAM change nothing on the screen. The buffer is the single source of truth: <see cref="Text"/> writes into it,
/// one way. An emulator that drives the same buffer (e.g. one bound through <see cref="Buffer"/>) is shown as it refreshes.
/// </para>
/// <para>
/// <see cref="PixelColumns"/> and <see cref="PixelRows"/> are the native (portrait) size, the orientation of the RAM and of the wire;
/// <see cref="Rotation"/> turns the picture for the screen, so at 90 or 270 the module is <see cref="PixelRows"/> wide.
/// </para>
/// <para>
/// A <see cref="EPaperRefreshKind.Full"/> refresh plays the flash of a real panel when <see cref="ShowRefreshFlash"/> is set and the control is
/// loaded: the new picture inverted (black and red become white, white becomes black), then all white, then the picture, each for a third
/// of <see cref="RefreshFlashDuration"/>. A full refresh during a flash restarts it with the newest picture; a
/// <see cref="EPaperRefreshKind.Partial"/> refresh during a flash does not interrupt it but changes the picture the flash shows and ends on.
/// A partial refresh outside a flash is shown at once.
/// </para>
/// <para>
/// The picture is one bitmap, scaled to the module with nearest-neighbour sampling so every pixel stays sharp. Red is shown only when
/// <see cref="HasRed"/> is set: on a black/white panel the red plane is not an image.
/// Buffer changes may come from any thread; they are drawn on the UI thread, at most once per frame.
/// </para>
/// </remarks>
public sealed partial class EPaperDisplay : FrameworkElement
{
    private const int MinPixelSize = 1;
    private const int QuarterTurn = 90;
    private const int HalfTurn = 180;
    private const int ThreeQuarterTurn = 270;
    private const int FullTurn = 360;
    private const int FlashPhaseCount = 3;
    private const double BezelCornerRadius = 1.0;
    private const double PanelCornerRadius = 0.5;

    private static readonly TimeSpan MinFlashPhase = TimeSpan.FromTicks(1);
    private static readonly Color DefaultPaperColor = Color.FromRgb(0xE8, 0xE6, 0xDF);
    private static readonly Color DefaultInkColor = Color.FromRgb(0x1A, 0x1A, 0x1A);
    private static readonly Color DefaultRedInkColor = Color.FromRgb(0xB2, 0x22, 0x22);

    private readonly EPaperFrameBuffer ownBuffer;
    private readonly Action updateAction;

    /// <summary>
    /// Gets or sets the colour of the paper (white pixels and the panel); the colour of a <see cref="SolidColorBrush"/> is used for the pixels,
    /// any other brush gives them the default #E8E6DF.
    /// </summary>
    [DependencyProperty(
        DefaultValueExpression = "CreateFrozenBrush(0xE8, 0xE6, 0xDF)",
        Flags = FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnInkChanged))]
    private Brush? paperBrush;

    /// <summary>
    /// Gets or sets the colour of black pixels; the colour of a <see cref="SolidColorBrush"/> is used, any other brush gives the default #1A1A1A.
    /// </summary>
    [DependencyProperty(
        DefaultValueExpression = "CreateFrozenBrush(0x1A, 0x1A, 0x1A)",
        Flags = FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnInkChanged))]
    private Brush? inkBrush;

    /// <summary>
    /// Gets or sets the colour of red pixels; the colour of a <see cref="SolidColorBrush"/> is used, any other brush gives the default #B22222.
    /// </summary>
    [DependencyProperty(
        DefaultValueExpression = "CreateFrozenBrush(0xB2, 0x22, 0x22)",
        Flags = FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnInkChanged))]
    private Brush? redInkBrush;

    /// <summary>The native (portrait) number of pixel columns of the panel, 1 to 400; 128 for the 2.9" and the 2.13" panels (the 2.13" glass shows 122 of them: set <see cref="VisibleColumns"/>).</summary>
    [DependencyProperty(
        DefaultValue = 128,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnPixelSizeChanged),
        CoerceValueCallback = nameof(CoercePixelSize))]
    private int pixelColumns;

    /// <summary>The native (portrait) number of pixel rows of the panel, 1 to 400; 296 for the 2.9" panel, 250 for the 2.13" panel.</summary>
    [DependencyProperty(
        DefaultValue = 296,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnPixelSizeChanged),
        CoerceValueCallback = nameof(CoercePixelSize))]
    private int pixelRows;

    /// <summary>
    /// The number of native columns the glass shows, counted from column 0; <see langword="null"/> (the default) shows all
    /// <see cref="PixelColumns"/>. Other values are coerced to 1 to <see cref="PixelColumns"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The 2.13" panels have 122 visible columns in a 128-bit-wide RAM row (§SSD1680.1): GxEPD2 (<c>GxEPD2_Type::WIDTH</c> = 128) and
    /// Adafruit_EPD (the 122 short side rounded up to 128) both keep a 128-pixel-wide native buffer with the picture in columns 0 to 121
    /// and bits 122 to 127 as padding, so set <see cref="PixelColumns"/> = 128 (the width of the frames they send) and
    /// <see cref="VisibleColumns"/> = 122.
    /// </para>
    /// <para>
    /// The buffer stays <see cref="PixelColumns"/> wide; the columns past <see cref="VisibleColumns"/> are cropped before the picture is
    /// turned, and <see cref="Rotation"/> and <see cref="Text"/> work on the visible width, as GxEPD2's <c>drawPixel</c> does
    /// (its rotation uses the visible width the graphics base class was constructed with): a <c>setRotation(1)</c> pixel (x, y) at native
    /// (121 - y, x) shows at screen (x, y).
    /// </para>
    /// </remarks>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnVisibleColumnsChanged),
        CoerceValueCallback = nameof(CoerceVisibleColumns))]
    private int? visibleColumns;

    /// <summary>Whether the panel is black/white/red; a black/white panel never shows the red plane.</summary>
    [DependencyProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnHasRedChanged))]
    private bool hasRed;

    /// <summary>
    /// How the native picture is turned for the screen, in degrees: 0, 90, 180 or 270, the same as GxEPD2 <c>setRotation(Rotation / 90)</c>.
    /// Other values are coerced to the nearest quarter turn, modulo 360 (e.g. -90 becomes 270).
    /// </summary>
    /// <remarks>
    /// <para>
    /// A screen pixel (x, y) shows the native pixel GxEPD2's <c>drawPixel</c> writes for it, with W the shown columns
    /// (<see cref="VisibleColumns"/>, or <see cref="PixelColumns"/> when it is not set):
    /// 0: (x, y); 90: (W - 1 - y, x); 180: (W - 1 - x, <see cref="PixelRows"/> - 1 - y); 270: (y, <see cref="PixelRows"/> - 1 - x). In words, the screen shows the native picture turned counter-clockwise by
    /// <see cref="Rotation"/> degrees (a sketch's picture is turned clockwise by it on its way into the RAM). The default 90 shows a sketch that
    /// calls GxEPD2 <c>setRotation(1)</c>, the usual landscape mounting, upright.
    /// </para>
    /// <para>
    /// Adafruit_EPD constructs the panel landscape, and its rotation 0 equals GxEPD2 <c>setRotation(3)</c>, 180 degrees from
    /// <c>setRotation(1)</c>: use 270 for an Adafruit_EPD sketch at its default rotation 0.
    /// </para>
    /// </remarks>
    [DependencyProperty(
        DefaultValue = 90,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnRotationChanged),
        CoerceValueCallback = nameof(CoerceRotation))]
    private int rotation;

    /// <summary>Whether a full refresh plays the flash of a real panel (inverted, white, then the picture); when not set, it is shown at once.</summary>
    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnShowRefreshFlashChanged))]
    private bool showRefreshFlash;

    /// <summary>The length of the whole refresh flash, split into three equal phases; zero or less shows a full refresh at once.</summary>
    [DependencyProperty(DefaultValue = "TimeSpan.FromMilliseconds(600)")]
    private TimeSpan refreshFlashDuration;

    /// <summary>Whether a bezel (a dark frame) is drawn around the panel.</summary>
    [DependencyProperty(
        DefaultValue = true,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender)]
    private bool showBezel;

    /// <summary>The colour of the bezel.</summary>
    [DependencyProperty(
        DefaultValue = nameof(Brushes.Black),
        Flags = FrameworkPropertyMetadataOptions.AffectsRender)]
    private Brush? bezelBrush;

    /// <summary>How the module fills the size of the control.</summary>
    [DependencyProperty(
        DefaultValue = Stretch.Uniform,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender)]
    private Stretch stretch;

    /// <summary>
    /// The text to show: the RAM is cleared to white with no red, the text is drawn in black from the top-left of the screen (as turned by
    /// <see cref="Rotation"/>, so it reads upright) with the 5 x 8 glyphs of the A00 <see cref="CharacterRom"/>, a glyph 6 pixels wide and
    /// lines separated by <c>\n</c> 8 rows apart (<c>\r</c> is ignored), and the panel gets a full refresh. The part outside the panel is clipped.
    /// </summary>
    /// <remarks>
    /// It is one-way: changes made to the buffer by other means are not reflected back.
    /// </remarks>
    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnTextChanged))]
    private string text;

    /// <summary>
    /// The panel to show. The control creates its own; set one (e.g. bound to a view model that runs an emulator) to show it.
    /// A supplied buffer is resized to <see cref="PixelColumns"/> by <see cref="PixelRows"/> and gets <see cref="HasRed"/>;
    /// <see langword="null"/> returns to the control's own buffer.
    /// </summary>
    /// <remarks>
    /// A newly assigned buffer takes over a non-empty <see cref="Text"/>, whatever the order the two were set in; an empty text leaves the buffer's picture alone.
    /// </remarks>
    [DependencyProperty(
        PropertyChangedCallback = nameof(OnBufferChanged),
        CoerceValueCallback = nameof(CoerceBuffer))]
    private EPaperFrameBuffer buffer;

    private EPaperFrameBuffer? subscribedBuffer;
    private WriteableBitmap? bitmap;
    private byte[] blackPlane = [];
    private byte[] redPlane = [];
    private int[] screenPixels = [];
    private bool isBufferNew = true;
    private long lastFullRefreshCount;
    private int isUpdatePending;
    private FlashPhase flashPhase;
    private ITimer? flashTimer;
    private int flashGeneration;
    private DotMatrixGeometry? geometry;
    private LayoutKey cachedKey;
    private DotMatrixLayout? layout;

    /// <summary>
    /// Initializes a new instance of the <see cref="EPaperDisplay"/> class: a black/white 128 x 296 (2.9") panel in landscape.
    /// </summary>
    public EPaperDisplay()
    {
        updateAction = OnUpdateRequested;
        ownBuffer = new EPaperFrameBuffer(PixelColumns, PixelRows, HasRed);
        RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        Unloaded += OnUnloaded;

        CoerceValue(BufferProperty);
    }

    private enum FlashPhase
    {
        None,
        Inverted,
        White,
    }

    /// <summary>Gets or sets the clock of the refresh flash; tests only.</summary>
    internal TimeProvider FlashTimeProvider { get; set; } = TimeProvider.System;

    /// <summary>Gets or sets an action run after the picture was copied into the bitmap, once per copy; tests only.</summary>
    internal Action? FrameCopiedForTests { get; set; }

    /// <summary>
    /// Draws the text into a black plane the size of the panel: every bit white except the glyph dots, placed in screen coordinates
    /// and mapped to native ones by the rotation (GxEPD2 <c>drawPixel</c>).
    /// </summary>
    /// <param name="value">The text.</param>
    /// <param name="width">The native width.</param>
    /// <param name="height">The native height.</param>
    /// <param name="rotation">The rotation, 0, 90, 180 or 270.</param>
    /// <param name="shownColumns">The native columns the glass shows (1 to <paramref name="width"/>); the rotation and the clipping use them.</param>
    /// <returns>The black plane, <c>ceil(width / 8)</c> bytes per row.</returns>
    internal static byte[] RasterizeText(
        string value,
        int width,
        int height,
        int rotation,
        int shownColumns)
    {
        var plane = new byte[((width + 7) / 8) * height];
        Array.Fill(plane, byte.MaxValue);
        var sink = new TextSink(plane, width, height, rotation, shownColumns);
        DisplayRasterizer.DrawText(ref sink, 0, 0, value, CharacterRom.A00, scale: 1);
        return plane;
    }

    /// <summary>
    /// Copies the shown picture into the bitmap for the current flash phase, turned by the rotation; starts the flash when a full
    /// refresh happened since the last copy. UI thread only.
    /// </summary>
    /// <returns><see langword="true"/> when the bitmap was recreated.</returns>
    internal bool UpdateBitmap()
    {
        var snapshot = Buffer.CopyShown(ref blackPlane, ref redPlane);
        if (isBufferNew)
        {
            // Refreshes made before the buffer was shown are not played.
            isBufferNew = false;
            lastFullRefreshCount = snapshot.FullRefreshCount;
        }
        else if (snapshot.FullRefreshCount != lastFullRefreshCount)
        {
            lastFullRefreshCount = snapshot.FullRefreshCount;
            if (CanFlash())
            {
                StartFlash();
            }
        }

        var quarterTurn = NormalizeRotation(Rotation);
        var isSideways = quarterTurn is QuarterTurn or ThreeQuarterTurn;
        var shownColumns = GetShownColumns(snapshot.Width);
        var screenWidth = isSideways ? snapshot.Height : shownColumns;
        var screenHeight = isSideways ? shownColumns : snapshot.Height;
        var isRecreated = false;
        if (bitmap is null ||
            bitmap.PixelWidth != screenWidth ||
            bitmap.PixelHeight != screenHeight)
        {
            bitmap = new WriteableBitmap(screenWidth, screenHeight, 96, 96, PixelFormats.Bgr32, palette: null);
            isRecreated = true;
        }

        if (screenPixels.Length != screenWidth * screenHeight)
        {
            screenPixels = new int[screenWidth * screenHeight];
        }

        FillScreen(snapshot, quarterTurn, shownColumns, screenWidth, screenHeight);

        bitmap.Lock();
        try
        {
            var stride = bitmap.BackBufferStride;
            var backBuffer = bitmap.BackBuffer;
            for (var y = 0; y < screenHeight; y++)
            {
                Marshal.Copy(screenPixels, y * screenWidth, backBuffer + (nint)((long)y * stride), screenWidth);
            }

            bitmap.AddDirtyRect(new Int32Rect(0, 0, screenWidth, screenHeight));
        }
        finally
        {
            bitmap.Unlock();
        }

        FrameCopiedForTests?.Invoke();
        return isRecreated;
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new EPaperDisplayAutomationPeer(this);

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
        => DotMatrixLayout.Measure(GetGeometry(), ShowBezel, availableSize, Stretch);

    /// <inheritdoc />
    protected override void OnRender(DrawingContext drawingContext)
    {
        ArgumentNullException.ThrowIfNull(drawingContext);

        base.OnRender(drawingContext);

        if (bitmap is null)
        {
            UpdateBitmap();
        }

        var moduleLayout = EnsureLayout();
        if (moduleLayout.IsEmpty)
        {
            return;
        }

        var bounds = new Rect(RenderSize);
        var isClipped = !bounds.Contains(moduleLayout.ModuleRect);
        if (isClipped)
        {
            drawingContext.PushClip(new RectangleGeometry(bounds));
        }

        DrawPanel(drawingContext, moduleLayout);
        if (bitmap is not null &&
            bitmap.PixelWidth == moduleLayout.DotColumns &&
            bitmap.PixelHeight == moduleLayout.DotRows)
        {
            var pixelsRect = new Rect(
                moduleLayout.GetDotRect(0, 0).TopLeft,
                moduleLayout.GetDotRect(moduleLayout.DotColumns - 1, moduleLayout.DotRows - 1).BottomRight);
            drawingContext.DrawImage(bitmap, pixelsRect);
        }

        if (isClipped)
        {
            drawingContext.Pop();
        }
    }

    private static SolidColorBrush CreateFrozenBrush(
        byte red,
        byte green,
        byte blue)
    {
        var brush = new SolidColorBrush(Color.FromRgb(red, green, blue));
        brush.Freeze();
        return brush;
    }

    private static int ToBgr32(
        Brush? brush,
        Color fallback)
    {
        var color = brush is SolidColorBrush solid ? solid.Color : fallback;
        return (color.R << 16) | (color.G << 8) | color.B;
    }

    private static int NormalizeRotation(int value)
    {
        var quarters = (int)System.Math.Round(value / (double)QuarterTurn, MidpointRounding.AwayFromZero);
        var degrees = (quarters * QuarterTurn) % FullTurn;
        return degrees < 0 ? degrees + FullTurn : degrees;
    }

    // Screen (x, y) to native (x, y), as GxEPD2's drawPixel maps a user pixel for setRotation(rotation / 90).
    private static void ToNative(
        int rotation,
        int width,
        int height,
        int x,
        int y,
        out int nativeX,
        out int nativeY)
    {
        switch (rotation)
        {
            case QuarterTurn:
                nativeX = width - 1 - y;
                nativeY = x;
                break;
            case HalfTurn:
                nativeX = width - 1 - x;
                nativeY = height - 1 - y;
                break;
            case ThreeQuarterTurn:
                nativeX = y;
                nativeY = height - 1 - x;
                break;
            default:
                nativeX = x;
                nativeY = y;
                break;
        }
    }

    private static object CoercePixelSize(
        DependencyObject d,
        object? value)
        => System.Math.Clamp((int)value!, MinPixelSize, EPaperFrameBuffer.MaxWidth);

    private static object CoerceRotation(
        DependencyObject d,
        object? value)
        => NormalizeRotation((int)value!);

    private static object? CoerceVisibleColumns(
        DependencyObject d,
        object? value)
        => value is int columns
            ? System.Math.Clamp(columns, MinPixelSize, ((EPaperDisplay)d).PixelColumns)
            : null;

    private static void OnVisibleColumnsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (EPaperDisplay)d;

        // The text is drawn against the visible width, so a new width draws it again; an empty text is not re-applied.
        if (!string.IsNullOrEmpty(display.Text))
        {
            display.ApplyText();
        }

        display.RequestUpdate();
    }

    private static object CoerceBuffer(
        DependencyObject d,
        object? value)
        => value ?? ((EPaperDisplay)d).ownBuffer;

    private static void OnPixelSizeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (EPaperDisplay)d;
        display.Buffer.Resize(display.PixelColumns, display.PixelRows);
        display.CoerceValue(VisibleColumnsProperty);

        // Text past the old size was clipped, so a new size draws the text again.
        // An empty text is not re-applied, so a resize never blanks an emulator's content.
        if (!string.IsNullOrEmpty(display.Text))
        {
            display.ApplyText();
        }
    }

    private static void OnHasRedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (EPaperDisplay)d;
        display.Buffer.HasRed = display.HasRed;
    }

    private static void OnRotationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (EPaperDisplay)d;

        // The text is placed in screen coordinates, so it is drawn again to read upright.
        if (!string.IsNullOrEmpty(display.Text))
        {
            display.ApplyText();
        }

        display.RequestUpdate();
    }

    private static void OnInkChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((EPaperDisplay)d).RequestUpdate();

    private static void OnShowRefreshFlashChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (EPaperDisplay)d;
        if (!display.ShowRefreshFlash)
        {
            display.StopFlash();
        }
    }

    private static void OnTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((EPaperDisplay)d).ApplyText();

    private static void OnBufferChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (EPaperDisplay)d;

        if (e.OldValue is EPaperFrameBuffer oldBuffer)
        {
            WeakEventManager<EPaperFrameBuffer, EventArgs>.RemoveHandler(
                oldBuffer,
                nameof(EPaperFrameBuffer.Changed),
                display.OnBufferContentChanged);
            WeakEventManager<EPaperFrameBuffer, EPaperRefreshedEventArgs>.RemoveHandler(
                oldBuffer,
                nameof(EPaperFrameBuffer.Refreshed),
                display.OnBufferRefreshed);
        }

        if (e.NewValue is EPaperFrameBuffer newBuffer)
        {
            Volatile.Write(ref display.subscribedBuffer, newBuffer);
            WeakEventManager<EPaperFrameBuffer, EventArgs>.AddHandler(
                newBuffer,
                nameof(EPaperFrameBuffer.Changed),
                display.OnBufferContentChanged);
            WeakEventManager<EPaperFrameBuffer, EPaperRefreshedEventArgs>.AddHandler(
                newBuffer,
                nameof(EPaperFrameBuffer.Refreshed),
                display.OnBufferRefreshed);

            // The new buffer's refresh count starts the comparison; its earlier refreshes are not played.
            display.isBufferNew = true;
            display.StopFlash();

            using (newBuffer.BeginBatch())
            {
                newBuffer.Resize(display.PixelColumns, display.PixelRows);
                newBuffer.HasRed = display.HasRed;

                // A buffer that arrives after the text (e.g. through a binding that resolves late) takes it over.
                if (!string.IsNullOrEmpty(display.Text))
                {
                    display.ApplyText();
                }
            }
        }

        display.RequestUpdate();
    }

    private void FillScreen(
        EPaperShownSnapshot snapshot,
        int quarterTurn,
        int shownColumns,
        int screenWidth,
        int screenHeight)
    {
        var paper = ToBgr32(PaperBrush, DefaultPaperColor);
        var ink = ToBgr32(InkBrush, DefaultInkColor);
        var redInk = ToBgr32(RedInkBrush, DefaultRedInkColor);
        var pixels = screenPixels;

        if (flashPhase == FlashPhase.White)
        {
            Array.Fill(pixels, paper);
            return;
        }

        // Inverted: white becomes black, black and red become white.
        var isInverted = flashPhase == FlashPhase.Inverted;
        var whiteValue = isInverted ? ink : paper;
        var blackValue = isInverted ? paper : ink;
        var redValue = isInverted ? paper : redInk;
        var isRedShown = snapshot.HasRed;
        var nativeWidth = snapshot.Width;
        var nativeHeight = snapshot.Height;
        var bytesPerRow = (nativeWidth + 7) / 8;
        var black = blackPlane;
        var red = redPlane;

        var index = 0;
        for (var y = 0; y < screenHeight; y++)
        {
            for (var x = 0; x < screenWidth; x++)
            {
                // The rotation works on the shown columns (0 .. shownColumns - 1); the rows keep the full native stride.
                ToNative(quarterTurn, shownColumns, nativeHeight, x, y, out var nativeX, out var nativeY);
                var byteIndex = (nativeY * bytesPerRow) + (nativeX >> 3);
                var mask = 0x80 >> (nativeX & 7);
                if (isRedShown && (red[byteIndex] & mask) != 0)
                {
                    pixels[index++] = redValue;
                }
                else
                {
                    pixels[index++] = (black[byteIndex] & mask) == 0 ? blackValue : whiteValue;
                }
            }
        }
    }

    private void ApplyText()
    {
        var target = Buffer;
        var value = Text ?? string.Empty;

        using (target.BeginBatch())
        {
            // Read the size inside the batch; the RAM and the size are set by this thread only.
            var width = target.Width;
            var height = target.Height;
            var black = RasterizeText(value, width, height, NormalizeRotation(Rotation), GetShownColumns(width));
            target.LoadRam(black, new byte[black.Length]);
            target.Refresh(EPaperRefreshKind.Full);
        }
    }

    private bool CanFlash()
        => ShowRefreshFlash &&
           IsLoaded &&
           RefreshFlashDuration > TimeSpan.Zero;

    private void StartFlash()
    {
        flashTimer?.Dispose();
        flashPhase = FlashPhase.Inverted;
        var generation = ++flashGeneration;

        // At least one tick: a zero period makes a one-shot timer, which would stop the flash on its white phase.
        var phase = RefreshFlashDuration / FlashPhaseCount;
        if (phase < MinFlashPhase)
        {
            phase = MinFlashPhase;
        }

        flashTimer = FlashTimeProvider.CreateTimer(OnFlashTimerTick, generation, phase, phase);
    }

    private void StopFlash()
    {
        flashTimer?.Dispose();
        flashTimer = null;
        flashGeneration++;
        if (flashPhase == FlashPhase.None)
        {
            return;
        }

        flashPhase = FlashPhase.None;
        RequestUpdate();
    }

    // Raised on a timer thread (the thread that advances the clock in tests).
    private void OnFlashTimerTick(object? state)
    {
        var generation = (int)state!;
        var dispatcher = Dispatcher;
        if (dispatcher.CheckAccess())
        {
            AdvanceFlash(generation);
        }
        else if (!dispatcher.HasShutdownStarted)
        {
            _ = dispatcher.BeginInvoke(DispatcherPriority.Render, () => AdvanceFlash(generation));
        }
    }

    private void AdvanceFlash(int generation)
    {
        // A tick of a flash that was restarted or stopped since is stale.
        if (generation != flashGeneration ||
            flashPhase == FlashPhase.None)
        {
            return;
        }

        if (flashPhase == FlashPhase.Inverted)
        {
            flashPhase = FlashPhase.White;
        }
        else
        {
            flashTimer?.Dispose();
            flashTimer = null;
            flashPhase = FlashPhase.None;
        }

        RequestUpdate();
    }

    // Safe from any thread; at most one update is pending at a time and it runs on the UI thread at render priority.
    private void RequestUpdate()
    {
        if (Interlocked.CompareExchange(ref isUpdatePending, 1, 0) != 0)
        {
            return;
        }

        var dispatcher = Dispatcher;
        if (dispatcher.HasShutdownStarted)
        {
            Interlocked.Exchange(ref isUpdatePending, 0);
            return;
        }

        _ = dispatcher.BeginInvoke(DispatcherPriority.Render, updateAction);
    }

    private void OnUpdateRequested()
    {
        // Cleared first: a change that lands while the picture is copied requests the next update.
        Interlocked.Exchange(ref isUpdatePending, 0);
        if (UpdateBitmap())
        {
            InvalidateVisual();
        }
    }

    // Raised on the thread that changed the buffer.
    private void OnBufferContentChanged(
        object? sender,
        EventArgs e)
    {
        if (sender is not EPaperFrameBuffer source ||
            !ReferenceEquals(source, Volatile.Read(ref subscribedBuffer)))
        {
            return;
        }

        RequestUpdate();
    }

    // Raised on the thread that refreshed the buffer; a full refresh of an unchanged picture still plays the flash.
    private void OnBufferRefreshed(
        object? sender,
        EPaperRefreshedEventArgs e)
    {
        if (sender is not EPaperFrameBuffer source ||
            !ReferenceEquals(source, Volatile.Read(ref subscribedBuffer)))
        {
            return;
        }

        RequestUpdate();
    }

    // IsLoaded is already false inside a genuine Unloaded, and stays true when a re-parented control's old Unloaded arrives late.
    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        if (!IsLoaded)
        {
            StopFlash();
        }
    }

    private int GetShownColumns(int nativeWidth)
        => VisibleColumns is { } columns
            ? System.Math.Clamp(columns, MinPixelSize, nativeWidth)
            : nativeWidth;

    private DotMatrixGeometry GetGeometry()
    {
        var isSideways = NormalizeRotation(Rotation) is QuarterTurn or ThreeQuarterTurn;
        var shownColumns = GetShownColumns(PixelColumns);
        var columnCount = isSideways ? PixelRows : shownColumns;
        var rowCount = isSideways ? shownColumns : PixelRows;
        if (geometry is null ||
            geometry.DotColumns != columnCount ||
            geometry.DotRows != rowCount)
        {
            geometry = new DotMatrixGeometry(
                columnCount,
                rowCount,
                CellWidth: 0,
                CellHeight: 0,
                DotGap: 0,
                CellGapX: 0,
                CellGapY: 0);
        }

        return geometry;
    }

    private DotMatrixLayout EnsureLayout()
    {
        var key = new LayoutKey(GetGeometry(), RenderSize, Stretch, ShowBezel);
        if (layout is not null && key == cachedKey)
        {
            return layout;
        }

        cachedKey = key;
        layout = DotMatrixLayout.Create(key.Geometry, key.ShowBezel, key.Size, key.Stretch);
        return layout;
    }

    private void DrawPanel(
        DrawingContext drawingContext,
        DotMatrixLayout moduleLayout)
    {
        var scale = System.Math.Min(moduleLayout.ScaleX, moduleLayout.ScaleY);

        if (ShowBezel && BezelBrush is { } bezel)
        {
            drawingContext.DrawRoundedRectangle(
                bezel,
                pen: null,
                moduleLayout.ModuleRect,
                BezelCornerRadius * scale,
                BezelCornerRadius * scale);
        }

        drawingContext.DrawRoundedRectangle(
            PaperBrush,
            pen: null,
            moduleLayout.PanelRect,
            PanelCornerRadius * scale,
            PanelCornerRadius * scale);
    }

    // Clears the black bit of each glyph dot: screen coordinates, clipped to the turned panel, mapped to native ones.
    private readonly struct TextSink : IPixelSink
    {
        private readonly byte[] plane;
        private readonly int shownColumns;
        private readonly int nativeHeight;
        private readonly int quarterTurn;
        private readonly int screenWidth;
        private readonly int screenHeight;
        private readonly int bytesPerRow;

        public TextSink(
            byte[] plane,
            int width,
            int height,
            int quarterTurn,
            int shownColumns)
        {
            this.plane = plane;
            this.shownColumns = shownColumns;
            nativeHeight = height;
            this.quarterTurn = quarterTurn;
            var isSideways = quarterTurn is QuarterTurn or ThreeQuarterTurn;
            screenWidth = isSideways ? height : shownColumns;
            screenHeight = isSideways ? shownColumns : height;
            bytesPerRow = (width + 7) / 8;
        }

        public bool Plot(
            long x,
            long y)
        {
            if (x < 0 || y < 0 || x >= screenWidth || y >= screenHeight)
            {
                return false;
            }

            ToNative(quarterTurn, shownColumns, nativeHeight, (int)x, (int)y, out var nativeX, out var nativeY);
            ref var target = ref plane[(nativeY * bytesPerRow) + (nativeX >> 3)];
            var mask = (byte)(0x80 >> (nativeX & 7));
            if ((target & mask) == 0)
            {
                return false;
            }

            target = (byte)(target & ~mask);
            return true;
        }

        public bool Fill(
            long x,
            long y,
            long width,
            long height)
        {
            if (!DisplayRasterizer.TryClip(x, y, width, height, screenWidth, screenHeight, out var firstX, out var lastX, out var firstY, out var lastY))
            {
                return false;
            }

            var changed = false;
            for (var row = firstY; row <= lastY; row++)
            {
                for (var column = firstX; column <= lastX; column++)
                {
                    changed |= Plot(column, row);
                }
            }

            return changed;
        }
    }

    private readonly record struct LayoutKey(
        DotMatrixGeometry Geometry,
        Size Size,
        Stretch Stretch,
        bool ShowBezel);
}