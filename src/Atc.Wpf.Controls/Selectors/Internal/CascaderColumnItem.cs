#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// A row of a <see cref="CascaderColumnList"/>. A click on the check box ticks the node; a click on the rest of a row
/// with children makes it active; a click anywhere on a leaf ticks it. Resting the mouse on it reports a hover to the
/// owner (for <see cref="CascaderExpandTrigger.Hover"/>).
/// </summary>
internal sealed class CascaderColumnItem : ContentControl
{
    /// <summary>
    /// Identifies the <see cref="IsActive"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive),
        typeof(bool),
        typeof(CascaderColumnItem),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Identifies the <see cref="IsFilterResult"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsFilterResultProperty = DependencyProperty.Register(
        nameof(IsFilterResult),
        typeof(bool),
        typeof(CascaderColumnItem),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Identifies the <see cref="FilterContent"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FilterContentProperty = DependencyProperty.Register(
        nameof(FilterContent),
        typeof(object),
        typeof(CascaderColumnItem),
        new PropertyMetadata(defaultValue: null));

    public CascaderColumnItem(CascaderColumnList owner)
    {
        OwnerList = owner;
    }

    /// <summary>
    /// Gets the list that holds this row.
    /// </summary>
    public CascaderColumnList OwnerList { get; }

    /// <summary>
    /// Gets or sets the node shown by this row.
    /// </summary>
    public CheckTreeNode? Node { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this row is the "please wait" row of a column whose nodes are loading.
    /// </summary>
    public bool IsPlaceholder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the row is active: its children are shown in the next column.
    /// </summary>
    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, BooleanBoxes.Box(value));
    }

    /// <summary>
    /// Gets or sets a value indicating whether the row is a filter result (it shows the highlighted path).
    /// </summary>
    public bool IsFilterResult
    {
        get => (bool)GetValue(IsFilterResultProperty);
        set => SetValue(IsFilterResultProperty, BooleanBoxes.Box(value));
    }

    /// <summary>
    /// Gets or sets the content shown for a filter result (the path with the match highlighted).
    /// </summary>
    public object? FilterContent
    {
        get => GetValue(FilterContentProperty);
        set => SetValue(FilterContentProperty, value);
    }

    /// <inheritdoc />
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseLeftButtonDown(e);
        if (e.Handled || IsPlaceholder || Node is not { } node)
        {
            return;
        }

        Focus();
        OwnerList.OwnerView.OnRowClicked(OwnerList, node, IsFromCheckArea(e.OriginalSource as DependencyObject));
        e.Handled = true;
    }

    /// <inheritdoc />
    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnGotKeyboardFocus(e);
        if (ReferenceEquals(e.NewFocus, this) && !IsPlaceholder && Node is { } node)
        {
            OwnerList.OwnerView.OnRowFocused(OwnerList, node);
        }
    }

    /// <inheritdoc />
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);
        if (!IsPlaceholder && Node is { } node)
        {
            OwnerList.OwnerView.OnRowHovered(node);
        }
    }

    /// <inheritdoc />
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        if (Node is { } node)
        {
            OwnerList.OwnerView.OnRowHoverLeft(node);
        }
    }

    private bool IsFromCheckArea(DependencyObject? source)
    {
        while (source is not null && !ReferenceEquals(source, this))
        {
            if (source is CascaderCheckArea)
            {
                return true;
            }

            source = source is Visual or System.Windows.Media.Media3D.Visual3D
                ? VisualTreeHelper.GetParent(source)
                : LogicalTreeHelper.GetParent(source);
        }

        return false;
    }
}