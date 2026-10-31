namespace Atc.Wpf.Components.Selectors;

/// <content>
/// Inherited values the row templates read instead of searching for the selector: a row that a bulk transfer
/// throws away is detached from the tree, and an ancestor search would then log a binding error.
/// </content>
public sealed partial class DualListSelector
{
    /// <summary>
    /// Identifies the RowItemTemplate attached property: the selector's <see cref="ItemTemplate"/>, inherited by its rows.
    /// </summary>
    public static readonly DependencyProperty RowItemTemplateProperty = DependencyProperty.RegisterAttached(
        "RowItemTemplate",
        typeof(DataTemplate),
        typeof(DualListSelector),
        new FrameworkPropertyMetadata(defaultValue: null, FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Identifies the RowSelectionStyle attached property: the selector's <see cref="ItemSelectionStyle"/>, inherited by its rows.
    /// </summary>
    public static readonly DependencyProperty RowSelectionStyleProperty = DependencyProperty.RegisterAttached(
        "RowSelectionStyle",
        typeof(DualListItemSelectionStyle),
        typeof(DualListSelector),
        new FrameworkPropertyMetadata(DualListItemSelectionStyle.Highlight, FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Identifies the RowIsOneWay attached property: the selector's <see cref="IsOneWay"/>, inherited by its rows.
    /// </summary>
    public static readonly DependencyProperty RowIsOneWayProperty = DependencyProperty.RegisterAttached(
        "RowIsOneWay",
        typeof(bool),
        typeof(DualListSelector),
        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Gets the item template a row inherits from its selector.
    /// </summary>
    /// <param name="element">The row element.</param>
    /// <returns>The inherited item template, or <see langword="null"/> for the default one.</returns>
    public static DataTemplate? GetRowItemTemplate(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (DataTemplate?)element.GetValue(RowItemTemplateProperty);
    }

    /// <summary>
    /// Sets the item template the rows below an element inherit. The selector sets it from <see cref="ItemTemplate"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The item template.</param>
    public static void SetRowItemTemplate(
        DependencyObject element,
        DataTemplate? value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(RowItemTemplateProperty, value);
    }

    /// <summary>
    /// Gets the selection style a row inherits from its selector.
    /// </summary>
    /// <param name="element">The row element.</param>
    /// <returns>The inherited selection style.</returns>
    public static DualListItemSelectionStyle GetRowSelectionStyle(
        DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (DualListItemSelectionStyle)element.GetValue(RowSelectionStyleProperty);
    }

    /// <summary>
    /// Sets the selection style the rows below an element inherit. The selector sets it from <see cref="ItemSelectionStyle"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The selection style.</param>
    public static void SetRowSelectionStyle(
        DependencyObject element,
        DualListItemSelectionStyle value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(RowSelectionStyleProperty, value);
    }

    /// <summary>
    /// Gets whether a row's selector is one-way.
    /// </summary>
    /// <param name="element">The row element.</param>
    /// <returns><see langword="true"/> when the selector is one-way.</returns>
    public static bool GetRowIsOneWay(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(RowIsOneWayProperty);
    }

    /// <summary>
    /// Sets whether the rows below an element belong to a one-way selector. The selector sets it from <see cref="IsOneWay"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value"><see langword="true"/> for one-way.</param>
    public static void SetRowIsOneWay(
        DependencyObject element,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(RowIsOneWayProperty, BooleanBoxes.Box(value));
    }

    private static void OnItemTemplateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => (d as DualListSelector)?.SyncRowValues();

    private void SyncRowValues()
    {
        SetRowItemTemplate(this, ItemTemplate);
        SetRowSelectionStyle(this, ItemSelectionStyle);
        SetRowIsOneWay(this, IsOneWay);
    }
}