namespace Atc.Wpf.Components.Flyouts;

/// <summary>
/// Provides data for the <see cref="Flyout.Opening"/> event.
/// </summary>
public class FlyoutOpeningEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlyoutOpeningEventArgs"/> class.
    /// </summary>
    public FlyoutOpeningEventArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlyoutOpeningEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event identifier.</param>
    public FlyoutOpeningEventArgs(RoutedEvent routedEvent)
        : base(routedEvent)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlyoutOpeningEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event identifier.</param>
    /// <param name="source">The source of the event.</param>
    public FlyoutOpeningEventArgs(
        RoutedEvent routedEvent,
        object source)
        : base(routedEvent, source)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether the flyout opening should be cancelled.
    /// </summary>
    public bool Cancel { get; set; }
}