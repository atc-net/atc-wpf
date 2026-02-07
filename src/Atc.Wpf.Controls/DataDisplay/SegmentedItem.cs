namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Represents a single segment in a <see cref="Segmented"/>.
/// </summary>
[ContentProperty(nameof(Content))]
public sealed partial class SegmentedItem : ContentControl
{
    /// <summary>
    /// Whether this segment is currently selected.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isSelected;

    /// <summary>
    /// Optional icon content displayed before the text.
    /// </summary>
    [DependencyProperty]
    private object? icon;

    /// <summary>
    /// An optional template for rendering the <see cref="Icon"/> content.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? iconTemplate;

    /// <summary>
    /// Whether this is the first segment in the container.
    /// Set by the parent <see cref="Segmented"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isFirst;

    /// <summary>
    /// Whether this is the last segment in the container.
    /// Set by the parent <see cref="Segmented"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isLast;

    /// <summary>
    /// The index of this segment within the container.
    /// Set by the parent <see cref="Segmented"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = -1)]
    private int index;

    /// <summary>
    /// Occurs when the segment is clicked.
    /// </summary>
    public event RoutedEventHandler? Click;

    static SegmentedItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(SegmentedItem),
            new FrameworkPropertyMetadata(typeof(SegmentedItem)));
    }

    /// <inheritdoc />
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);

        if (!IsEnabled)
        {
            return;
        }

        Click?.Invoke(this, new RoutedEventArgs());
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnKeyDown(e);

        if (e.Handled)
        {
            return;
        }

        if (e.Key is Key.Space or Key.Enter)
        {
            Click?.Invoke(this, new RoutedEventArgs());
            e.Handled = true;
        }
    }
}