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
    [DependencyProperty(DefaultValue = false, PropertyChangedCallback = nameof(OnIsSelectedChanged))]
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
    /// The key of this segment in <see cref="Segmented.SelectedKeys"/>. When not set, the text of
    /// <see cref="ContentControl.Content"/> is used.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnKeyChanged))]
    private string? key;

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
    /// Whether this segment is part of a joined row (<see langword="true"/>) or a separate pill button.
    /// Set by the parent <see cref="Segmented"/> from <see cref="Segmented.IsJoined"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isJoined;

    /// <summary>
    /// Whether only the icon is shown; the text becomes the tooltip and the UI Automation name.
    /// Set by the parent <see cref="Segmented"/> from <see cref="Segmented.IconOnly"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isIconOnly;

    /// <summary>
    /// The size of this segment.
    /// Set by the parent <see cref="Segmented"/> from <see cref="Segmented.Size"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = SegmentedSize.Medium)]
    private SegmentedSize size;

    private bool hasAutoToolTip;

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

    /// <summary>
    /// Gets the key used in <see cref="Segmented.SelectedKeys"/>: <see cref="Key"/>, or the text of the content
    /// (a string, the text of a <see cref="TextBlock"/>, or <c>Content.ToString()</c>).
    /// </summary>
    internal string? EffectiveKey
        => Key ?? DisplayText;

    /// <summary>
    /// Gets the readable text of the segment: the content text, or the <see cref="Key"/>.
    /// </summary>
    internal string? DisplayText
        => Content switch
        {
            string text => text,
            TextBlock textBlock => textBlock.Text,
            null => Key,
            _ => Content.ToString(),
        };

    /// <summary>
    /// Gets or sets the <see cref="Segmented"/> this segment belongs to.
    /// </summary>
    internal Segmented? Owner { get; set; }

    /// <summary>
    /// Does what a mouse click does, when the segment is enabled.
    /// </summary>
    internal void PerformClick()
    {
        if (!IsEnabled)
        {
            return;
        }

        Click?.Invoke(this, new RoutedEventArgs());
    }

    /// <summary>
    /// Applies the icon-only presentation: the text becomes the tooltip unless one was set by the user.
    /// </summary>
    internal void ApplyIconOnly(bool iconOnly)
    {
        SetCurrentValue(IsIconOnlyProperty, iconOnly);

        if (iconOnly)
        {
            if (ToolTip is null || hasAutoToolTip)
            {
                ToolTip = DisplayText;
                hasAutoToolTip = true;
            }
        }
        else if (hasAutoToolTip)
        {
            ClearValue(ToolTipProperty);
            hasAutoToolTip = false;
        }
    }

    /// <inheritdoc />
    protected override void OnContentChanged(
        object oldContent,
        object newContent)
    {
        base.OnContentChanged(oldContent, newContent);

        if (hasAutoToolTip)
        {
            ToolTip = DisplayText;
        }

        if (Key is null)
        {
            Owner?.OnItemKeyChanged();
        }
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new SegmentedItemAutomationPeer(this);

    /// <inheritdoc />
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);
        PerformClick();
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

        if (e.Key is System.Windows.Input.Key.Space or System.Windows.Input.Key.Enter)
        {
            PerformClick();
            e.Handled = true;
        }
    }

    private static void OnIsSelectedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var item = (SegmentedItem)d;
        if (UIElementAutomationPeer.FromElement(item) is SegmentedItemAutomationPeer peer)
        {
            peer.RaiseSelectionChanged((bool)e.OldValue, (bool)e.NewValue);
        }
    }

    private static void OnKeyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((SegmentedItem)d).Owner?.OnItemKeyChanged();
}