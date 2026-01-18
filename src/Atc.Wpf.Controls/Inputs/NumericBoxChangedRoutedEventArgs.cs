namespace Atc.Wpf.Controls.Inputs;

public sealed class NumericBoxChangedRoutedEventArgs(
    RoutedEvent routedEvent,
    double interval)
    : RoutedEventArgs(routedEvent)
{
    public double Interval { get; set; } = interval;
}