namespace Atc.Wpf.Controls.Zoom;

/// <summary>
/// A named viewport state that can be saved and restored for quick navigation.
/// </summary>
/// <param name="Name">A human-readable name for this bookmark (e.g., "Overview", "Detail A").</param>
/// <param name="State">The captured viewport state.</param>
public sealed record ViewBookmark(
    string Name,
    ViewportState State);