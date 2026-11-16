namespace Atc.Wpf.Displays;

/// <summary>
/// A chain of 8 x 8 LED matrix modules driven by MAX7219 chips (the FC-16 style 32 x 8 displays and their relatives) drawn LED by LED,
/// so a screen can mirror the display of an ESP32 or Raspberry Pi project exactly.
/// </summary>
/// <remarks>
/// <para>
/// The control shows a <see cref="Max7219ChainBuffer"/>, the registers of the chips; device 0, the first in the chain (nearest the
/// microcontroller's DIN), is the rightmost module (§MAX7219.7). <see cref="Wiring"/> says how a module's LEDs are wired to the digit
/// registers. <see cref="Text"/> writes into the buffer, one way.
/// </para>
/// <para>
/// What is lit, per device (§MAX7219.5): display test lights every LED; otherwise a shut-down device is dark; otherwise a digit above
/// the scan limit is dark (on <see cref="Max7219MatrixWiring.Generic"/> modules a digit is a column, on the others a row); otherwise an
/// LED is lit when its bit is set in the byte the chip drives: the register value, or, for a digit whose decode-mode bit is set, its Code B
/// segment pattern with D7 kept (§MAX7219.4), as the chip would show it. Matrix libraries keep decoding off.
/// </para>
/// <para>
/// The control drives <see cref="DotMatrixDisplayBase.Brightness"/> from the intensity of device 0: <c>0.25 + 0.75 * intensity / 15</c>,
/// or 1.0 while device 0 is in display test. The libraries send the same intensity to every device. It is set with
/// <see cref="DependencyObject.SetCurrentValue"/>, so a brightness set on the control is replaced on the next buffer change.
/// </para>
/// <para>
/// The colours come from <see cref="LedColor"/>; a brush, dot shape, glow or ghost opacity set on the control (locally, by a style or a binding)
/// wins over the preset. Buffer changes may come from any thread; they are drawn on the UI thread, at most once per frame.
/// </para>
/// </remarks>
public sealed partial class LedMatrixDisplay : DotMatrixDisplayBase
{
    private const int MinModuleCount = 1;
    private const int ModuleSize = 8;
    private const int TopBit = 7;
    private const double DotGap = 0.25;
    private const double ModuleGap = 1.0;
    private const double RoundDot = 0.5;
    private const double LedGlowRadius = 0.35;
    private const double LedGhostOpacity = 0.6;
    private const double MinIntensityBrightness = 0.25;
    private const double IntensityBrightnessRange = 0.75;
    private const double FullBrightness = 1.0;

    // §LIB.MDMAX72XX.1 begin(): scan limit 7, intensity MAX_INTENSITY / 2.
    private const byte BeginScanLimit = 7;
    private const byte BeginIntensity = 7;

    private readonly Max7219ChainBuffer ownBuffer;

