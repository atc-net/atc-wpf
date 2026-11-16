namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

public partial class OledDisplayDemoViewModel : ViewModelBase
{
    private const byte SetContrastCommand = 0x81;
    private const byte DisplayOffCommand = 0xAE;
    private const byte DisplayOnCommand = 0xAF;
    private const int FramesPerSecond = 30;
    private const int BallRadius = 6;
    private const int MaxDescribedBytes = 8;

    private readonly Dispatcher dispatcher;
    private readonly DispatcherTimer frameTimer;
    private readonly DispatcherTimer replayTimer;
    private readonly List<(string Description, Action Apply)> replaySteps = [];

    // Frame mode streams into one buffer and Replay mode emulates the chip on another, so the two never disturb each other.
    private readonly MonochromeFrameBuffer frameBuffer = new();
    private readonly MonochromeFrameBuffer replayBuffer = new();

    private MonochromeFrameBuffer? controlBuffer;
    private MonochromeFrameBuffer? scratch;
    private Ssd1306Controller? controller;
    private int replayIndex;
    private bool isSyncingFromChip;
    private double ballX = 30;
    private double ballY = 20;
    private double ballVelocityX = 2.3;
    private double ballVelocityY = 1.6;
    private int frameCount;
    private bool isLoaded;
    private GraphicDisplayFeedMode previousFeedMode = GraphicDisplayFeedMode.Text;
    private int userContrast;
    private bool isUserDisplayOn;

    [ObservableProperty(AfterChangedCallback = nameof(ApplyPreset))]
    private OledDisplayPreset selectedPreset = OledDisplayPreset.All[1];

    [PropertyDisplay("Size and Chip", "Module", 1)]
    [PropertyEditorHint(EditorHint.ReadOnly)]
    [ObservableProperty]
    private string moduleInfo = "128 x 64, SSD1306";

    [ObservableProperty]
    private int pixelColumns = 128;

    [ObservableProperty]
    private int pixelRows = 64;

    [PropertyDisplay("Color", "Module", 2)]
    [ObservableProperty]
    private OledDisplayColor color = OledDisplayColor.White;

    [PropertyDisplay("Yellow Zone Rows", "Module", 3)]
    [PropertyRange(0, 64, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private int yellowZoneRows = 16;

    [PropertyDisplay("Unlit Pixel Opacity", "Module", 4)]
    [PropertyRange(0, 0.3, 0.01)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double ghostOpacity;

    [PropertyDisplay("Show Bezel", "Module", 5)]
    [ObservableProperty]
    private bool showBezel = true;

    [PropertyDisplay("Contrast", "Display", 1)]
    [PropertyRange(0, 255, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyContrast))]
    private int contrast = 0xCF;

    [PropertyDisplay("Display On", "Display", 2)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyDisplayOn))]
    private bool isDisplayOn = true;

    [PropertyDisplay("Inverted", "Display", 3)]
    [ObservableProperty]
    private bool isInverted;

