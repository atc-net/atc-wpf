namespace Atc.Wpf.Controls.Inputs;

/// <summary>Provides data for the <see cref="NumericBox"/> increment and decrement events.</summary>
/// <param name="routedEvent">The routed event identifier.</param>
/// <param name="interval">The interval by which the value changed.</param>
public sealed class NumericBoxChangedRoutedEventArgs(
    RoutedEvent routedEvent,
    double interval)
    : RoutedEventArgs(routedEvent)
{
    /// <summary>Gets or sets the interval by which the value changed.</summary>
    public double Interval { get; set; } = interval;
}