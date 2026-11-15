namespace Atc.Wpf.Controls.Displays;

/// <summary>
/// A colour graphic display module (a TFT LCD such as the ST7735, ST7789 or ILI9341, or a colour OLED such as the SSD1331 or SSD1351,
/// up to 480 x 480 pixels) drawn pixel by pixel, so a screen can mirror the display of an ESP32 or Raspberry Pi project exactly.
/// </summary>
/// <remarks>
/// <para>
/// The control shows a <see cref="ColorFrameBuffer"/>, the picture and display state of the chip. The buffer is the single source of truth:
/// <see cref="Text"/> writes into it, one way. An emulator that drives the same buffer (e.g. one bound through <see cref="Buffer"/>)
/// is shown as it writes.
/// </para>
/// <para>
/// The picture is one bitmap, scaled to the module with nearest-neighbour sampling so every pixel stays sharp. Only the rows that changed
/// are copied into it. The buffer's <see cref="ColorFrameBuffer.IsInverted"/> inverts every colour; a <see cref="ColorPanelKind.Tft"/>
/// panel lifts black to the faint glow of its backlight (<c>c' = 14 + c * 241 / 255</c> per channel), while a <see cref="ColorPanelKind.Oled"/>
/// keeps true black. <see cref="ColorFrameBuffer.Brightness"/> is shown as the opacity of the picture over the panel, and a display that is off
/// or sleeping shows the panel only.
/// </para>
/// <para>
/// When one pixel is at least 4 device-independent units, <see cref="ShowPixelGrid"/> draws thin panel-coloured gaps between the pixels.
/// </para>
/// <para>
/// Buffer changes may come from any thread; they are drawn on the UI thread, at most once per frame.
/// A buffer is meant to be shown by one control at a time: the rows that changed are handed to the first control that asks for them.
/// </para>
/// </remarks>
public sealed partial class ColorGraphicDisplay : FrameworkElement
{
    private const int MinPixelSize = 1;
    private const int MaxCopyAttempts = 4;
    private const int RgbMask = 0x00FFFFFF;
    private const byte BacklightLevel = 14;
    private const double GridGap = 0.12;
    private const double MinGridPixelSize = 4.0;
    private const double BezelCornerRadius = 1.0;
    private const double PanelCornerRadius = 0.5;
    private const double MaxBrightness = 255.0;
    private static readonly SolidColorBrush TftPanelBrush = CreateFrozenBrush(Color.FromRgb(BacklightLevel, BacklightLevel, BacklightLevel));
    private static readonly SolidColorBrush OledPanelBrush = CreateFrozenBrush(Colors.Black);
    private static readonly byte[] BacklightTable = CreateBacklightTable();

    private readonly ColorFrameBuffer ownBuffer;
    private readonly Action updateAction;