    /// <summary>The number of 8 x 8 modules (MAX7219 devices) in the chain, 1 to 8.</summary>
    [DependencyProperty(
        DefaultValue = 4,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnModuleCountChanged),
        CoerceValueCallback = nameof(CoerceModuleCount))]
    private int moduleCount;

    /// <summary>How the LEDs of a module are wired to the digit registers.</summary>
    [DependencyProperty(
        DefaultValue = Max7219MatrixWiring.Fc16,
        PropertyChangedCallback = nameof(OnWiringChanged))]
    private Max7219MatrixWiring wiring;

    /// <summary>The colour preset of the LEDs.</summary>
    [DependencyProperty(
        DefaultValue = LedColor.Red,
        PropertyChangedCallback = nameof(OnLedColorChanged))]
    private LedColor ledColor;

    /// <summary>
    /// The text to show: the digit registers are cleared, then the text is drawn from the top-left with the 5 x 8 glyphs of <see cref="CharacterRom"/>,
    /// a glyph 6 LEDs wide (<c>\r</c> and <c>\n</c> are ignored), through <see cref="Wiring"/>, so the registers hold what that module type would.
    /// The part outside the display is clipped.
    /// </summary>
    /// <remarks>
    /// A non-empty text also sets every device as MD_MAX72XX's <c>begin()</c> does (§LIB.MDMAX72XX.1): display test off, scan limit 7,
    /// intensity 7, decode off and shutdown off. It is one-way: changes made to the buffer by other means are not reflected back.
    /// </remarks>
    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnTextChanged))]
    private string text;

    /// <summary>The character ROM that maps the characters of <see cref="Text"/> to glyphs.</summary>
    [DependencyProperty(
        DefaultValue = CharacterRom.A00,
        PropertyChangedCallback = nameof(OnTextLayoutChanged))]
    private CharacterRom characterRom;

    /// <summary>
    /// The registers to show. The control creates its own (in the power-on state: shut down, so dark); set one (e.g. bound to a view model
    /// that runs a <see cref="Max7219Controller"/>) to show it. A supplied buffer is resized to <see cref="ModuleCount"/> devices;
    /// <see langword="null"/> returns to the control's own buffer.
    /// </summary>
    /// <remarks>
    /// A newly assigned buffer takes over a non-empty <see cref="Text"/>; an empty text leaves the buffer's registers alone.
    /// </remarks>
    [DependencyProperty(
        PropertyChangedCallback = nameof(OnBufferChanged),
        CoerceValueCallback = nameof(CoerceBuffer))]
    private Max7219ChainBuffer buffer;

    private DotMatrixGeometry? geometry;
    private Max7219ChainBuffer? subscribedBuffer;
    private Max7219ChainSnapshot snapshot;
    private Max7219MatrixWiring snapshotWiring;
    private int snapshotModuleCount;
    private int contentVersion;
    private int isBrightnessUpdatePending;

    /// <summary>
    /// Initializes a new instance of the <see cref="LedMatrixDisplay"/> class: four red FC-16 modules.
    /// </summary>
    public LedMatrixDisplay()
    {
        ownBuffer = new Max7219ChainBuffer(ModuleCount);

        CoerceValue(BufferProperty);
        ApplyPreset();
    }

    /// <summary>Gets the dot grid the control draws.</summary>
    internal DotMatrixGeometry CurrentGeometry => GetGeometry();

    /// <summary>
    /// Maps a dot of a module to its register.
    /// </summary>
    /// <param name="wiring">The module wiring.</param>
    /// <param name="row">The row, 0 = top.</param>
    /// <param name="column">The module column, 0 = leftmost LED of the module.</param>
    /// <returns>The digit register (0-7) and the bit in it (0-7) that drive the LED.</returns>
    internal static (int Digit, int Bit) MapDot(
        Max7219MatrixWiring wiring,
        int row,
        int column)
        => wiring switch
        {
            // §MAX7219.7 visual table.
            Max7219MatrixWiring.Generic => (TopBit - column, TopBit - row),
            Max7219MatrixWiring.Parola => (row, column),
            Max7219MatrixWiring.ICStation => (TopBit - row, column),
            _ => (row, TopBit - column),
        };

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new LedMatrixDisplayAutomationPeer(this);

    /// <inheritdoc />
    protected override DotMatrixGeometry GetGeometry()
    {
        var columnCount = ModuleCount * ModuleSize;
        if (geometry is null ||
            geometry.DotColumns != columnCount)
        {
            geometry = new DotMatrixGeometry(
                columnCount,
                ModuleSize,
                CellWidth: ModuleSize,
                CellHeight: 0,
                DotGap,
                CellGapX: ModuleGap,
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
        snapshot = Buffer.CopyState();
        snapshotWiring = Wiring;
        snapshotModuleCount = ModuleCount;
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
        var state = snapshot;
        var device = snapshotModuleCount - 1 - (x / ModuleSize);
        if (state.Digits is null ||
            device < 0 ||
            device >= state.DeviceCount)
        {
            return false;
        }

        if (state.DisplayTests[device])
        {
            return true;
        }

        if (state.Shutdowns[device])
        {
            return false;
        }

        var (digit, bit) = MapDot(snapshotWiring, y, x % ModuleSize);
        if (digit > state.ScanLimits[device])
        {
            return false;
        }

        var segmentByte = Max7219Segments.GetSegmentByte(
            state.Digits[(device * Max7219ChainBuffer.DigitCount) + digit],
            (state.DecodeModes[device] & (1 << digit)) != 0);
        return (segmentByte & (1 << bit)) != 0;
    }

    private static object CoerceModuleCount(
        DependencyObject d,
        object? value)
        => System.Math.Clamp((int)value!, MinModuleCount, Max7219ChainBuffer.MaxDeviceCount);

    private static object CoerceBuffer(
        DependencyObject d,
        object? value)
        => value ?? ((LedMatrixDisplay)d).ownBuffer;

    private static void OnModuleCountChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (LedMatrixDisplay)d;
        using (display.Buffer.BeginBatch())
        {
            display.Buffer.Resize(display.ModuleCount);
            display.ReapplyText();
        }

        display.InvalidateDots();
    }

    private static void OnWiringChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (LedMatrixDisplay)d;
        display.ReapplyText();
        display.InvalidateDots();
    }

    private static void OnLedColorChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((LedMatrixDisplay)d).ApplyPreset();

    private static void OnTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((LedMatrixDisplay)d).ApplyText();

    private static void OnTextLayoutChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((LedMatrixDisplay)d).ReapplyText();

    private static void OnBufferChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (LedMatrixDisplay)d;

        if (e.OldValue is Max7219ChainBuffer oldBuffer)
        {
            WeakEventManager<Max7219ChainBuffer, EventArgs>.RemoveHandler(
                oldBuffer,
                nameof(Max7219ChainBuffer.Changed),
                display.OnBufferContentChanged);
        }

        if (e.NewValue is Max7219ChainBuffer newBuffer)
        {
            Volatile.Write(ref display.subscribedBuffer, newBuffer);
            WeakEventManager<Max7219ChainBuffer, EventArgs>.AddHandler(
                newBuffer,
                nameof(Max7219ChainBuffer.Changed),
                display.OnBufferContentChanged);

            using (newBuffer.BeginBatch())
            {
                newBuffer.Resize(display.ModuleCount);

                // A buffer that arrives after the text (e.g. through a binding that resolves late) takes it over.
                display.ReapplyText();
            }
        }

        display.UpdateBrightness();
        display.InvalidateDots();
    }

    private void ApplyPreset()
    {
        var (panel, lit, unlit) = LedMatrixPalette.Get(LedColor);

        SetPresetValue(PanelBrushProperty, LedMatrixPalette.CreateBrush(panel));
        SetPresetValue(LitDotBrushProperty, LedMatrixPalette.CreateBrush(lit));
        SetPresetValue(UnlitDotBrushProperty, LedMatrixPalette.CreateBrush(unlit));
        SetPresetValue(GhostOpacityProperty, LedGhostOpacity);
        SetPresetValue(DotCornerRadiusProperty, RoundDot);
        SetPresetValue(GlowRadiusProperty, LedGlowRadius);

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

    // An empty text is not re-applied, so a layout change never blanks an emulator's content.
    private void ReapplyText()
    {
        if (!string.IsNullOrEmpty(Text))
        {
            ApplyText();
        }
    }

    private void ApplyText()
    {
        var target = Buffer;
        var value = Text ?? string.Empty;

        using (target.BeginBatch())
        {
            var deviceCount = target.DeviceCount;
            var digits = new byte[deviceCount * Max7219ChainBuffer.DigitCount];
            if (value.Length > 0)
            {
                var sink = new RegisterSink(digits, deviceCount, Wiring);
                DisplayRasterizer.DrawText(ref sink, 0, 0, value.Replace("\n", string.Empty, StringComparison.Ordinal), CharacterRom, 1);
            }

            for (var device = 0; device < deviceCount; device++)
            {
                if (value.Length > 0)
                {
                    target.SetDisplayTest(device, isDisplayTest: false);
                    target.SetScanLimit(device, BeginScanLimit);
                    target.SetIntensity(device, BeginIntensity);
                    target.SetDecodeMode(device, 0);
                }

                for (var digit = 0; digit < Max7219ChainBuffer.DigitCount; digit++)
                {
                    target.SetDigit(device, digit, digits[(device * Max7219ChainBuffer.DigitCount) + digit]);
                }

                if (value.Length > 0)
                {
                    target.SetShutdown(device, isShutdown: false);
                }
            }
        }
    }

    // UI thread only. The pending flag is cleared by the posted update before it reads, so a change made while it runs posts another.
    private void UpdateBrightness()
    {
        var state = Buffer.CopyState();
        var value = state.DisplayTests[0]
            ? FullBrightness
            : MinIntensityBrightness + (IntensityBrightnessRange * state.Intensities[0] / Max7219ChainBuffer.MaxIntensity);

        SetCurrentValue(BrightnessProperty, value);
    }

    // Raised on the thread that changed the buffer.
    private void OnBufferContentChanged(
        object? sender,
        EventArgs e)
    {
        if (sender is not Max7219ChainBuffer source ||
            !ReferenceEquals(source, Volatile.Read(ref subscribedBuffer)))
        {
            return;
        }

        Interlocked.Increment(ref contentVersion);
        InvalidateDots();

        if (Dispatcher.CheckAccess())
        {
            UpdateBrightness();
            return;
        }

        PostBrightnessUpdate();
    }

    // Any thread: one brightness update at a time is queued on the dispatcher; it reads the latest registers when it runs.
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

    // Sets the register bits of the lit text pixels; the display column counts from the left, device 0 is the rightmost module.
    private readonly struct RegisterSink : IPixelSink
    {
        private readonly byte[] digits;
        private readonly int deviceCount;
        private readonly Max7219MatrixWiring moduleWiring;

        public RegisterSink(
            byte[] digits,
            int deviceCount,
            Max7219MatrixWiring wiring)
        {
            this.digits = digits;
            this.deviceCount = deviceCount;
            moduleWiring = wiring;
        }

        public bool Plot(
            long x,
            long y)
        {
            if (x < 0 ||
                y < 0 ||
                x >= (long)deviceCount * ModuleSize ||
                y >= ModuleSize)
            {
                return false;
            }

            var device = deviceCount - 1 - (int)(x / ModuleSize);
            var (digit, bit) = MapDot(moduleWiring, (int)y, (int)(x % ModuleSize));
            var index = (device * Max7219ChainBuffer.DigitCount) + digit;
            var mask = (byte)(1 << bit);
            if ((digits[index] & mask) != 0)
            {
                return false;
            }

            digits[index] |= mask;
            return true;
        }

        public bool Fill(
            long x,
            long y,
            long width,
            long height)
        {
            var changed = false;
            for (var row = y; row < y + height; row++)
            {
                for (var column = x; column < x + width; column++)
                {
                    changed |= Plot(column, row);
                }
            }

            return changed;
        }
    }
}