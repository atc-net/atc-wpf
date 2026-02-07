namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Provides data for the <see cref="Breadcrumb.ItemClicked"/> event.
/// </summary>
public sealed class BreadcrumbItemClickedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BreadcrumbItemClickedEventArgs"/> class.
    /// </summary>
    /// <param name="item">The breadcrumb item that was clicked.</param>
    /// <param name="index">The index of the clicked item.</param>
    public BreadcrumbItemClickedEventArgs(
        BreadcrumbItem item,
        int index)
    {
        Item = item;
        Index = index;
    }

    /// <summary>
    /// Gets the breadcrumb item that was clicked.
    /// </summary>
    public BreadcrumbItem Item { get; }

    /// <summary>
    /// Gets the index of the clicked item within the breadcrumb.
    /// </summary>
    public int Index { get; }
}