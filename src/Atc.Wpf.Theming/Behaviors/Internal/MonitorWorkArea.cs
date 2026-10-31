namespace Atc.Wpf.Theming.Behaviors.Internal;

/// <summary>
/// The bounds and the work area of one monitor, in physical pixels and screen coordinates.
/// </summary>
/// <param name="Bounds">The full bounds of the monitor.</param>
/// <param name="WorkArea">The part of the monitor not covered by the taskbar and docked tool bars.</param>
internal sealed record MonitorWorkArea(
    Rect Bounds,
    Rect WorkArea);