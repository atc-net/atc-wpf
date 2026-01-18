namespace Atc.Wpf.Controls.Inputs;

[SuppressMessage("Design", "CA1003:Use generic event handler instances", Justification = "OK.")]
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
public delegate void NumericBoxChangedRoutedEventHandler(
    object sender,
    NumericBoxChangedRoutedEventArgs args);