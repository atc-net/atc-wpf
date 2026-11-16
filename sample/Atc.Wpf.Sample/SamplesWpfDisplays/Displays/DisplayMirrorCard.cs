namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// One display id of the display mirror sample: the control that shows it, the target the receiver applies its packets to and its statistics.
/// </summary>
public abstract partial class DisplayMirrorCard : ViewModelBase, IDisposable
{
    private long previousPackets;
    private DateTimeOffset previousSample;

    [ObservableProperty]
    private string statisticsText = "No packets yet.";

    [ObservableProperty]
    private string lastRejection = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayMirrorCard"/> class.
    /// </summary>
    /// <param name="displayId">The display id.</param>
    /// <param name="describe">The description the card was made from, or <see langword="null"/> when none arrived.</param>
    protected DisplayMirrorCard(
        byte displayId,
        DisplayMirrorDescribe? describe)
    {
        DisplayId = displayId;
        Describe = describe;
    }

    /// <summary>
    /// Gets the display id.
    /// </summary>
    public byte DisplayId { get; }

    /// <summary>
    /// Gets the description the card was made from, or <see langword="null"/> when none arrived.
    /// </summary>
    public DisplayMirrorDescribe? Describe { get; }

    /// <summary>
    /// Gets the card header.
    /// </summary>
    public string Header
        => Describe is { } describe
            ? string.Create(
                CultureInfo.InvariantCulture,
                $"#{DisplayId} {describe.Name} ({describe.Kind}, {describe.Chip}, {describe.Width} x {describe.Height})")
            : string.Create(CultureInfo.InvariantCulture, $"#{DisplayId} (no Describe yet)");

    /// <summary>
    /// Gets the target the receiver applies the packets of <see cref="DisplayId"/> to, or <see langword="null"/> for a card that only shows statistics.
    /// </summary>
    public abstract IDisplayMirrorTarget? Target { get; }

    /// <summary>
    /// Shows a statistics snapshot; called on the UI thread a few times a second.
    /// </summary>
    /// <param name="statistics">The statistics of <see cref="DisplayId"/>.</param>
    /// <param name="now">The current time.</param>
    public void UpdateStatistics(
        DisplayMirrorStatistics statistics,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(statistics);

        var elapsed = (now - previousSample).TotalSeconds;
        var rate = previousSample != default && elapsed > 0
            ? (statistics.Packets - previousPackets) / elapsed
            : 0;
        previousPackets = statistics.Packets;
        previousSample = now;

        var last = statistics.LastPacket is { } lastPacket
            ? string.Create(CultureInfo.InvariantCulture, $"{(now - lastPacket).TotalSeconds:0.0} s ago")
            : "never";

        StatisticsText = string.Create(
            CultureInfo.InvariantCulture,
            $"{statistics.Packets} packets ({rate:0.0}/s), {statistics.Bytes / 1024.0:0.0} KiB, {statistics.Unsupported} unsupported, {statistics.Rejected} rejected, last {last}");
    }

    /// <summary>
    /// Shows why the latest packet of <see cref="DisplayId"/> was rejected.
    /// </summary>
    /// <param name="reason">The reason.</param>
    public void ShowRejection(DisplayMirrorRejectReason reason)
        => LastRejection = string.Create(CultureInfo.InvariantCulture, $"Last rejected: {reason}");

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the emulators of the card.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> when called from <see cref="Dispose()"/>.</param>
    protected virtual void Dispose(bool disposing)
    {
    }
}