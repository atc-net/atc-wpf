namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

public sealed partial class EPaperDisplayDemoViewModel : DisplayFamilyDemoViewModelBase
{
    private const int FullRefreshEvery = 5;

    // Frame mode streams into one buffer and Replay mode emulates the chip on another, so the two never disturb each other.
    private readonly EPaperFrameBuffer frameBuffer = new();
    private readonly EPaperFrameBuffer replayBuffer = new();
    private int frameCount;

    [ObservableProperty(AfterChangedCallback = nameof(ApplyPreset))]
    private EPaperDisplayPreset selectedPreset = EPaperDisplayPreset.All[2];

    [PropertyDisplay("Size and Colours", "Module", 1)]
    [PropertyEditorHint(EditorHint.ReadOnly)]
    [ObservableProperty]
    private string moduleInfo = "128 x 296 native, black/white";

    [ObservableProperty]
    private int pixelColumns = 128;

    [ObservableProperty]
    private int pixelRows = 296;

    // Null shows every column; the 2.13" panels show 122 of their 128-pixel-wide rows.
    [ObservableProperty]
    private int? visibleColumns;

    [PropertyDisplay("Black/White/Red", "Module", 2)]
    [ObservableProperty]
    private bool hasRed;

    [PropertyDisplay("Rotation (GxEPD2 setRotation x 90)", "Module", 3)]
    [PropertyRange(0, 270, 90)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private int rotation = 90;

    [PropertyDisplay("Show Bezel", "Module", 4)]
    [ObservableProperty]
    private bool showBezel = true;

    [PropertyDisplay("Show Refresh Flash", "Refresh", 1)]
    [ObservableProperty]
    private bool showRefreshFlash = true;

    [PropertyDisplay("Refresh Flash (ms)", "Refresh", 2)]
    [PropertyRange(0, 3000, 50)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty(DependentPropertyNames = [nameof(RefreshFlashDuration)])]
    private int refreshFlashMilliseconds = 600;

    [PropertyDisplay("Text", "Content", 2)]
    [PropertyEditorHint(EditorHint.MultiLineText)]
    [ObservableProperty(DependentPropertyNames = [nameof(DisplayText)])]
    private string text = "Hello, ESP32!\n\nAtc.Wpf e-paper\nSSD1680";

    public EPaperDisplayDemoViewModel()
        : base(TimeSpan.FromSeconds(1))
    {
    }

    /// <summary>
    /// Gets the presets of the common modules.
    /// </summary>
    public IReadOnlyList<EPaperDisplayPreset> Presets
        => EPaperDisplayPreset.All;

    /// <summary>
    /// Gets the buffer the control shows: <see langword="null"/> (the control's own, showing <see cref="Text"/>) in Text mode.
    /// </summary>
    public EPaperFrameBuffer? ActiveBuffer
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

    /// <summary>
    /// Gets the length of the refresh flash.
    /// </summary>
    public TimeSpan RefreshFlashDuration
        => TimeSpan.FromMilliseconds(RefreshFlashMilliseconds);

    /// <inheritdoc />
    protected override void OnFeedModeChanged()
    {
        // The text is emptied before a view-model buffer is shown, and the control's own buffer is back before the text returns.
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
    }

    // As a device that sends EPaperFrame: the whole RAM, then a refresh; a full refresh every fifth frame, partial ones in between.
    protected override void OnFrameTick()
    {
        frameCount++;
        var (black, red) = EPaperDisplayCaptures.CreatePicture(
            frameBuffer.Width,
            VisibleColumns ?? frameBuffer.Width,
            frameBuffer.Height,
            frameBuffer.HasRed,
            ["Frame stream", string.Create(CultureInfo.InvariantCulture, $"Frame {frameCount}")]);

        using (frameBuffer.BeginBatch())
        {
            frameBuffer.LoadRam(black, red);
            frameBuffer.Refresh(frameCount % FullRefreshEvery == 1 ? EPaperRefreshKind.Full : EPaperRefreshKind.Partial);
        }
    }

    // A new controller applies the chip's power-on state (white RAM, no red), as on a module that was just powered.
    protected override (IDisposable Owner, string Status) CreateReplay(
        ICollection<(string Description, Action Apply)> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);

        var preset = SelectedPreset;
        var chip = new Ssd1680Controller(replayBuffer);
        foreach (var step in EPaperDisplayCaptures.CreateGxEpd2Capture(preset))
        {
            steps.Add(step.IsData
                ? (DescribeBytes("SPI data", step.Bytes), () => chip.WriteData(step.Bytes))
                : (DescribeBytes("SPI command", step.Bytes), () => chip.WriteCommands(step.Bytes)));
        }

        return (chip, $"{preset.LibraryClass} capture over SPI loaded");
    }

    private void ApplyPreset()
    {
        var preset = SelectedPreset;

        frameBuffer.Resize(preset.Columns, preset.Rows);
        replayBuffer.Resize(preset.Columns, preset.Rows);

        PixelColumns = preset.Columns;
        VisibleColumns = preset.VisibleColumns == preset.Columns ? null : preset.VisibleColumns;
        PixelRows = preset.Rows;
        HasRed = preset.HasRed;
        ModuleInfo = string.Create(
            CultureInfo.InvariantCulture,
            $"{preset.VisibleColumns} x {preset.Rows} native ({preset.Columns}-pixel rows), {(preset.HasRed ? "black/white/red" : "black/white")}");

        ReloadReplayIfActive();
    }
}