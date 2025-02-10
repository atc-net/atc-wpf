namespace Atc.Wpf.Controls.BaseControls;

public sealed class NumericBoxChangedRoutedEventArgs : RoutedEventArgs
{
    public double Interval { get; set; }

    public NumericBoxChangedRoutedEventArgs(
        RoutedEvent routedEvent,
        double interval)
        : base(routedEvent)
    {
        Interval = interval;
    }
}