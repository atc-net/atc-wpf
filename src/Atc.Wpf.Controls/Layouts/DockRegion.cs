namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// Represents an individual dockable region within a <see cref="DockPanelPro"/>.
/// Supports collapsible behavior, header, and size constraints.
/// </summary>
[ContentProperty(nameof(Content))]
[DefaultStyleKey]
public sealed partial class DockRegion : HeaderedContentControl
{
    /// <summary>
    /// Occurs when the region is expanded.
    /// </summary>
    [RoutedEvent(RoutingStrategy.Bubble, GenerateRaiseMethod = true)]
    private static readonly RoutedEvent expanded;

    /// <summary>
    /// Occurs when the region is collapsed.
    /// </summary>
    [RoutedEvent(RoutingStrategy.Bubble, GenerateRaiseMethod = true)]
    private static readonly RoutedEvent collapsed;

    /// <summary>
    /// Occurs when the region's size changes (for layout persistence).
    /// </summary>
    [RoutedEvent(RoutingStrategy.Bubble)]
    private static readonly RoutedEvent regionSizeChanged;

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

    static DockRegion()
    {
        OverrideDefaultStyleKey();
        StyleCheck.Register(typeof(DockRegion));

        // A side region whose panes are all floating or closed collapses, so it takes no space.
        VisibilityProperty.OverrideMetadata(
            typeof(DockRegion),
            new FrameworkPropertyMetadata(Visibility.Visible, propertyChangedCallback: null, CoerceVisibility));
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
        RaiseExpanded();
    }

    private void OnCollapsed()
    {
        RaiseCollapsed();
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

        if (AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged) &&
            UIElementAutomationPeer.FromElement(region) is DockRegionAutomationPeer peer)
        {
            peer.RaiseExpandCollapseStateChanged(!isExpanded, isExpanded);
        }
    }
}