    /// <summary>The number of pixel columns of the module, 1 to 480.</summary>
    [DependencyProperty(
        DefaultValue = 128,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnPixelSizeChanged),
        CoerceValueCallback = nameof(CoercePixelSize))]
    private int pixelColumns;

    /// <summary>The number of pixel rows of the module, 1 to 480.</summary>
    [DependencyProperty(
        DefaultValue = 160,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnPixelSizeChanged),
        CoerceValueCallback = nameof(CoercePixelSize))]
    private int pixelRows;

    /// <summary>The kind of panel: a <see cref="ColorPanelKind.Tft"/> lifts black to its backlight, an <see cref="ColorPanelKind.Oled"/> keeps true black.</summary>
    [DependencyProperty(
        DefaultValue = ColorPanelKind.Tft,
        Flags = FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnPanelKindChanged))]
    private ColorPanelKind panelKind;

    /// <summary>Whether thin panel-coloured gaps are drawn between the pixels; they are drawn only when one pixel is at least 4 device-independent units.</summary>
    [DependencyProperty(
        DefaultValue = true,
        Flags = FrameworkPropertyMetadataOptions.AffectsRender)]
    private bool showPixelGrid;

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
    /// The text to show: the buffer is cleared to black, then the text is drawn from the top-left in <see cref="TextColor"/> with the 5 x 8 glyphs
    /// of <see cref="CharacterRom"/>, a glyph 6 pixels wide and lines separated by <c>\n</c> 8 rows apart (<c>\r</c> is ignored).
    /// The part outside the module is clipped.
    /// </summary>
    /// <remarks>
    /// It is one-way: changes made to the buffer by other means are not reflected back.
    /// </remarks>
    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnTextChanged))]
    private string text;

    /// <summary>The colour of the glyphs of <see cref="Text"/>.</summary>
    [DependencyProperty(
        DefaultValue = nameof(Colors.White),
        PropertyChangedCallback = nameof(OnTextStyleChanged))]
    private Color textColor;

    /// <summary>The character ROM that maps the characters of <see cref="Text"/> to glyphs.</summary>
    [DependencyProperty(
        DefaultValue = CharacterRom.A00,
        PropertyChangedCallback = nameof(OnTextStyleChanged))]
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
    private ColorFrameBuffer buffer;

    private ColorFrameBuffer? subscribedBuffer;
    private WriteableBitmap? bitmap;
    private int[] scratchRow = [];
    private bool isFullCopyRequested = true;
    private bool isPictureVisible = true;
    private byte pictureBrightness = byte.MaxValue;
    private int isUpdatePending;
    private DotMatrixGeometry? geometry;
    private LayoutKey cachedKey;
    private DotMatrixLayout? layout;
    private StreamGeometry? grid;
    private DotMatrixLayout? gridLayout;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorGraphicDisplay"/> class: a 128 x 160 TFT.
    /// </summary>
    public ColorGraphicDisplay()
    {
        updateAction = OnUpdateRequested;
        ownBuffer = new ColorFrameBuffer(PixelColumns, PixelRows);
        RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);

        CoerceValue(BufferProperty);
    }

    /// <summary>Gets or sets an action run after changed rows were copied into the bitmap, once per copy; tests only.</summary>
    internal Action? FrameCopiedForTests { get; set; }

    /// <summary>
    /// Copies the rows of the buffer that changed into the bitmap, applying the inversion and the panel. UI thread only.
    /// </summary>
    /// <returns><see langword="true"/> when the drawing must be redone (a new bitmap, or the display state changed).</returns>
    internal bool UpdateBitmap()
    {
        var source = Buffer;
        var hasDirtyRows = source.TryTakeDirtyRows(out var first, out var last);
        var isFull = isFullCopyRequested || bitmap is null;
        if (!hasDirtyRows && !isFull)
        {
            return false;
        }

        isFullCopyRequested = false;

        // Read after the rows were taken: a state change made later marks every row dirty again, so the next update shows it.
        var isInverted = source.IsInverted;
        var isVisible = source.IsDisplayOn && !source.IsSleeping;
        var level = source.Brightness;
        var isTft = PanelKind == ColorPanelKind.Tft;
        var isRedrawNeeded = isVisible != isPictureVisible || level != pictureBrightness;
        isPictureVisible = isVisible;
        pictureBrightness = level;

        // The size and the pixels are read under separate locks; an emulator may resize in between, so recreate the bitmap and copy again.
        for (var attempt = 0; attempt < MaxCopyAttempts; attempt++)
        {
            var width = source.Width;
            var height = source.Height;
            if (bitmap is null ||
                bitmap.PixelWidth != width ||
                bitmap.PixelHeight != height)
            {
                bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, palette: null);
                isFull = true;
                isRedrawNeeded = true;
            }

            var copyFirst = isFull ? 0 : System.Math.Max(0, first);
            var copyLast = isFull ? height - 1 : System.Math.Min(last, height - 1);
            if (copyFirst > copyLast)
            {
                return isRedrawNeeded;
            }

            if (TryCopy(source, bitmap, copyFirst, copyLast, isInverted, isTft))
            {
                FrameCopiedForTests?.Invoke();
                return isRedrawNeeded;
            }

            bitmap = null;
        }

        isFullCopyRequested = true;
        RequestUpdate();
        return isRedrawNeeded;
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new ColorGraphicDisplayAutomationPeer(this);

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
        => DotMatrixLayout.Measure(GetGeometry(), ShowBezel, availableSize, Stretch);

    /// <inheritdoc />
    protected override void OnRender(DrawingContext drawingContext)
    {
        ArgumentNullException.ThrowIfNull(drawingContext);

        base.OnRender(drawingContext);

        UpdateBitmap();
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

        var panelBrush = PanelKind == ColorPanelKind.Tft ? TftPanelBrush : OledPanelBrush;
        DrawPanel(drawingContext, moduleLayout, panelBrush);
        if (isPictureVisible && pictureBrightness > 0 && bitmap is not null)
        {
            DrawPicture(drawingContext, moduleLayout, panelBrush, bitmap);
        }

        if (isClipped)
        {
            drawingContext.Pop();
        }
    }

    private static SolidColorBrush CreateFrozenBrush(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }

    private static byte[] CreateBacklightTable()
    {
        var table = new byte[256];
        for (var i = 0; i < table.Length; i++)
        {
            table[i] = (byte)(BacklightLevel + (i * (255 - BacklightLevel) / 255));
        }

        return table;
    }

    private static void AddRectangle(
        StreamGeometryContext context,
        Rect rect)
    {
        context.BeginFigure(rect.TopLeft, isFilled: true, isClosed: true);
        context.LineTo(rect.TopRight, isStroked: false, isSmoothJoin: false);
        context.LineTo(rect.BottomRight, isStroked: false, isSmoothJoin: false);
        context.LineTo(rect.BottomLeft, isStroked: false, isSmoothJoin: false);
    }

    private static object CoercePixelSize(
        DependencyObject d,
        object? value)
        => System.Math.Clamp((int)value!, MinPixelSize, ColorFrameBuffer.MaxSize);

    private static object CoerceBuffer(
        DependencyObject d,
        object? value)
        => value ?? ((ColorGraphicDisplay)d).ownBuffer;

    private static void OnPixelSizeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (ColorGraphicDisplay)d;
        display.Buffer.Resize(display.PixelColumns, display.PixelRows);

        // Text past the old size was clipped, so growing the module draws the text again.
        // An empty text is not re-applied, so a resize never blanks an emulator's content.
        if (!string.IsNullOrEmpty(display.Text))
        {
            display.ApplyText();
        }
    }

    private static void OnPanelKindChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        // The backlight is applied in the bitmap, so every row is copied again.
        var display = (ColorGraphicDisplay)d;
        display.isFullCopyRequested = true;
        display.RequestUpdate();
    }

    private static void OnTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((ColorGraphicDisplay)d).ApplyText();

    private static void OnTextStyleChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (ColorGraphicDisplay)d;

        // An empty text is not re-applied, so a colour or ROM change never blanks an emulator's content.
        if (!string.IsNullOrEmpty(display.Text))
        {
            display.ApplyText();
        }
    }

    private static void OnBufferChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (ColorGraphicDisplay)d;

        if (e.OldValue is ColorFrameBuffer oldBuffer)
        {
            WeakEventManager<ColorFrameBuffer, EventArgs>.RemoveHandler(
                oldBuffer,
                nameof(ColorFrameBuffer.Changed),
                display.OnBufferContentChanged);
        }

        if (e.NewValue is ColorFrameBuffer newBuffer)
        {
            Volatile.Write(ref display.subscribedBuffer, newBuffer);
            WeakEventManager<ColorFrameBuffer, EventArgs>.AddHandler(
                newBuffer,
                nameof(ColorFrameBuffer.Changed),
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

        // The rows that changed belong to the buffer, so a new buffer is copied whole.
        display.isFullCopyRequested = true;
        display.RequestUpdate();
    }

    private bool TryCopy(
        ColorFrameBuffer source,
        WriteableBitmap target,
        int first,
        int last,
        bool isInverted,
        bool isTft)
    {
        target.Lock();
        try
        {
            var width = target.PixelWidth;
            var stride = target.BackBufferStride;
            var backBuffer = target.BackBuffer;
            if (!source.TryCopyRows(first, last, backBuffer, stride, width, target.PixelHeight))
            {
                return false;
            }

            if (isInverted || isTft)
            {
                ApplyPanel(backBuffer, stride, width, first, last, isInverted, isTft);
            }

            target.AddDirtyRect(new Int32Rect(0, first, width, last - first + 1));
            return true;
        }
        finally
        {
            target.Unlock();
        }
    }

    // Inverts the colours and lifts black to the backlight in place, row by row through one reused managed row.
    private void ApplyPanel(
        nint backBuffer,
        int stride,
        int width,
        int first,
        int last,
        bool isInverted,
        bool isTft)
    {
        if (scratchRow.Length != width)
        {
            scratchRow = new int[width];
        }

        var row = scratchRow;
        var invertMask = isInverted ? RgbMask : 0;
        var table = BacklightTable;
        for (var y = first; y <= last; y++)
        {
            var address = backBuffer + (nint)((long)y * stride);
            Marshal.Copy(address, row, 0, width);
            for (var x = 0; x < row.Length; x++)
            {
                var value = (row[x] ^ invertMask) & RgbMask;
                if (isTft)
                {
                    value = (table[(value >> 16) & 0xFF] << 16) |
                            (table[(value >> 8) & 0xFF] << 8) |
                            table[value & 0xFF];
                }

                row[x] = value;
            }

            Marshal.Copy(row, 0, address, width);
        }
    }

    private void ApplyText()
    {
        var target = Buffer;
        var value = Text ?? string.Empty;

        using (target.BeginBatch())
        {
            target.Clear(Colors.Black);
            if (value.Length > 0)
            {
                target.DrawText(0, 0, value, TextColor, scale: 1, CharacterRom);
            }
        }
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
        // Cleared first: a change that lands while the rows are copied requests the next update.
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
        if (sender is not ColorFrameBuffer source ||
            !ReferenceEquals(source, Volatile.Read(ref subscribedBuffer)))
        {
            return;
        }

        RequestUpdate();
    }

    private DotMatrixGeometry GetGeometry()
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
        DotMatrixLayout moduleLayout,
        Brush panelBrush)
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
            panelBrush,
            pen: null,
            moduleLayout.PanelRect,
            PanelCornerRadius * scale,
            PanelCornerRadius * scale);
    }

    private void DrawPicture(
        DrawingContext drawingContext,
        DotMatrixLayout moduleLayout,
        Brush panelBrush,
        WriteableBitmap picture)
    {
        var pixelsRect = new Rect(
            moduleLayout.GetDotRect(0, 0).TopLeft,
            moduleLayout.GetDotRect(moduleLayout.DotColumns - 1, moduleLayout.DotRows - 1).BottomRight);

        var isDimmed = pictureBrightness < byte.MaxValue;
        if (isDimmed)
        {
            drawingContext.PushOpacity(pictureBrightness / MaxBrightness);
        }

        drawingContext.DrawImage(picture, pixelsRect);

        if (isDimmed)
        {
            drawingContext.Pop();
        }

        if (ShowPixelGrid && GetGrid(moduleLayout) is { } gaps)
        {
            drawingContext.DrawGeometry(panelBrush, pen: null, gaps);
        }
    }

    // One frozen geometry of the gaps between the pixels: a strip per inner column border and per inner row border.
    private StreamGeometry? GetGrid(DotMatrixLayout moduleLayout)
    {
        if (ReferenceEquals(gridLayout, moduleLayout))
        {
            return grid;
        }

        gridLayout = moduleLayout;
        grid = null;
        if (System.Math.Min(moduleLayout.ScaleX, moduleLayout.ScaleY) < MinGridPixelSize)
        {
            return null;
        }

        var gapX = GridGap * moduleLayout.ScaleX;
        var gapY = GridGap * moduleLayout.ScaleY;
        var topLeft = moduleLayout.GetDotRect(0, 0).TopLeft;
        var bottomRight = moduleLayout.GetDotRect(moduleLayout.DotColumns - 1, moduleLayout.DotRows - 1).BottomRight;
        var gaps = new StreamGeometry { FillRule = FillRule.Nonzero };
        using (var context = gaps.Open())
        {
            for (var x = 1; x < moduleLayout.DotColumns; x++)
            {
                var border = moduleLayout.GetDotRect(x, 0).X;
                AddRectangle(context, new Rect(border - (gapX / 2), topLeft.Y, gapX, bottomRight.Y - topLeft.Y));
            }

            for (var y = 1; y < moduleLayout.DotRows; y++)
            {
                var border = moduleLayout.GetDotRect(0, y).Y;
                AddRectangle(context, new Rect(topLeft.X, border - (gapY / 2), bottomRight.X - topLeft.X, gapY));
            }
        }

        gaps.Freeze();
        grid = gaps;
        return grid;
    }

    private readonly record struct LayoutKey(
        DotMatrixGeometry Geometry,
        Size Size,
        Stretch Stretch,
        bool ShowBezel);
}