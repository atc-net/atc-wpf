namespace Atc.Wpf.Controls.DataGrids;

/// <summary>
/// Provides data for the <see cref="DataGridFilter"/> <c>FilterChanged</c> attached event.
/// </summary>
public sealed class DataGridFilterChangedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridFilterChangedEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="filters">The active column filters.</param>
    /// <param name="searchText">The applied search text.</param>
    /// <param name="itemCount">The number of rows that pass, or -1 when unknown.</param>
    public DataGridFilterChangedEventArgs(
        RoutedEvent routedEvent,
        IReadOnlyList<DataGridColumnFilter> filters,
        string searchText,
        int itemCount)
        : base(routedEvent)
    {
        Filters = filters;
        SearchText = searchText;
        ItemCount = itemCount;
    }

    /// <summary>
    /// Gets the active column filters.
    /// </summary>
    public IReadOnlyList<DataGridColumnFilter> Filters { get; }

    /// <summary>
    /// Gets the applied search text (empty when no search is active).
    /// </summary>
    public string SearchText { get; }

    /// <summary>
    /// Gets the number of rows that pass all filters, or -1 when the view does not tell.
    /// </summary>
    public int ItemCount { get; }
}