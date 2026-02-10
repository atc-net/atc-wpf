namespace Atc.Wpf.Components.Flyouts;

/// <summary>
/// Provides data for the <see cref="Flyout.Closing"/> event.
/// </summary>
public class FlyoutClosingEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlyoutClosingEventArgs"/> class.
    /// </summary>
    public FlyoutClosingEventArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlyoutClosingEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event identifier.</param>
    public FlyoutClosingEventArgs(RoutedEvent routedEvent)
        : base(routedEvent)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlyoutClosingEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event identifier.</param>
    /// <param name="source">The source of the event.</param>
    public FlyoutClosingEventArgs(
        RoutedEvent routedEvent,
        object source)
        : base(routedEvent, source)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether the flyout closing should be cancelled.
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the close was triggered by light dismiss
    /// (clicking outside the flyout or pressing Escape).
    /// </summary>
    public bool IsLightDismiss { get; set; }
}