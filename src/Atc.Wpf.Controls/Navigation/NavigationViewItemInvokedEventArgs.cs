namespace Atc.Wpf.Controls.Navigation;

/// <summary>
/// Provides data for the <see cref="NavigationView.ItemInvoked"/> event.
/// </summary>
public sealed class NavigationViewItemInvokedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewItemInvokedEventArgs"/> class.
    /// </summary>
    /// <param name="item">The invoked item.</param>
    public NavigationViewItemInvokedEventArgs(NavigationViewItem item)
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
    }

    /// <summary>
    /// Gets the invoked item.
    /// </summary>
    public NavigationViewItem Item { get; }
}