    [PropertyDisplay("Feed Mode", "Content", 1)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyFeedMode))]
    private GraphicDisplayFeedMode feedMode = GraphicDisplayFeedMode.Text;

    [PropertyDisplay("Text", "Content", 2)]
    [PropertyEditorHint(EditorHint.MultiLineText)]
    [ObservableProperty(DependentPropertyNames = [nameof(DisplayText)])]
    private string text = "Hello, ESP32!\n\nAtc.Wpf OLED\n128 x 64 SSD1306";

    [PropertyDisplay("Character ROM", "Content", 3)]
    [ObservableProperty]
    private CharacterRom characterRom = CharacterRom.A00;

    [PropertyDisplay("Replay over SPI", "Replay", 1)]
    [ObservableProperty(AfterChangedCallback = nameof(ReloadReplayIfActive))]
    private bool isSpiReplay;

    [PropertyDisplay("Replay Interval (ms)", "Replay", 2)]
    [PropertyRange(5, 500, 5)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyReplayInterval))]
    private int replayInterval = 40;

    [ObservableProperty]
    private string replayStatus = "Choose the Replay feed mode to play a capture.";

    public OledDisplayDemoViewModel()
    {
        dispatcher = Dispatcher.CurrentDispatcher;

        frameTimer = new DispatcherTimer(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(1000.0 / FramesPerSecond),
        };

        frameTimer.Tick += OnFrameTimerTick;

        replayTimer = new DispatcherTimer(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(replayInterval),
        };

        replayTimer.Tick += OnReplayTimerTick;

        replayBuffer.Changed += OnReplayBufferChanged;

        StartReplayCommand = new RelayCommand(StartReplay);
        StopReplayCommand = new RelayCommand(PauseReplay);
        StepReplayCommand = new RelayCommand(StepReplay);
    }

    /// <summary>
    /// Gets the presets of the common modules.
    /// </summary>
    public IReadOnlyList<OledDisplayPreset> Presets => OledDisplayPreset.All;

    /// <summary>
    /// Gets the buffer the control shows: <see langword="null"/> (the control's own buffer, showing <see cref="Text"/>) in Text mode,
    /// otherwise a buffer owned by the view model.
    /// </summary>
    public MonochromeFrameBuffer? ActiveBuffer
        => FeedMode switch
        {
            GraphicDisplayFeedMode.Frame => frameBuffer,
            GraphicDisplayFeedMode.Replay => replayBuffer,
            _ => null,
        };

    /// <summary>
    /// Gets the text the control shows: <see cref="Text"/> in Text mode, otherwise empty, so the text never draws over a frame or a replay.
    /// </summary>
    public string DisplayText
        => FeedMode == GraphicDisplayFeedMode.Text
            ? Text
            : string.Empty;

    [PropertyDisplay("Start Replay", "Replay", 3)]
    public ICommand StartReplayCommand { get; }

    [PropertyDisplay("Stop Replay", "Replay", 4)]
    public ICommand StopReplayCommand { get; }

    [PropertyDisplay("Step", "Replay", 5)]
    public ICommand StepReplayCommand { get; }

    /// <summary>
    /// Gives the view model the control's own buffer, which shows <see cref="Text"/>, so the contrast and display on/off
    /// settings reach it in Text mode. The view calls it before it sets the data context.
    /// </summary>
    /// <param name="buffer">The control's own buffer.</param>
    public void AttachControlBuffer(MonochromeFrameBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        controlBuffer = buffer;
        if (FeedMode == GraphicDisplayFeedMode.Text)
        {
            ApplyDisplayState(buffer);
        }
    }

    /// <summary>
    /// Restarts the feed of the current mode; the view calls it when it is loaded again.
    /// </summary>
    public void Resume()
    {
        isLoaded = true;

        if (FeedMode == GraphicDisplayFeedMode.Frame)
        {
            frameTimer.Start();
        }
        else if (FeedMode == GraphicDisplayFeedMode.Replay)
        {
            // Unloading released the emulator, so the capture plays again from the start.
            if (controller is null)
            {
                LoadReplay();
            }

            replayTimer.Start();
        }
    }

    /// <summary>
    /// Stops the frame stream and the replay and releases the emulator; the view calls it when it unloads.
    /// </summary>
    public void Stop()
    {
        isLoaded = false;
        frameTimer.Stop();
        StopReplay();
    }

    private static string Describe(
        string prefix,
        byte[] bytes)
    {
        var shown = string.Join(' ', bytes.Take(MaxDescribedBytes).Select(b => b.ToString("X2", CultureInfo.InvariantCulture)));
        return bytes.Length > MaxDescribedBytes
            ? string.Create(CultureInfo.InvariantCulture, $"{prefix} {shown} ... ({bytes.Length} bytes)")
            : $"{prefix} {shown}";
    }

    private void ApplyPreset()
    {
        var preset = SelectedPreset;

        // The view model's buffers are resized first, so a frame tick never meets a buffer of the old size.
        frameBuffer.Resize(preset.Columns, preset.Rows);
        replayBuffer.Resize(preset.Columns, preset.Rows);

        PixelColumns = preset.Columns;
        PixelRows = preset.Rows;
        Color = preset.Color;
        ModuleInfo = string.Create(
            CultureInfo.InvariantCulture,
            $"{preset.Columns} x {preset.Rows}, {preset.Chip.ToString().ToUpperInvariant()}");

        // The emulator is recreated for the new chip: the capture is the one the library sends to that module.
        ReloadReplayIfActive();
    }

    private void ApplyFeedMode()
    {
        frameTimer.Stop();
        StopReplay();

        KeepUserDisplayState();

        // The text is emptied before a view-model buffer is shown, and the control's own buffer is back before the text returns,
        // so the text is drawn only into the control's own buffer.
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

        switch (FeedMode)
        {
            case GraphicDisplayFeedMode.Text:
                if (controlBuffer is not null)
                {
                    ApplyDisplayState(controlBuffer);
                }

                ReplayStatus = "Choose the Replay feed mode to play a capture.";
                break;
            case GraphicDisplayFeedMode.Frame:
                frameBuffer.IsInverted = false;
                frameBuffer.IsEntireDisplayOn = false;
                ApplyDisplayState(frameBuffer);
                ReplayStatus = string.Create(CultureInfo.InvariantCulture, $"Streaming whole frames with LoadFrame at {FramesPerSecond} fps.");
                if (isLoaded)
                {
                    frameTimer.Start();
                }

                break;
            case GraphicDisplayFeedMode.Replay:
                LoadReplay();
                if (isLoaded)
                {
                    replayTimer.Start();
                }

                break;
        }
    }

    // In Replay mode the panel shows the chip's contrast and display state; the user's settings are kept and restored when it ends.
    private void KeepUserDisplayState()
    {
        if (FeedMode == GraphicDisplayFeedMode.Replay && previousFeedMode != GraphicDisplayFeedMode.Replay)
        {
            userContrast = Contrast;
            isUserDisplayOn = IsDisplayOn;
        }
        else if (FeedMode != GraphicDisplayFeedMode.Replay && previousFeedMode == GraphicDisplayFeedMode.Replay)
        {
            Contrast = userContrast;
            IsDisplayOn = isUserDisplayOn;
        }

        previousFeedMode = FeedMode;
    }

    private void ApplyDisplayState(MonochromeFrameBuffer target)
    {
        using (target.BeginBatch())
        {
            target.Contrast = (byte)Contrast;
            target.IsDisplayOn = IsDisplayOn;
        }
    }

    // In Replay mode the chip owns its state, so the settings are sent as the commands a host would send.
    private void ApplyContrast()
    {
        if (isSyncingFromChip)
        {
            return;
        }

        if (controller is not null)
        {
            controller.WriteCommands([SetContrastCommand, (byte)Contrast]);
        }
        else if ((ActiveBuffer ?? controlBuffer) is { } target)
        {
            target.Contrast = (byte)Contrast;
        }
    }

    private void ApplyDisplayOn()
    {
        if (isSyncingFromChip)
        {
            return;
        }

        if (controller is not null)
        {
            controller.WriteCommand(IsDisplayOn ? DisplayOnCommand : DisplayOffCommand);
        }
        else if ((ActiveBuffer ?? controlBuffer) is { } target)
        {
            target.IsDisplayOn = IsDisplayOn;
        }
    }

    private void ApplyReplayInterval()
        => replayTimer.Interval = TimeSpan.FromMilliseconds(ReplayInterval);

    private void ReloadReplayIfActive()
    {
        if (FeedMode != GraphicDisplayFeedMode.Replay)
        {
            return;
        }

        // A paused replay stays paused, and nothing runs while the view is unloaded.
        var wasRunning = replayTimer.IsEnabled;
        LoadReplay();
        if (wasRunning && isLoaded)
        {
            replayTimer.Start();
        }
    }

    private void StartReplay()
    {
        if (FeedMode != GraphicDisplayFeedMode.Replay)
        {
            FeedMode = GraphicDisplayFeedMode.Replay;
            return;
        }

        if (controller is null || replayIndex >= replaySteps.Count)
        {
            LoadReplay();
        }

        replayTimer.Start();
    }

    private void PauseReplay()
        => replayTimer.Stop();

    private void StepReplay()
    {
        if (FeedMode != GraphicDisplayFeedMode.Replay)
        {
            return;
        }

        replayTimer.Stop();

        if (controller is null || replayIndex >= replaySteps.Count)
        {
            LoadReplay();
        }

        ApplyNextStep();
    }

    // A new controller applies the chip's power-on state: display off and the RAM cleared, as on a module that was just powered.
    private void LoadReplay()
    {
        StopReplay();

        var preset = SelectedPreset;
        var chip = new Ssd1306Controller(replayBuffer, preset.Chip);
        controller = chip;

        if (IsSpiReplay)
        {
            foreach (var step in OledDisplayCaptures.CreateSpiCapture(preset.Chip, preset.Rows))
            {
                replaySteps.Add(step.IsData
                    ? (Describe("SPI data", step.Bytes), () => chip.WriteData(step.Bytes))
                    : (Describe("SPI command", step.Bytes), () => chip.WriteCommands(step.Bytes)));
            }
        }
        else
        {
            var decoder = new Ssd1306I2cDecoder(chip);
            foreach (var transaction in OledDisplayCaptures.CreateI2cCapture(preset.Chip, preset.Rows))
            {
                replaySteps.Add((Describe("I2C", transaction), () => decoder.Write(transaction)));
            }
        }

        var library = preset.Chip == OledChip.Sh1106
            ? "U8g2"
            : "Adafruit_SSD1306";
        ReplayStatus = string.Create(
            CultureInfo.InvariantCulture,
            $"{library} capture over {(IsSpiReplay ? "SPI" : "I2C")} loaded: {replaySteps.Count} steps.");

        SyncFromChip();
    }

    private void StopReplay()
    {
        replayTimer.Stop();
        replaySteps.Clear();
        replayIndex = 0;

        controller?.Dispose();
        controller = null;
    }

    private void ApplyNextStep()
    {
        if (replayIndex >= replaySteps.Count)
        {
            replayTimer.Stop();
            return;
        }

        var (description, apply) = replaySteps[replayIndex];
        apply();
        replayIndex++;

        ReplayStatus = string.Create(
            CultureInfo.InvariantCulture,
            $"Step {replayIndex} of {replaySteps.Count}: {description}");

        if (replayIndex >= replaySteps.Count)
        {
            replayTimer.Stop();
        }
    }

    // Shows the chip's contrast and display state (set by the replayed commands) in the property panel.
    private void SyncFromChip()
    {
        if (FeedMode != GraphicDisplayFeedMode.Replay || controller is null)
        {
            return;
        }

        isSyncingFromChip = true;
        try
        {
            Contrast = replayBuffer.Contrast;
            IsDisplayOn = replayBuffer.IsDisplayOn;
        }
        finally
        {
            isSyncingFromChip = false;
        }
    }

    private void DrawBouncingBall()
    {
        var width = frameBuffer.Width;
        var height = frameBuffer.Height;
        if (scratch is null || scratch.Width != width || scratch.Height != height)
        {
            scratch = new MonochromeFrameBuffer(width, height);
        }

        ballX += ballVelocityX;
        ballY += ballVelocityY;
        if (ballX < BallRadius + 1 || ballX > width - BallRadius - 2)
        {
            ballVelocityX = -ballVelocityX;
            ballX = System.Math.Clamp(ballX, BallRadius + 1, width - BallRadius - 2);
        }

        if (ballY < BallRadius + 1 || ballY > height - BallRadius - 2)
        {
            ballVelocityY = -ballVelocityY;
            ballY = System.Math.Clamp(ballY, BallRadius + 1, height - BallRadius - 2);
        }

        frameCount++;

        using (scratch.BeginBatch())
        {
            scratch.Clear();
            scratch.DrawRectangle(0, 0, width, height);
            scratch.DrawText(3, 3, frameCount.ToString(CultureInfo.InvariantCulture));

            var centerX = (int)System.Math.Round(ballX);
            var centerY = (int)System.Math.Round(ballY);
            for (var dy = -BallRadius; dy <= BallRadius; dy++)
            {
                var half = (int)System.Math.Sqrt((BallRadius * BallRadius) - (dy * dy));
                scratch.DrawLine(centerX - half, centerY + dy, centerX + half, centerY + dy);
            }
        }

        // As a device would: the whole frame, the same bytes as Adafruit getBuffer() or U8g2 getBufferPtr().
        frameBuffer.LoadFrame(scratch.GetFrame());
    }

    private void OnFrameTimerTick(
        object? sender,
        EventArgs e)
        => DrawBouncingBall();

    private void OnReplayTimerTick(
        object? sender,
        EventArgs e)
        => ApplyNextStep();

    private void OnReplayBufferChanged(
        object? sender,
        EventArgs e)
    {
        if (dispatcher.CheckAccess())
        {
            SyncFromChip();
        }
        else
        {
            _ = dispatcher.BeginInvoke(SyncFromChip);
        }
    }
}