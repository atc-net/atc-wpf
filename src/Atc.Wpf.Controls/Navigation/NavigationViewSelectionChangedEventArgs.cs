namespace Atc.Wpf.Controls.Navigation;

/// <summary>
/// Provides data for the <see cref="NavigationView.SelectionChanged"/> event.
/// </summary>
public sealed class NavigationViewSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewSelectionChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldItem">The previously selected item, or null.</param>
    /// <param name="newItem">The newly selected item, or null.</param>
    public NavigationViewSelectionChangedEventArgs(
        NavigationViewItem? oldItem,
        NavigationViewItem? newItem)
    {
        OldItem = oldItem;
        NewItem = newItem;
    }

    /// <summary>
    /// Gets the previously selected item, or null.
    /// </summary>
    public NavigationViewItem? OldItem { get; }

    /// <summary>
    /// Gets the newly selected item, or null.
    /// </summary>
    public NavigationViewItem? NewItem { get; }
}