namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

public sealed partial class SevenSegmentDisplayDemoViewModel : DisplayFamilyDemoViewModelBase
{
    private const byte DecimalPoint = 0x80;

    // Each mode has its own registers, so the modes never disturb each other.
    private readonly Max7219ChainBuffer textBuffer = new(1);
    private readonly Max7219ChainBuffer frameBuffer = new(1);
    private readonly Max7219ChainBuffer replayBuffer = new(1);
    private int tenths;

    [ObservableProperty(AfterChangedCallback = nameof(ApplyPreset))]
    private SevenSegmentDisplayPreset selectedPreset = SevenSegmentDisplayPreset.All[1];

    [PropertyDisplay("Digits", "Module", 1)]
    [PropertyRange(1, 8, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private int digitCount = 8;

    [PropertyDisplay("LED Color", "Module", 2)]
    [ObservableProperty(DependentPropertyNames = [nameof(SegmentBrush)])]
    private LedColor ledColor = LedColor.Red;

    [PropertyDisplay("Intensity", "Module", 3)]
    [PropertyRange(0, 15, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyIntensity))]
    private int intensity = 8;

    [PropertyDisplay("Text (0-9 - E H L P, '.' for a decimal point)", "Content", 2)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyText))]
    private string text = "HELP 1.23";

    public SevenSegmentDisplayDemoViewModel()
        : base(TimeSpan.FromMilliseconds(100))
    {
        ApplyText();
    }

    /// <summary>
    /// Gets the presets of the common modules.
    /// </summary>
    public IReadOnlyList<SevenSegmentDisplayPreset> Presets
        => SevenSegmentDisplayPreset.All;

    /// <summary>
    /// Gets the registers the control shows.
    /// </summary>
    public Max7219ChainBuffer ActiveBuffer
        => FeedMode switch
        {
            GraphicDisplayFeedMode.Frame => frameBuffer,
            GraphicDisplayFeedMode.Replay => replayBuffer,
            _ => textBuffer,
        };

    /// <summary>
    /// Gets the brush of the segments and decimal points.
    /// </summary>
    public Brush SegmentBrush
        => LedColor switch
        {
            LedColor.Green => Brushes.LimeGreen,
            LedColor.Blue => Brushes.DeepSkyBlue,
            LedColor.Yellow => Brushes.Gold,
            LedColor.White => Brushes.WhiteSmoke,
            _ => Brushes.Red,
        };

    /// <inheritdoc />
    protected override void OnFeedModeChanged()
    {
        if (FeedMode == GraphicDisplayFeedMode.Frame)
        {
            // As a sketch that keeps its own registers: on, all digits scanned, no decode (it sends segment bytes).
            using (frameBuffer.BeginBatch())
            {
                frameBuffer.SetShutdown(0, isShutdown: false);
                frameBuffer.SetScanLimit(0, 7);
                frameBuffer.SetDecodeMode(0, 0x00);
                frameBuffer.SetIntensity(0, (byte)Intensity);
            }
        }

        RaisePropertyChanged(nameof(ActiveBuffer));
    }

    // A stopwatch in tenths of a second, as segment bytes with the decimal point before the last digit.
    protected override void OnFrameTick()
    {
        tenths++;
        var value = tenths;
        using (frameBuffer.BeginBatch())
        {
            for (var digit = 0; digit < 8; digit++)
            {
                var segments = digit > 1 && value == 0
                    ? (byte)0x00
                    : SevenSegmentDisplayCaptures.GetDigitSegments(value % 10);
                if (digit == 1)
                {
                    segments |= DecimalPoint;
                }

                frameBuffer.SetDigit(0, digit, segments);
                value /= 10;
            }
        }
    }

    // A new controller applies the power-on state: shut down, blank digits.
    protected override (IDisposable Owner, string Status) CreateReplay(
        ICollection<(string Description, Action Apply)> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);

        var chip = new Max7219Controller(replayBuffer);
        foreach (var frame in SevenSegmentDisplayCaptures.CreateLedControlCapture())
        {
            steps.Add((DescribeBytes("Latch", frame), () => chip.WriteData(frame)));
        }

        return (chip, "LedControl capture loaded");
    }

    private void ApplyPreset()
    {
        DigitCount = SelectedPreset.DigitCount;
        LedColor = SelectedPreset.LedColor;
    }

    private void ApplyIntensity()
    {
        textBuffer.SetIntensity(0, (byte)Intensity);
        frameBuffer.SetIntensity(0, (byte)Intensity);
    }

    // The text right-aligned on digit 0 (the rightmost), with the chip's Code B decoder; '.' lights the decimal point of the character before it.
    private void ApplyText()
    {
        var codes = new List<byte>();
        foreach (var c in Text ?? string.Empty)
        {
            if (c == '.' && codes.Count > 0 && (codes[^1] & DecimalPoint) == 0)
            {
                codes[^1] |= DecimalPoint;
            }
            else if (c == '.')
            {
                codes.Add(0x0F | DecimalPoint);
            }
            else
            {
                codes.Add(SevenSegmentDisplayCaptures.GetCodeB(c));
            }
        }

        using (textBuffer.BeginBatch())
        {
            textBuffer.SetShutdown(0, isShutdown: false);
            textBuffer.SetScanLimit(0, 7);
            textBuffer.SetDecodeMode(0, 0xFF);
            textBuffer.SetIntensity(0, (byte)Intensity);
            for (var digit = 0; digit < 8; digit++)
            {
                var index = codes.Count - 1 - digit;
                textBuffer.SetDigit(0, digit, index >= 0 ? codes[index] : (byte)0x0F);
            }
        }
    }
}