namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// A card for a display id the sample cannot show: no Describe arrived yet, or it names a kind, chip or size the sample has no control for.
/// It has no target, so the packets of its id count as rejected (no target); only the statistics are shown.
/// </summary>
public sealed class DisplayMirrorPlaceholderCard : DisplayMirrorCard
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayMirrorPlaceholderCard"/> class.
    /// </summary>
    /// <param name="displayId">The display id.</param>
    /// <param name="describe">The description, or <see langword="null"/> when none arrived.</param>
    /// <param name="reason">Why the display is not shown.</param>
    public DisplayMirrorPlaceholderCard(
        byte displayId,
        DisplayMirrorDescribe? describe,
        string reason)
        : base(displayId, describe)
        => Reason = reason;

    /// <summary>
    /// Gets why the display is not shown.
    /// </summary>
    public string Reason { get; }

    /// <inheritdoc />
    public override IDisplayMirrorTarget? Target => null;
}