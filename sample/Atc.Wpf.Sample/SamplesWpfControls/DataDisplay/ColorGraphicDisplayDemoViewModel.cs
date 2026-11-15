namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class ColorGraphicDisplayDemoViewModel : ViewModelBase
{
    private const int FramesPerSecond = 30;
    private const int MaxDescribedBytes = 8;
    private const string TextModeStatus = "Choose the Replay feed mode to play a capture.";

    private readonly Dispatcher dispatcher;
    private readonly DispatcherTimer frameTimer;
    private readonly DispatcherTimer replayTimer;
    private readonly List<(string Description, Action Apply)> replaySteps = [];

    // Frame mode streams into one buffer and Replay mode emulates the chip on another, so the two never disturb each other.
    private readonly ColorFrameBuffer frameBuffer = new();
    private readonly ColorFrameBuffer replayBuffer = new();

    private ColorFrameBuffer? controlBuffer;
    private IGraphicDisplayController? controller;
    private byte[] frameBytes = [];
    private int replayIndex;
    private bool isSyncingFromChip;
    private double ballX = 40;
    private double ballY = 50;
    private double ballVelocityX = 2.3;
    private double ballVelocityY = 1.7;
    private int frameCount;
    private bool isLoaded;
    private GraphicDisplayFeedMode previousFeedMode = GraphicDisplayFeedMode.Text;
    private int userBrightness;
    private bool isUserDisplayOn;
    private bool isUserInverted;

    [ObservableProperty(AfterChangedCallback = nameof(ApplyPreset))]
    private ColorGraphicDisplayPreset selectedPreset = ColorGraphicDisplayPreset.All[2];

    [PropertyDisplay("Size and Chip", "Module", 1)]
    [PropertyEditorHint(EditorHint.ReadOnly)]
    [ObservableProperty]
    private string moduleInfo = "128 x 160, ST7735";

    [ObservableProperty]
    private int pixelColumns = 128;

    [ObservableProperty]
    private int pixelRows = 160;

    [PropertyDisplay("Panel Kind", "Module", 2)]
    [ObservableProperty]
    private ColorPanelKind panelKind = ColorPanelKind.Tft;

    [PropertyDisplay("Show Pixel Grid", "Module", 3)]
    [ObservableProperty]
    private bool showPixelGrid = true;

    [PropertyDisplay("Show Bezel", "Module", 4)]
    [ObservableProperty]
    private bool showBezel = true;

    [PropertyDisplay("Panel Is BGR (emulator)", "Module", 5)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyPanelBgr))]
    private bool isPanelBgr = true;

    [PropertyDisplay("Panel Is Inverted (TFT emulator)", "Module", 6)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyPanelInverted))]
    private bool isPanelInverted;

    [PropertyDisplay("Brightness", "Display", 1)]
    [PropertyRange(0, 255, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyBrightness))]
    private int brightness = 255;

    [PropertyDisplay("Display On", "Display", 2)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyDisplayOn))]
    private bool isDisplayOn = true;

    [PropertyDisplay("Inverted", "Display", 3)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyInverted))]
    private bool isInverted;

    [PropertyDisplay("Feed Mode", "Content", 1)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyFeedMode))]
    private GraphicDisplayFeedMode feedMode = GraphicDisplayFeedMode.Text;

    [PropertyDisplay("Text", "Content", 2)]
    [PropertyEditorHint(EditorHint.MultiLineText)]
    [ObservableProperty(DependentPropertyNames = [nameof(DisplayText)])]
    private string text = "Hello, ESP32!\n\nAtc.Wpf colour\ngraphic display";

    [PropertyDisplay("Text Color", "Content", 3)]
    [ObservableProperty]
    private Color textColor = Colors.White;

    [PropertyDisplay("Character ROM", "Content", 4)]
    [ObservableProperty]
    private CharacterRom characterRom = CharacterRom.A00;

    [PropertyDisplay("RGB565 Byte Order (receiver)", "Frame", 1)]
    [ObservableProperty]
    private Rgb565ByteOrder byteOrder = Rgb565ByteOrder.LittleEndian;

    [PropertyDisplay("Replay Interval (ms)", "Replay", 1)]
    [PropertyRange(5, 500, 5)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyReplayInterval))]
    private int replayInterval = 40;

    [ObservableProperty]
    private string replayStatus = TextModeStatus;

    public ColorGraphicDisplayDemoViewModel()
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
    public IReadOnlyList<ColorGraphicDisplayPreset> Presets
        => ColorGraphicDisplayPreset.All;

    /// <summary>
    /// Gets the buffer the control shows: <see langword="null"/> (the control's own buffer, showing <see cref="Text"/>) in Text mode,
    /// otherwise a buffer owned by the view model.
    /// </summary>
    public ColorFrameBuffer? ActiveBuffer
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

    [PropertyDisplay("Start Replay", "Replay", 2)]
    public ICommand StartReplayCommand { get; }

    [PropertyDisplay("Stop Replay", "Replay", 3)]
    public ICommand StopReplayCommand { get; }

    [PropertyDisplay("Step", "Replay", 4)]
    public ICommand StepReplayCommand { get; }

    /// <summary>
    /// Gives the view model the control's own buffer, which shows <see cref="Text"/>, so the brightness, display on/off and invert
    /// settings reach it in Text mode. The view calls it before it sets the data context.
    /// </summary>
    /// <param name="buffer">The control's own buffer.</param>
    public void AttachControlBuffer(ColorFrameBuffer buffer)
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

    private static string ChipName(ColorGraphicDisplayPreset preset)
        => preset.Name[(preset.Name.LastIndexOf(' ') + 1)..];

    // A triangle wave over 0..63 folded to 0..31.
    private static int Triangle(int value)
    {
        var folded = value & 63;
        return folded < 32 ? folded : 63 - folded;
    }

    private void ApplyPreset()
    {
        var preset = SelectedPreset;

        // The view model's buffers are resized first, so a frame tick never meets a buffer of the old size.
        frameBuffer.Resize(preset.Columns, preset.Rows);
        replayBuffer.Resize(preset.Columns, preset.Rows);

        PixelColumns = preset.Columns;
        PixelRows = preset.Rows;
        PanelKind = preset.Kind;
        ModuleInfo = string.Create(CultureInfo.InvariantCulture, $"{preset.Columns} x {preset.Rows}, {ChipName(preset)}");
        ShowPanelDefaults(preset);

        // The emulator is recreated for the new chip: the capture is the one the library sends to that module.
        ReloadReplayIfActive();
    }

    // The panel switches show the new module's wiring in every mode: read from a throw-away emulator on a scratch buffer.
    private void ShowPanelDefaults(ColorGraphicDisplayPreset preset)
    {
        var probe = preset.CreateController(new ColorFrameBuffer(preset.Columns, preset.Rows));
        isSyncingFromChip = true;
        try
        {
            switch (probe)
            {
                case TftController tft:
                    IsPanelBgr = tft.IsPanelBgr;
                    IsPanelInverted = tft.IsPanelInverted;
                    break;
                case Ssd1331Controller ssd1331:
                    IsPanelBgr = ssd1331.IsPanelBgr;
                    IsPanelInverted = false;
                    break;
                case Ssd1351Controller ssd1351:
                    IsPanelBgr = ssd1351.IsPanelBgr;
                    IsPanelInverted = false;
                    break;
            }
        }
        finally
        {
            isSyncingFromChip = false;
            probe.Dispose();
        }
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

                ReplayStatus = TextModeStatus;
                break;
            case GraphicDisplayFeedMode.Frame:
                frameBuffer.IsSleeping = false;
                ApplyDisplayState(frameBuffer);
                ReplayStatus = string.Create(
                    CultureInfo.InvariantCulture,
                    $"Streaming little-endian RGB565 frames (as a GFXcanvas16 holds them) with LoadFrameRgb565 at {FramesPerSecond} fps; set the byte order to BigEndian to see a wrong swap.");
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

    // In Replay mode the panel shows the chip's display state; the user's settings are kept and restored when it ends.
    private void KeepUserDisplayState()
    {
        if (FeedMode == GraphicDisplayFeedMode.Replay && previousFeedMode != GraphicDisplayFeedMode.Replay)
        {
            userBrightness = Brightness;
            isUserDisplayOn = IsDisplayOn;
            isUserInverted = IsInverted;
        }
        else if (FeedMode != GraphicDisplayFeedMode.Replay && previousFeedMode == GraphicDisplayFeedMode.Replay)
        {
            Brightness = userBrightness;
            IsDisplayOn = isUserDisplayOn;
            IsInverted = isUserInverted;
        }

        previousFeedMode = FeedMode;
    }

    private void ApplyDisplayState(ColorFrameBuffer target)
    {
        using (target.BeginBatch())
        {
            target.Brightness = (byte)Brightness;
            target.IsDisplayOn = IsDisplayOn;
            target.IsInverted = IsInverted;
        }
    }

    // In Replay mode the chip owns its state, so the settings are sent as the commands a host would send.
    private void ApplyBrightness()
    {
        if (isSyncingFromChip)
        {
            return;
        }

        switch (controller)
        {
            case TftController { Chip: not TftChip.St7735 } tft:
                tft.WriteCommand(0x51);
                tft.WriteData([(byte)Brightness]);
                break;
            case not null:
                // The ST7735 has no brightness command and the colour OLEDs keep their contrast apart from the brightness.
                ReplayStatus = $"The {ChipName(SelectedPreset)} has no brightness command (a module dims its backlight through a pin).";
                _ = dispatcher.BeginInvoke(SyncFromChip);
                break;
            default:
                if ((ActiveBuffer ?? controlBuffer) is { } target)
                {
                    target.Brightness = (byte)Brightness;
                }

                break;
        }
    }

    private void ApplyDisplayOn()
    {
        if (isSyncingFromChip)
        {
            return;
        }

        switch (controller)
        {
            case TftController tft:
                tft.WriteCommand(IsDisplayOn ? (byte)0x29 : (byte)0x28);
                break;
            case not null:
                controller.WriteCommand(IsDisplayOn ? (byte)0xAF : (byte)0xAE);
                break;
            default:
                if ((ActiveBuffer ?? controlBuffer) is { } target)
                {
                    target.IsDisplayOn = IsDisplayOn;
                }

                break;
        }
    }

    private void ApplyInverted()
    {
        if (isSyncingFromChip)
        {
            return;
        }

        switch (controller)
        {
            case TftController tft:
                // The switch is the inversion shown: an inverted (IPS) panel shows it with the chip's inversion off.
                tft.WriteCommand(IsInverted != tft.IsPanelInverted ? (byte)0x21 : (byte)0x20);
                break;
            case Ssd1331Controller ssd1331:
                ssd1331.WriteCommand(IsInverted ? (byte)0xA7 : (byte)0xA4);
                break;
            case Ssd1351Controller ssd1351:
                ssd1351.WriteCommand(IsInverted ? (byte)0xA7 : (byte)0xA6);
                break;
            default:
                if ((ActiveBuffer ?? controlBuffer) is { } target)
                {
                    target.IsInverted = IsInverted;
                }

                break;
        }
    }

    // The colour order is the wiring of the emulated module: it changes how the chip's colour order bit is shown.
    private void ApplyPanelBgr()
    {
        if (isSyncingFromChip)
        {
            return;
        }

        switch (controller)
        {
            case TftController tft:
                tft.IsPanelBgr = IsPanelBgr;
                break;
            case Ssd1331Controller ssd1331:
                ssd1331.IsPanelBgr = IsPanelBgr;
                break;
            case Ssd1351Controller ssd1351:
                ssd1351.IsPanelBgr = IsPanelBgr;
                break;
            default:
                ReplayStatus = "The panel colour order belongs to the chip emulator: a replay starts with the module's colour order, and the switch acts while a replay is loaded.";
                break;
        }
    }

    // An IPS panel (the ST7789 modules) shows true colours with the chip's inversion on; a TN panel with it off.
    private void ApplyPanelInverted()
    {
        if (isSyncingFromChip)
        {
            return;
        }

        if (controller is TftController tft)
        {
            tft.IsPanelInverted = IsPanelInverted;
        }
        else
        {
            ReplayStatus = "The inverted panel belongs to the TFT chip emulator: a replay starts with the module's panel, and the switch acts while a TFT replay is loaded.";
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

    // A new controller applies the chip's power-on state (display off), as on a module that was just powered.
    private void LoadReplay()
    {
        StopReplay();

        var preset = SelectedPreset;
        replayBuffer.Resize(preset.Columns, preset.Rows);
        var chip = preset.CreateController(replayBuffer);
        controller = chip;

        foreach (var step in ColorGraphicDisplayCaptures.Create(preset.Name))
        {
            replaySteps.Add(step.IsData
                ? (Describe("SPI data", step.Bytes), () => chip.WriteData(step.Bytes))
                : (Describe("SPI command", step.Bytes), () => chip.WriteCommands(step.Bytes)));
        }

        ReplayStatus = string.Create(
            CultureInfo.InvariantCulture,
            $"Adafruit {ChipName(preset)} capture loaded: {replaySteps.Count} steps.");

        SyncFromChip();
    }

    private void StopReplay()
    {
        replayTimer.Stop();
        replaySteps.Clear();
        replayIndex = 0;

        // A controller left alive stays subscribed to the buffer and would draw its RAM over the next one.
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

    // Shows the chip's display state (set by the replayed commands) and the module's colour order in the property panel.
    private void SyncFromChip()
    {
        if (FeedMode != GraphicDisplayFeedMode.Replay || controller is null)
        {
            return;
        }

        isSyncingFromChip = true;
        try
        {
            Brightness = replayBuffer.Brightness;
            IsDisplayOn = replayBuffer.IsDisplayOn && !replayBuffer.IsSleeping;
            IsInverted = replayBuffer.IsInverted;
            IsPanelBgr = controller switch
            {
                TftController tft => tft.IsPanelBgr,
                Ssd1331Controller ssd1331 => ssd1331.IsPanelBgr,
                Ssd1351Controller ssd1351 => ssd1351.IsPanelBgr,
                _ => IsPanelBgr,
            };

            if (controller is TftController panel)
            {
                IsPanelInverted = panel.IsPanelInverted;
            }
        }
        finally
        {
            isSyncingFromChip = false;
        }
    }

    // A moving colour gradient and a bouncing white ball, written as a device holds RGB565 in memory: little-endian.
    private void DrawFrame()
    {
        var width = frameBuffer.Width;
        var height = frameBuffer.Height;
        if (frameBytes.Length != width * height * 2)
        {
            frameBytes = new byte[width * height * 2];
        }

        var radius = System.Math.Max(3, System.Math.Min(width, height) / 8);
        ballX += ballVelocityX;
        ballY += ballVelocityY;
        if (ballX < radius || ballX > width - radius - 1)
        {
            ballVelocityX = -ballVelocityX;
            ballX = System.Math.Clamp(ballX, radius, width - radius - 1);
        }

        if (ballY < radius || ballY > height - radius - 1)
        {
            ballVelocityY = -ballVelocityY;
            ballY = System.Math.Clamp(ballY, radius, height - radius - 1);
        }

        frameCount++;
        var centerX = (int)System.Math.Round(ballX);
        var centerY = (int)System.Math.Round(ballY);
        var index = 0;
        for (var y = 0; y < height; y++)
        {
            var green = y * 63 / System.Math.Max(1, height - 1);
            var blue = Triangle((y * 64 / height) + (2 * frameCount));
            var dy = y - centerY;
            for (var x = 0; x < width; x++)
            {
                var dx = x - centerX;
                var word = (dx * dx) + (dy * dy) <= radius * radius
                    ? 0xFFFF
                    : (Triangle((x * 64 / width) + frameCount) << 11) | (green << 5) | blue;
                frameBytes[index++] = (byte)word;
                frameBytes[index++] = (byte)(word >> 8);
            }
        }

        frameBuffer.LoadFrameRgb565(frameBytes, ByteOrder);
    }

    private void OnFrameTimerTick(
        object? sender,
        EventArgs e)
        => DrawFrame();

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