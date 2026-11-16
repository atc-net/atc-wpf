namespace Atc.Wpf.Displays;

/// <summary>
/// A row of seven-segment digits driven by a MAX7219 chip (the common 8-digit modules), drawn with <see cref="SevenSegment"/> digits,
/// so a screen can mirror the display of an ESP32 or Raspberry Pi project.
/// </summary>
/// <remarks>
/// <para>
/// The control shows device 0 of a <see cref="Max7219ChainBuffer"/>; other devices of the buffer are not shown. MAX7219 digit 0 is the
/// rightmost digit, as on common 8-digit modules (§MAX7219.8).
/// </para>
/// <para>
/// What is lit (§MAX7219.5): display test lights every segment and decimal point at full brightness; otherwise a shut-down chip is dark;
/// otherwise a digit above the scan limit is dark; otherwise a digit whose decode-mode bit is set shows its Code B character
/// (0-9, -, E, H, L, P or blank, from the low nibble; §MAX7219.4) and any other digit shows its register bits D6..D0 as segments
/// A B C D E F G (§MAX7219.3). Bit D7 is the decimal point in both modes. The intensity sets the opacity of the digits:
/// <c>0.25 + 0.75 * intensity / 15</c>.
/// </para>
/// <para>
/// Buffer changes may come from any thread; they are shown on the UI thread.
/// </para>
/// </remarks>
public sealed partial class SevenSegmentDisplay : UserControl
{
    private const int MinDigitCount = 1;
    private const string DefaultBrushKey = "AtcApps.Brushes.Accent";
    private const double MinIntensityOpacity = 0.25;
    private const double IntensityOpacityRange = 0.75;
    private const double FullOpacity = 1.0;
    private const double DigitWidth = 30;
    private const double DigitHeight = 50;
    private const double DigitSpacing = 6;

    private readonly Max7219ChainBuffer ownBuffer = new(1);
    private readonly StackPanel panel = new() { Orientation = Orientation.Horizontal };
    private readonly List<SevenSegment> segmentControls = [];

    /// <summary>The number of digits shown, 1 to 8: MAX7219 digits 0 (rightmost) to <c>DigitCount - 1</c>.</summary>
    [DependencyProperty(
        DefaultValue = 8,
        PropertyChangedCallback = nameof(OnDigitCountChanged),
        CoerceValueCallback = nameof(CoerceDigitCount))]
    private int digitCount;

    /// <summary>
    /// The registers to show (device 0 only). The control creates its own (in the power-on state: shut down, so dark); set one
    /// (e.g. bound to a view model that runs a <see cref="Max7219Controller"/>) to show it; <see langword="null"/> returns to the control's own buffer.
    /// </summary>
    [DependencyProperty(
        PropertyChangedCallback = nameof(OnBufferChanged),
        CoerceValueCallback = nameof(CoerceBuffer))]
    private Max7219ChainBuffer buffer;

    /// <summary>The brush of the segments; <see langword="null"/> uses the theme's accent brush.</summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnBrushChanged))]
    private Brush? segmentBrush;

    /// <summary>The brush of the decimal points; <see langword="null"/> uses the theme's accent brush.</summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnBrushChanged))]
    private Brush? dotBrush;

    private Max7219ChainBuffer? subscribedBuffer;
    private int isUpdatePending;

    /// <summary>
    /// Initializes a new instance of the <see cref="SevenSegmentDisplay"/> class: eight digits.
    /// </summary>
    public SevenSegmentDisplay()
    {
        Content = new Viewbox { Child = panel };

        CoerceValue(BufferProperty);
        RebuildDigits();
    }

    /// <summary>Gets the digit controls from left to right; the last one is MAX7219 digit 0.</summary>
    internal IReadOnlyList<SevenSegment> SegmentControls
        => segmentControls;

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new SevenSegmentDisplayAutomationPeer(this);

    private static object CoerceDigitCount(
        DependencyObject d,
        object? value)
        => System.Math.Clamp((int)value!, MinDigitCount, Max7219ChainBuffer.DigitCount);

    private static object CoerceBuffer(
        DependencyObject d,
        object? value)
        => value ?? ((SevenSegmentDisplay)d).ownBuffer;

    private static void OnDigitCountChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((SevenSegmentDisplay)d).RebuildDigits();

    private static void OnBrushChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (SevenSegmentDisplay)d;
        foreach (var control in display.segmentControls)
        {
            display.ApplyBrushes(control);
        }
    }

    private static void OnBufferChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var display = (SevenSegmentDisplay)d;

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
        }

        display.UpdateDigits();
    }

    private void RebuildDigits()
    {
        panel.Children.Clear();
        segmentControls.Clear();

        for (var i = 0; i < DigitCount; i++)
        {
            var control = new SevenSegment
            {
                Width = DigitWidth,
                Height = DigitHeight,
                Margin = new Thickness(DigitSpacing / 2, 0, DigitSpacing / 2, 0),
                PlayAnimation = false,
                Segments = SevenSegmentParts.None,
            };
            ApplyBrushes(control);
            segmentControls.Add(control);
            panel.Children.Add(control);
        }

        UpdateDigits();
    }

    private void ApplyBrushes(SevenSegment control)
    {
        if (SegmentBrush is { } segments)
        {
            control.NumberColor = segments;
        }
        else
        {
            control.SetResourceReference(SevenSegment.NumberColorProperty, DefaultBrushKey);
        }

        if (DotBrush is { } dots)
        {
            control.ColonColor = dots;
        }
        else
        {
            control.SetResourceReference(SevenSegment.ColonColorProperty, DefaultBrushKey);
        }
    }

    // UI thread only: shows device 0 of one snapshot of the buffer.
    private void UpdateDigits()
    {
        var state = Buffer.CopyState();
        var isTest = state.DisplayTests[0];
        var isShutdown = state.Shutdowns[0];
        var scanLimit = state.ScanLimits[0];
        var decodeMode = state.DecodeModes[0];
        var opacity = isTest
            ? FullOpacity
            : MinIntensityOpacity + (IntensityOpacityRange * state.Intensities[0] / Max7219ChainBuffer.MaxIntensity);

        var count = segmentControls.Count;
        for (var digit = 0; digit < count; digit++)
        {
            var control = segmentControls[count - 1 - digit];
            byte segmentByte;
            if (isTest)
            {
                segmentByte = 0xFF;
            }
            else if (isShutdown || digit > scanLimit)
            {
                segmentByte = 0x00;
            }
            else
            {
                segmentByte = Max7219Segments.GetSegmentByte(state.Digits[digit], (decodeMode & (1 << digit)) != 0);
            }

            control.Segments = Max7219Segments.ToParts(segmentByte);
            control.ShowDot = (segmentByte & Max7219Segments.DecimalPoint) != 0;
            control.Opacity = opacity;
        }
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

        if (Dispatcher.CheckAccess())
        {
            UpdateDigits();
            return;
        }

        if (Interlocked.CompareExchange(ref isUpdatePending, 1, 0) != 0 ||
            Dispatcher.HasShutdownStarted)
        {
            return;
        }

        _ = Dispatcher.BeginInvoke(
            DispatcherPriority.Render,
            new Action(() =>
            {
                Interlocked.Exchange(ref isUpdatePending, 0);
                UpdateDigits();
            }));
    }
}