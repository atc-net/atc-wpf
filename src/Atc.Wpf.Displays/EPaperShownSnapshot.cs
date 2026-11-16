namespace Atc.Wpf.Displays;

/// <summary>
/// The state of an <see cref="EPaperFrameBuffer"/> read together with its shown planes by <see cref="EPaperFrameBuffer.CopyShown"/>.
/// </summary>
/// <param name="Width">The native width in pixels.</param>
/// <param name="Height">The native height in pixels.</param>
/// <param name="HasRed">Whether the red plane is shown.</param>
/// <param name="FullRefreshCount">The number of <see cref="EPaperRefreshKind.Full"/> refreshes so far.</param>
internal readonly record struct EPaperShownSnapshot(
    int Width,
    int Height,
    bool HasRed,
    long FullRefreshCount);