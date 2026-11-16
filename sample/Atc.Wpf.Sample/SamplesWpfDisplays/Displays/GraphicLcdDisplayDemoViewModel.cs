namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

public sealed partial class GraphicLcdDisplayDemoViewModel : DisplayFamilyDemoViewModelBase
{
    private const int Width = 128;
    private const int Height = 64;
    private const int BallRadius = 6;

    // Frame mode streams into one buffer and Replay mode emulates the chip on another, so the two never disturb each other.
    private readonly MonochromeFrameBuffer frameBuffer = new(Width, Height);
    private readonly MonochromeFrameBuffer replayBuffer = new(Width, Height);
    private readonly MonochromeFrameBuffer scratch = new(Width, Height);
    private double ballX = 30;
    private double ballY = 20;
    private double ballVelocityX = 2.3;
    private double ballVelocityY = 1.6;
    private int frameCount;

    [ObservableProperty(AfterChangedCallback = nameof(ApplyPreset))]
    private GraphicLcdDisplayPreset selectedPreset = GraphicLcdDisplayPreset.All[0];

    [PropertyDisplay("Color", "Module", 1)]
    [ObservableProperty]
    private CharacterDisplayColor color = CharacterDisplayColor.Blue;

    [PropertyDisplay("Backlight On", "Module", 2)]
    [ObservableProperty]
    private bool isBacklightOn = true;

    [PropertyDisplay("Show Bezel", "Module", 3)]
    [ObservableProperty]
    private bool showBezel = true;

    [PropertyDisplay("Text", "Content", 2)]
    [PropertyEditorHint(EditorHint.MultiLineText)]
    [ObservableProperty(DependentPropertyNames = [nameof(DisplayText)])]
    private string text = "Hello, ESP32!\n\nAtc.Wpf graphic LCD\n128 x 64 ST7920";

    [PropertyDisplay("Character ROM", "Content", 3)]
    [ObservableProperty]
    private CharacterRom characterRom = CharacterRom.A00;

    [PropertyDisplay("Replay ESPHome (else U8g2)", "Replay", 1)]
    [ObservableProperty(AfterChangedCallback = nameof(ReloadReplayIfActive))]
    private bool isEspHomeReplay;

    public GraphicLcdDisplayDemoViewModel()
        : base(TimeSpan.FromMilliseconds(1000.0 / 30))
    {
    }

    /// <summary>
    /// Gets the presets of the common modules.
    /// </summary>
    public IReadOnlyList<GraphicLcdDisplayPreset> Presets
        => GraphicLcdDisplayPreset.All;

    /// <summary>
    /// Gets the buffer the control shows: <see langword="null"/> (the control's own, showing <see cref="Text"/>) in Text mode.
    /// </summary>
    public MonochromeFrameBuffer? ActiveBuffer
        => FeedMode switch
        {
            GraphicDisplayFeedMode.Frame => frameBuffer,
            GraphicDisplayFeedMode.Replay => replayBuffer,
            _ => null,
        };

    /// <summary>
    /// Gets the text the control shows: <see cref="Text"/> in Text mode, otherwise empty.
    /// </summary>
    public string DisplayText
        => FeedMode == GraphicDisplayFeedMode.Text
            ? Text
            : string.Empty;

    /// <inheritdoc />
    protected override void OnFeedModeChanged()
    {
        if (FeedMode == GraphicDisplayFeedMode.Text)
        {
            RaisePropertyChanged(nameof(ActiveBuffer));
            RaisePropertyChanged(nameof(DisplayText));
        }
        else
        {
            RaisePropertyChanged(nameof(DisplayText));
            RaisePropertyChanged(nameof(ActiveBuffer));
        }

        frameBuffer.IsDisplayOn = true;
    }

    // As a device that sends its frame buffer (U8g2 getBufferPtr() as a MonoFrame): a border, a counter and a bouncing ball.
    protected override void OnFrameTick()
    {
        ballX += ballVelocityX;
        ballY += ballVelocityY;
        if (ballX < BallRadius + 1 || ballX > Width - BallRadius - 2)
        {
            ballVelocityX = -ballVelocityX;
            ballX = System.Math.Clamp(ballX, BallRadius + 1, Width - BallRadius - 2);
        }

        if (ballY < BallRadius + 1 || ballY > Height - BallRadius - 2)
        {
            ballVelocityY = -ballVelocityY;
            ballY = System.Math.Clamp(ballY, BallRadius + 1, Height - BallRadius - 2);
        }

        frameCount++;
        using (scratch.BeginBatch())
        {
            scratch.Clear();
            scratch.DrawRectangle(0, 0, Width, Height);
            scratch.DrawText(3, 3, frameCount.ToString(CultureInfo.InvariantCulture));
            var centerX = (int)System.Math.Round(ballX);
            var centerY = (int)System.Math.Round(ballY);
            for (var dy = -BallRadius; dy <= BallRadius; dy++)
            {
                var half = (int)System.Math.Sqrt((BallRadius * BallRadius) - (dy * dy));
                scratch.DrawLine(centerX - half, centerY + dy, centerX + half, centerY + dy);
            }
        }

        frameBuffer.LoadFrame(scratch.GetFrame());
    }

    // A new controller applies the power-on state (display and graphics off), as on a module that was just powered.
    protected override (IDisposable Owner, string Status) CreateReplay(
        ICollection<(string Description, Action Apply)> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);

        var chip = new St7920Controller(replayBuffer);
        var decoder = new St7920SerialDecoder(chip);
        var capture = IsEspHomeReplay
            ? GraphicLcdDisplayCaptures.CreateEspHomeCapture()
            : GraphicLcdDisplayCaptures.CreateU8g2Capture();
        foreach (var transfer in capture)
        {
            steps.Add((DescribeBytes("SPI", transfer), () => decoder.Write(transfer)));
        }

        return (chip, $"{(IsEspHomeReplay ? "ESPHome" : "U8g2")} capture over SPI loaded");
    }

    private void ApplyPreset()
        => Color = SelectedPreset.Color;
}