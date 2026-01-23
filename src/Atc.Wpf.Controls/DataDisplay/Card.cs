namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// A Card control for grouping related content with elevation/shadow, optional header/footer,
/// and expand/collapse functionality.
/// </summary>
[ContentProperty(nameof(Content))]
public sealed partial class Card : HeaderedContentControl
{
    /// <summary>
    /// Shadow depth level (0-5, where 0 is no shadow).
    /// </summary>
    [DependencyProperty(DefaultValue = 2)]
    private int elevation;

    /// <summary>
    /// Header background brush.
    /// </summary>
    [DependencyProperty]
    private Brush? headerBackground;

    /// <summary>
    /// Header foreground brush.
    /// </summary>
    [DependencyProperty]
    private Brush? headerForeground;

    /// <summary>
    /// Header inner padding.
    /// </summary>
    [DependencyProperty(DefaultValue = "new Thickness(12)")]
    private Thickness headerPadding;

    /// <summary>
    /// Whether to show the header area.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showHeader;

    /// <summary>
    /// Footer content.
    /// </summary>
    [DependencyProperty]
    private object? footer;

    /// <summary>
    /// Footer template.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? footerTemplate;

    /// <summary>
    /// Footer background brush.
    /// </summary>
    [DependencyProperty]
    private Brush? footerBackground;

    /// <summary>
    /// Footer inner padding.
    /// </summary>
    [DependencyProperty(DefaultValue = "new Thickness(12)")]
    private Thickness footerPadding;

    /// <summary>
    /// Whether to show the footer area (auto-hides if Footer is null).
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showFooter;

    /// <summary>
    /// Whether the card supports expand/collapse functionality.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isExpandable;

    /// <summary>
    /// Whether the card is currently expanded.
    /// </summary>
    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnIsExpandedChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private bool isExpanded;

    /// <summary>
    /// Position of the toggle button (Left or Right).
    /// </summary>
    [DependencyProperty(DefaultValue = default(ExpanderButtonLocation))]
    private ExpanderButtonLocation expanderButtonLocation;

    /// <summary>
    /// Corner radius for the card.
    /// </summary>
    [DependencyProperty(DefaultValue = "new CornerRadius(4)")]
    private CornerRadius cornerRadius;

    /// <summary>
    /// Content area padding.
    /// </summary>
    [DependencyProperty(DefaultValue = "new Thickness(12)")]
    private Thickness contentPadding;

    /// <summary>
    /// Occurs when the card is expanded.
    /// </summary>
    public event RoutedEventHandler? Expanded;

    /// <summary>
    /// Occurs when the card is collapsed.
    /// </summary>
    public event RoutedEventHandler? Collapsed;

    static Card()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Card),
            new FrameworkPropertyMetadata(typeof(Card)));
    }

    private void OnExpanded()
        => Expanded?.Invoke(this, new RoutedEventArgs());

    private void OnCollapsed()
        => Collapsed?.Invoke(this, new RoutedEventArgs());

    private static void OnIsExpandedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not Card card ||
            e.NewValue is not bool isExpanded)
        {
            return;
        }

        if (isExpanded)
        {
            card.OnExpanded();
        }
        else
        {
            card.OnCollapsed();
        }
    }
}