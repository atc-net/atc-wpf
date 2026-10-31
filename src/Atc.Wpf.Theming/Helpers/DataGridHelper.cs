namespace Atc.Wpf.Theming.Helpers;

/// <summary>
/// Inherited attached properties that carry <see cref="DataGrid"/> settings down to the parts of the themed
/// <see cref="DataGridRow"/> template. The DataGrid style sets them on the grid (with bindings to the grid itself),
/// and the row template reads them through property value inheritance. A row the grid regenerates or recycles while
/// it is outside the grid's visual tree then simply sees the default value, where a
/// <c>RelativeSource FindAncestor</c> binding to the grid would log "Cannot find source for binding" (error 4).
/// </summary>
public static class DataGridHelper
{
    /// <summary>
    /// The visibility of the row headers, as <see cref="DataGrid.HeadersVisibility"/> gives it for rows
    /// (inherited; <see cref="Visibility.Collapsed"/> by default, as the themed grid shows column headers only).
    /// </summary>
    public static readonly DependencyProperty RowHeaderVisibilityProperty = DependencyProperty.RegisterAttached(
        "RowHeaderVisibility",
        typeof(Visibility),
        typeof(DataGridHelper),
        new FrameworkPropertyMetadata(Visibility.Collapsed, FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// The scrolling orientation of the row details, as <see cref="DataGrid.AreRowDetailsFrozen"/> gives it
    /// (inherited; <see cref="SelectiveScrollingOrientation.Both"/> by default, i.e. details that are not frozen).
    /// </summary>
    public static readonly DependencyProperty RowDetailsScrollingOrientationProperty = DependencyProperty.RegisterAttached(
        "RowDetailsScrollingOrientation",
        typeof(SelectiveScrollingOrientation),
        typeof(DataGridHelper),
        new FrameworkPropertyMetadata(SelectiveScrollingOrientation.Both, FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Gets the visibility of the row headers.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The visibility.</returns>
    public static Visibility GetRowHeaderVisibility(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (Visibility)element.GetValue(RowHeaderVisibilityProperty);
    }

    /// <summary>
    /// Sets the visibility of the row headers.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The visibility.</param>
    public static void SetRowHeaderVisibility(
        DependencyObject element,
        Visibility value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(RowHeaderVisibilityProperty, value);
    }

    /// <summary>
    /// Gets the scrolling orientation of the row details.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The scrolling orientation.</returns>
    public static SelectiveScrollingOrientation GetRowDetailsScrollingOrientation(
        DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (SelectiveScrollingOrientation)element.GetValue(RowDetailsScrollingOrientationProperty);
    }

    /// <summary>
    /// Sets the scrolling orientation of the row details.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The scrolling orientation.</param>
    public static void SetRowDetailsScrollingOrientation(
        DependencyObject element,
        SelectiveScrollingOrientation value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(RowDetailsScrollingOrientationProperty, value);
    }
}