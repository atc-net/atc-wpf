namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// Represents an individual dockable region within a <see cref="DockPanelPro"/>.
/// Supports collapsible behavior, header, and size constraints.
/// </summary>
[ContentProperty(nameof(Content))]
public sealed partial class DockRegion : HeaderedContentControl
{
    /// <summary>
    /// Identifies the Expanded routed event.
    /// </summary>
    public static readonly RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent(
        nameof(Expanded),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(DockRegion));

    /// <summary>
    /// Identifies the Collapsed routed event.
    /// </summary>
    public static readonly RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent(
        nameof(Collapsed),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(DockRegion));

    /// <summary>
    /// Identifies the SizeChanged routed event for layout persistence.
    /// </summary>
    public static readonly RoutedEvent RegionSizeChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(RegionSizeChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(DockRegion));

    /// <summary>
    /// Gets or sets a unique identifier for this region for layout persistence.
    /// </summary>
    [DependencyProperty]
    private string? regionId;

    /// <summary>
    /// Gets or sets a value indicating whether this region can be collapsed.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isCollapsible;

    /// <summary>
    /// Gets or sets a value indicating whether this region is currently expanded.
    /// </summary>
    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnIsExpandedChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private bool isExpanded;

    /// <summary>
    /// Gets or sets a value indicating whether this region can be resized via splitters.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isResizable;

    /// <summary>
    /// Gets or sets the header background brush.
    /// </summary>
    [DependencyProperty]
    private Brush? headerBackground;

    /// <summary>
    /// Gets or sets the header foreground brush.
    /// </summary>
    [DependencyProperty]
    private Brush? headerForeground;

    /// <summary>
    /// Gets or sets the header padding.
    /// </summary>
    [DependencyProperty(DefaultValue = "new Thickness(8)")]
    private Thickness headerPadding;

    /// <summary>
    /// Gets or sets the corner radius for the region.
    /// </summary>
    [DependencyProperty]
    private CornerRadius cornerRadius;

    /// <summary>
    /// Gets or sets the content padding.
    /// </summary>
    [DependencyProperty(DefaultValue = "new Thickness(0)")]
    private Thickness contentPadding;

    /// <summary>
    /// Occurs when the region is expanded.
    /// </summary>
    public event RoutedEventHandler Expanded
    {
        add => AddHandler(ExpandedEvent, value);
        remove => RemoveHandler(ExpandedEvent, value);
    }

    /// <summary>
    /// Occurs when the region is collapsed.
    /// </summary>
    public event RoutedEventHandler Collapsed
    {
        add => AddHandler(CollapsedEvent, value);
        remove => RemoveHandler(CollapsedEvent, value);
    }

    /// <summary>
    /// Occurs when the region's size changes (for layout persistence).
    /// </summary>
    public event RoutedEventHandler RegionSizeChanged
    {
        add => AddHandler(RegionSizeChangedEvent, value);
        remove => RemoveHandler(RegionSizeChangedEvent, value);
    }

    static DockRegion()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(DockRegion),
            new FrameworkPropertyMetadata(typeof(DockRegion)));
    }

    /// <summary>
    /// Toggles the expanded state of this region.
    /// </summary>
    public void ToggleExpanded()
    {
        if (IsCollapsible)
        {
            IsExpanded = !IsExpanded;
        }
    }

    /// <summary>
    /// Raises the <see cref="RegionSizeChanged"/> event.
    /// </summary>
    internal void RaiseRegionSizeChanged()
    {
        RaiseEvent(new RoutedEventArgs(RegionSizeChangedEvent, this));
    }

    private void OnExpanded()
    {
        RaiseEvent(new RoutedEventArgs(ExpandedEvent, this));
    }

    private void OnCollapsed()
    {
        RaiseEvent(new RoutedEventArgs(CollapsedEvent, this));
    }

    private static void OnIsExpandedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not DockRegion region ||
            e.NewValue is not bool isExpanded)
        {
            return;
        }

        if (isExpanded)
        {
            region.OnExpanded();
        }
        else
        {
            region.OnCollapsed();
        }
    }
}
