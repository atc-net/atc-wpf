namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class CharacterDisplayDemoViewModel : ViewModelBase
{
    private const double LcdGhostOpacity = 0.08;
    private const double ReflectiveGhostOpacity = 0.04;
    private const double VfdGhostOpacity = 0.06;
    private const int BarGraphStep = 13;
    private const int DotsPerCell = 5;
    private const int DegreeSlot = 0;
    private const int ThermometerSlot = 1;
    private const int FirstBarSlot = 2;

    private static readonly byte[] ThermometerGlyph = [0x04, 0x0A, 0x0A, 0x0E, 0x0E, 0x1F, 0x1F, 0x0E];

    private readonly DispatcherTimer replayTimer;
    private readonly List<(string Description, Action Apply)> replaySteps = [];
    private int replayIndex;
    private bool isLastReplayI2c;
    private Pcf8574Hd44780Decoder? decoder;
    private int barGraphLevel = 60;

    [PropertyDisplay("Size Preset", "Size", 1)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplySizePreset))]
    private CharacterDisplaySizePreset sizePreset = CharacterDisplaySizePreset.Size16x2;

    [PropertyDisplay("Columns", "Size", 2)]
    [PropertyRange(1, 40, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private int columns = 16;

    [PropertyDisplay("Rows", "Size", 3)]
    [PropertyRange(1, 4, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private int rows = 2;

    [PropertyDisplay("Kind", "Appearance", 1)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyPresetGhostOpacity))]
    private CharacterDisplayKind kind = CharacterDisplayKind.Lcd;

    [PropertyDisplay("Color", "Appearance", 2)]
    [ObservableProperty]
    private CharacterDisplayColor color = CharacterDisplayColor.YellowGreen;

    [PropertyDisplay("Backlight On", "Appearance", 3)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyPresetGhostOpacity))]
    private bool isBacklightOn = true;

    [PropertyDisplay("Brightness", "Appearance", 4)]
    [PropertyRange(0.25, 1, 0.25)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double brightness = 1.0;

    [PropertyDisplay("Ghost Dot Opacity", "Appearance", 5)]
    [PropertyRange(0, 0.5, 0.01)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double ghostOpacity = LcdGhostOpacity;

    [PropertyDisplay("Show Bezel", "Appearance", 6)]
    [ObservableProperty]
    private bool showBezel = true;

    [PropertyDisplay("Text", "Content", 1)]
    [PropertyEditorHint(EditorHint.MultiLineText)]
    [ObservableProperty]
    private string text = "Hello, ESP32!\nAtc.Wpf LCD 16x2";

    [PropertyDisplay("Character ROM", "Content", 2)]
    [ObservableProperty]
    private CharacterRom characterRom = CharacterRom.A00;

    [PropertyDisplay("Cursor Mode", "Content", 3)]
    [ObservableProperty]
    private CharacterDisplayCursor cursorMode = CharacterDisplayCursor.None;

    [PropertyDisplay("Cursor Row", "Content", 4)]
    [PropertyRange(0, 3, 1)]
    [ObservableProperty]
    private int cursorRow;

    [PropertyDisplay("Cursor Column", "Content", 5)]
    [PropertyRange(0, 39, 1)]
    [ObservableProperty]
    private int cursorColumn;

    [PropertyDisplay("Display On", "Content", 6)]
    [ObservableProperty]
    private bool isDisplayOn = true;

    [PropertyDisplay("Replay Interval (ms)", "Emulator", 1)]
    [PropertyRange(10, 500, 10)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyReplayInterval))]
    private int replayInterval = 60;

    [ObservableProperty]
    private IList<byte[]>? customCharacters;

    [ObservableProperty]
    private string replayStatus = "No capture loaded.";

    public CharacterDisplayDemoViewModel()
    {
        replayTimer = new DispatcherTimer(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(replayInterval),
        };

        replayTimer.Tick += OnReplayTimerTick;
    }

    /// <summary>
    /// Gets the display state shared by the control and the emulators (an <see cref="Hd44780Controller"/>, fed directly or through a PCF8574 decoder).
    /// </summary>
    public CharacterDisplayBuffer Buffer { get; } = new(16, 2);

    [RelayCommand]
    private void ShowThermometer()
    {
        PauseReplay();
        CustomCharacters = CreateCustomCharacters();
        Text = "\u0001 Temperature\n  21.5\u0000C";
    }

    [RelayCommand]
    private void ShowBarGraph()
    {
        PauseReplay();
        CustomCharacters = CreateCustomCharacters();

        barGraphLevel = (barGraphLevel + BarGraphStep) % 101;

        var dots = barGraphLevel * Columns * DotsPerCell / 100;
        var bar = new StringBuilder(Columns);
        bar.Append((char)(FirstBarSlot + DotsPerCell - 1), dots / DotsPerCell);
        if (dots % DotsPerCell > 0)
        {
            bar.Append((char)(FirstBarSlot + (dots % DotsPerCell) - 1));
        }

        var label = string.Create(CultureInfo.InvariantCulture, $"Level {barGraphLevel,3} %");
        Text = Rows == 1
            ? bar.ToString()
            : label + "\n" + bar;
    }

    [RelayCommand]
    private void ReplayLogicalCapture()
    {
        // Play while the same capture plays does nothing.
        if (replayTimer.IsEnabled && !isLastReplayI2c)
        {
            return;
        }

        LoadLogicalCapture();
        replayTimer.Start();
    }

    [RelayCommand]
    private void ReplayI2cCapture()
    {
        if (replayTimer.IsEnabled && isLastReplayI2c)
        {
            return;
        }

        LoadI2cCapture();
        replayTimer.Start();
    }

    [RelayCommand]
    private void Step()
    {
        replayTimer.Stop();

        if (replayIndex >= replaySteps.Count)
        {
            if (isLastReplayI2c)
            {
                LoadI2cCapture();
            }
            else
            {
                LoadLogicalCapture();
            }
        }

        ApplyNextStep();
    }

    private static List<byte[]> CreateCustomCharacters()
    {
        var glyphs = new List<byte[]>
        {
            CharacterDisplayCaptures.DegreeGlyph.ToArray(),
            ThermometerGlyph,
        };

        // Slots 2 to 6: a bar segment 1 to 5 dots wide, on rows 1 to 6.
        for (var width = 1; width <= DotsPerCell; width++)
        {
            var bits = (byte)(0x1F & ~(0x1F >> width));
            glyphs.Add([0x00, bits, bits, bits, bits, bits, bits, 0x00]);
        }

        return glyphs;
    }

    private static string DescribeLogical(Hd44780CaptureStep step)
        => step.IsData
            ? string.Create(CultureInfo.InvariantCulture, $"Data 0x{step.Value:X2} '{(step.Value < 0x20 ? '?' : (char)step.Value)}'")
            : string.Create(CultureInfo.InvariantCulture, $"Command 0x{step.Value:X2}");

    private static string DescribeI2c(byte[] chunk)
        => "I2C " + string.Join(' ', chunk.Select(b => b.ToString("X2", CultureInfo.InvariantCulture)));

    private void ApplySizePreset()
    {
        (Columns, Rows) = SizePreset switch
        {
            CharacterDisplaySizePreset.Size8x1 => (8, 1),
            CharacterDisplaySizePreset.Size8x2 => (8, 2),
            CharacterDisplaySizePreset.Size10x2 => (10, 2),
            CharacterDisplaySizePreset.Size16x1 => (16, 1),
            CharacterDisplaySizePreset.Size16x4 => (16, 4),
            CharacterDisplaySizePreset.Size20x2 => (20, 2),
            CharacterDisplaySizePreset.Size20x4 => (20, 4),
            CharacterDisplaySizePreset.Size24x2 => (24, 2),
            CharacterDisplaySizePreset.Size40x2 => (40, 2),
            CharacterDisplaySizePreset.Size40x4 => (40, 4),
            _ => (16, 2),
        };
    }

    // A bound GhostOpacity wins over the preset of the control, so the sample follows the preset values itself.
    // The constants copy the internal CharacterDisplayPalette values; keep them in sync.
    private void ApplyPresetGhostOpacity()
    {
        GhostOpacity = Kind == CharacterDisplayKind.Vfd
            ? VfdGhostOpacity
            : IsBacklightOn
                ? LcdGhostOpacity
                : ReflectiveGhostOpacity;
    }

    private void ApplyReplayInterval()
        => replayTimer.Interval = TimeSpan.FromMilliseconds(ReplayInterval);

    private void LoadLogicalCapture()
    {
        var controller = PrepareReplay(isI2c: false);
        foreach (var step in CharacterDisplayCaptures.CreateLogicalCapture())
        {
            replaySteps.Add((DescribeLogical(step), () => controller.Write([step.Value], step.IsData)));
        }
    }

    private void LoadI2cCapture()
    {
        var controller = PrepareReplay(isI2c: true);
        var i2cDecoder = new Pcf8574Hd44780Decoder(controller);
        i2cDecoder.BacklightChanged += OnDecoderBacklightChanged;
        decoder = i2cDecoder;

        foreach (var chunk in CharacterDisplayCaptures.CreateI2cCapture())
        {
            replaySteps.Add((DescribeI2c(chunk), () => i2cDecoder.Write(chunk)));
        }
    }

    // The capture is a 20 x 4 screen. The size is set before Text is cleared, so a resize does not write the old text over the replay.
    private Hd44780Controller PrepareReplay(bool isI2c)
    {
        StopReplay();

        // The decoder starts with the backlight on, as does a capture of HD44780 bytes.
        IsBacklightOn = true;

        // Columns and Rows are set too: the preset does not change (no callback) when the sliders moved away from it.
        SizePreset = CharacterDisplaySizePreset.Size20x4;
        Columns = 20;
        Rows = 4;
        Text = string.Empty;

        replaySteps.Clear();
        replayIndex = 0;
        isLastReplayI2c = isI2c;
        ReplayStatus = isI2c
            ? "PCF8574 (I2C) capture loaded."
            : "HD44780 capture loaded.";

        return new Hd44780Controller(Buffer);
    }

    /// <summary>
    /// Stops a running replay and releases the decoder; the view calls it when it unloads.
    /// </summary>
    public void StopReplay()
    {
        replayTimer.Stop();
        replaySteps.Clear();
        replayIndex = 0;

        if (decoder is not null)
        {
            decoder.BacklightChanged -= OnDecoderBacklightChanged;
            decoder = null;
        }
    }

    private void PauseReplay()
        => replayTimer.Stop();

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

    private void OnReplayTimerTick(
        object? sender,
        EventArgs e)
        => ApplyNextStep();

    // Raised on the thread that wrote the bytes: here the UI thread of the replay timer.
    private void OnDecoderBacklightChanged(
        object? sender,
        EventArgs e)
    {
        if (sender is Pcf8574Hd44780Decoder source)
        {
            IsBacklightOn = source.IsBacklightOn;
        }
    }
}