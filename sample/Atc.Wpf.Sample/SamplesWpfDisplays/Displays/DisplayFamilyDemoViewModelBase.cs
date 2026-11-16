namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// The feed modes, frame stream and capture replay shared by the e-paper, LED matrix, seven-segment and graphic LCD samples,
/// with the lifecycle of the OLED sample: nothing runs while the view is unloaded, and a replay restarts only if it was running.
/// </summary>
public abstract partial class DisplayFamilyDemoViewModelBase : ViewModelBase
{
    private const int MaxDescribedBytes = 8;

    private readonly DispatcherTimer frameTimer;
    private readonly DispatcherTimer replayTimer;
    private readonly List<(string Description, Action Apply)> replaySteps = [];
    private IDisposable? replayOwner;
    private int replayIndex;
    private bool isLoaded;

    [PropertyDisplay("Feed Mode", "Content", 1)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyFeedMode))]
    private GraphicDisplayFeedMode feedMode = GraphicDisplayFeedMode.Text;

    [PropertyDisplay("Replay Interval (ms)", "Replay", 10)]
    [PropertyRange(5, 1000, 5)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty(AfterChangedCallback = nameof(ApplyReplayInterval))]
    private int replayInterval = 60;

    [ObservableProperty]
    private string replayStatus = "Choose the Replay feed mode to play a capture.";

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayFamilyDemoViewModelBase"/> class.
    /// </summary>
    /// <param name="frameInterval">The interval of the frame stream.</param>
    protected DisplayFamilyDemoViewModelBase(TimeSpan frameInterval)
    {
        frameTimer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = frameInterval };
        frameTimer.Tick += (_, _) => OnFrameTick();

        replayTimer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromMilliseconds(replayInterval) };
        replayTimer.Tick += (_, _) => ApplyNextStep();

        StartReplayCommand = new RelayCommand(StartReplay);
        StopReplayCommand = new RelayCommand(() => replayTimer.Stop());
        StepReplayCommand = new RelayCommand(StepReplay);
    }

    [PropertyDisplay("Start Replay", "Replay", 11)]
    public ICommand StartReplayCommand { get; }

    [PropertyDisplay("Stop Replay", "Replay", 12)]
    public ICommand StopReplayCommand { get; }

    [PropertyDisplay("Step", "Replay", 13)]
    public ICommand StepReplayCommand { get; }

    /// <summary>
    /// Restarts the feed of the current mode; the view calls it when it is loaded.
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
            if (replayOwner is null)
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

    /// <summary>
    /// Describes a run of bytes for the replay status.
    /// </summary>
    /// <param name="prefix">What the bytes are.</param>
    /// <param name="bytes">The bytes.</param>
    /// <returns>The description.</returns>
    protected static string DescribeBytes(
        string prefix,
        IReadOnlyCollection<byte> bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);

        var shown = string.Join(' ', bytes.Take(MaxDescribedBytes).Select(b => b.ToString("X2", CultureInfo.InvariantCulture)));
        return bytes.Count > MaxDescribedBytes
            ? string.Create(CultureInfo.InvariantCulture, $"{prefix} {shown} ... ({bytes.Count} bytes)")
            : $"{prefix} {shown}";
    }

    /// <summary>
    /// Called after the feed mode changed and the timers stopped, before the frame stream or the replay starts: switch the buffer the control shows.
    /// </summary>
    protected abstract void OnFeedModeChanged();

    /// <summary>
    /// Draws the next frame of the frame stream.
    /// </summary>
    protected abstract void OnFrameTick();

    /// <summary>
    /// Creates a fresh emulator in its power-on state and the steps of the capture to replay into it.
    /// </summary>
    /// <param name="steps">The steps to fill: a description and the action that applies it.</param>
    /// <returns>The emulator to dispose when the replay ends, and the status text.</returns>
    protected abstract (IDisposable Owner, string Status) CreateReplay(
        ICollection<(string Description, Action Apply)> steps);

    /// <summary>
    /// Loads the capture again if the Replay mode is active (e.g. after the module changed); a paused replay stays paused.
    /// </summary>
    protected void ReloadReplayIfActive()
    {
        if (FeedMode != GraphicDisplayFeedMode.Replay)
        {
            return;
        }

        var wasRunning = replayTimer.IsEnabled;
        LoadReplay();
        if (wasRunning && isLoaded)
        {
            replayTimer.Start();
        }
    }

    private void ApplyFeedMode()
    {
        frameTimer.Stop();
        StopReplay();
        OnFeedModeChanged();

        switch (FeedMode)
        {
            case GraphicDisplayFeedMode.Frame:
                ReplayStatus = "Streaming frames into the buffer, as a device that sends its frame buffer.";
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
            default:
                ReplayStatus = "Choose the Replay feed mode to play a capture.";
                break;
        }
    }

    private void ApplyReplayInterval()
        => replayTimer.Interval = TimeSpan.FromMilliseconds(ReplayInterval);

    private void StartReplay()
    {
        if (FeedMode != GraphicDisplayFeedMode.Replay)
        {
            FeedMode = GraphicDisplayFeedMode.Replay;
            return;
        }

        if (replayOwner is null || replayIndex >= replaySteps.Count)
        {
            LoadReplay();
        }

        replayTimer.Start();
    }

    private void StepReplay()
    {
        if (FeedMode != GraphicDisplayFeedMode.Replay)
        {
            return;
        }

        replayTimer.Stop();
        if (replayOwner is null || replayIndex >= replaySteps.Count)
        {
            LoadReplay();
        }

        ApplyNextStep();
    }

    private void LoadReplay()
    {
        StopReplay();
        var (owner, status) = CreateReplay(replaySteps);
        replayOwner = owner;
        ReplayStatus = string.Create(CultureInfo.InvariantCulture, $"{status}: {replaySteps.Count} steps.");
    }

    private void StopReplay()
    {
        replayTimer.Stop();
        replaySteps.Clear();
        replayIndex = 0;
        replayOwner?.Dispose();
        replayOwner = null;
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
        ReplayStatus = string.Create(CultureInfo.InvariantCulture, $"Step {replayIndex} of {replaySteps.Count}: {description}");

        if (replayIndex >= replaySteps.Count)
        {
            replayTimer.Stop();
        }
    }
}