namespace Atc.Wpf.Controls.Inputs;

/// <summary>Represents the method that handles the <see cref="NumericBox"/> increment and decrement events.</summary>
[SuppressMessage("Design", "CA1003:Use generic event handler instances", Justification = "OK.")]
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
public delegate void NumericBoxChangedRoutedEventHandler(
    object sender,
    NumericBoxChangedRoutedEventArgs args);