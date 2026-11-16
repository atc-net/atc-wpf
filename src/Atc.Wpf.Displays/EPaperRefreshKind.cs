namespace Atc.Wpf.Displays;

/// <summary>
/// The kind of an e-paper refresh: how the panel moves from the old picture to the new one.
/// </summary>
public enum EPaperRefreshKind
{
    /// <summary>
    /// A full refresh: the panel flashes (inverted, white, then the picture) and clears any ghosting.
    /// </summary>
    Full,

    /// <summary>
    /// A partial (fast) refresh: the panel changes only the pixels that differ, without a flash.
    /// </summary>
    Partial,
}