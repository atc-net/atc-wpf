namespace Atc.Wpf.Components.Monitoring;

/// <summary>
/// Carries an <see cref="ApplicationMonitorScrollEvent"/> from an <see cref="ApplicationMonitorViewModel"/> to the view
/// bound to it.
/// </summary>
/// <param name="scrollEvent">The scroll request.</param>
internal sealed class ApplicationMonitorScrollRequestedEventArgs(
    ApplicationMonitorScrollEvent scrollEvent) : EventArgs
{
    /// <summary>
    /// Gets the scroll request.
    /// </summary>
    public ApplicationMonitorScrollEvent ScrollEvent { get; } = scrollEvent;
}