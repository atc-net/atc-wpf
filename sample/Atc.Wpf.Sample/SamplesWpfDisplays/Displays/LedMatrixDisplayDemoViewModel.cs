namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

public sealed partial class LedMatrixDisplayDemoViewModel : DisplayFamilyDemoViewModelBase
{
    private const string ScrollText = "Atc.Wpf LED matrix * ";

    // Frame mode streams into one buffer and Replay mode emulates the chips on another, so the two never disturb each other.
    private readonly Max7219ChainBuffer frameBuffer = new();
    private readonly Max7219ChainBuffer replayBuffer = new();
    private int scrollOffset;

    [ObservableProperty(AfterChangedCallback = nameof(ApplyPreset))]
    private LedMatrixDisplayPreset selectedPreset = LedMatrixDisplayPreset.All[1];

    [PropertyDisplay("Modules", "Module", 1)]
    [PropertyRange(1, 8, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyModuleCount))]
    private int moduleCount = 4;

    [PropertyDisplay("Wiring", "Module", 2)]
    [ObservableProperty(AfterChangedCallback = nameof(ReloadReplayIfActive))]
    private Max7219MatrixWiring wiring = Max7219MatrixWiring.Fc16;

    [PropertyDisplay("LED Color", "Module", 3)]
    [ObservableProperty]
    private LedColor ledColor = LedColor.Red;

    [PropertyDisplay("Show Bezel", "Module", 4)]
    [ObservableProperty]
    private bool showBezel = true;

    [PropertyDisplay("Text", "Content", 2)]
    [ObservableProperty(DependentPropertyNames = [nameof(DisplayText)])]
    private string text = "Hi ESP32";

    [PropertyDisplay("Character ROM", "Content", 3)]
    [ObservableProperty]
    private CharacterRom characterRom = CharacterRom.A00;

    [PropertyDisplay("Library", "Replay", 1)]
    [ObservableProperty(AfterChangedCallback = nameof(ReloadReplayIfActive))]
    private LedMatrixReplayLibrary replayLibrary = LedMatrixReplayLibrary.MdMax72xx;

    public LedMatrixDisplayDemoViewModel()
        : base(TimeSpan.FromMilliseconds(60))
    {
    }

    /// <summary>
    /// Gets the presets of the common modules.
    /// </summary>
    public IReadOnlyList<LedMatrixDisplayPreset> Presets
        => LedMatrixDisplayPreset.All;

    /// <summary>
    /// Gets the buffer the control shows: <see langword="null"/> (the control's own, showing <see cref="Text"/>) in Text mode.
    /// </summary>
    public Max7219ChainBuffer? ActiveBuffer
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

        if (FeedMode == GraphicDisplayFeedMode.Frame)
        {
            // As a sketch that keeps its own registers: every device on, scan limit 7, mid intensity, no decode.
            using (frameBuffer.BeginBatch())
            {
                for (var device = 0; device < frameBuffer.DeviceCount; device++)
                {
                    frameBuffer.SetShutdown(device, isShutdown: false);
                    frameBuffer.SetDisplayTest(device, isDisplayTest: false);
                    frameBuffer.SetScanLimit(device, 7);
                    frameBuffer.SetIntensity(device, 7);
                    frameBuffer.SetDecodeMode(device, 0);
                }
            }
        }
    }

    // Scrolls the text one LED column per frame, written straight into the registers in the module's wiring.
    protected override void OnFrameTick()
    {
        scrollOffset++;
        var registers = LedMatrixDisplayCaptures.CreateRegisters(ScrollText, frameBuffer.DeviceCount, Wiring, scrollOffset);
        using (frameBuffer.BeginBatch())
        {
            for (var device = 0; device < registers.Length; device++)
            {
                for (var digit = 0; digit < 8; digit++)
                {
                    frameBuffer.SetDigit(device, digit, registers[device][digit]);
                }
            }
        }
    }

    // A new controller applies the power-on state: every chip shut down, blank digits.
    protected override (IDisposable Owner, string Status) CreateReplay(
        ICollection<(string Description, Action Apply)> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);

        var chain = new Max7219Controller(replayBuffer);
        foreach (var frame in LedMatrixDisplayCaptures.CreateCapture(ReplayLibrary, ModuleCount, Wiring))
        {
            steps.Add((DescribeBytes("Latch", frame), () => chain.WriteData(frame)));
        }

        return (chain, $"{ReplayLibrary} capture loaded ({ModuleCount} devices)");
    }

    private void ApplyPreset()
    {
        var preset = SelectedPreset;
        ModuleCount = preset.ModuleCount;
        Wiring = preset.Wiring;
        LedColor = preset.LedColor;
    }

    private void ApplyModuleCount()
    {
        frameBuffer.Resize(ModuleCount);
        replayBuffer.Resize(ModuleCount);
        if (FeedMode == GraphicDisplayFeedMode.Frame)
        {
            OnFeedModeChanged();
        }

        ReloadReplayIfActive();
    }
}