namespace Atc.Wpf.Controls.BaseControls;

public sealed class NumericBoxChangedRoutedEventArgs(
    RoutedEvent routedEvent,
    double interval)
    : RoutedEventArgs(routedEvent)
{
    public double Interval { get; set; } = interval;
}