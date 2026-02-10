namespace Atc.Wpf.Components.Flyouts;

/// <summary>
/// A content presenter control for flyout content that provides standardized layout
/// with header, content, and footer sections.
/// </summary>
/// <remarks>
/// The FlyoutPresenter provides a consistent layout structure for flyout content:
/// <list type="bullet">
///   <item>Header section - Title and optional close button</item>
///   <item>Content section - Main content area with optional scrolling</item>
///   <item>Footer section - Optional buttons or actions</item>
/// </list>
/// </remarks>
[TemplatePart(Name = PartHeader, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PartContent, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PartFooter, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PartScrollViewer, Type = typeof(ScrollViewer))]
public partial class FlyoutPresenter : ContentControl
{
    private const string PartHeader = "PART_Header";
    private const string PartContent = "PART_Content";
    private const string PartFooter = "PART_Footer";
    private const string PartScrollViewer = "PART_ScrollViewer";

    /// <summary>
    /// Gets or sets the header content.
    /// </summary>
    [DependencyProperty]
    private object? header;

    /// <summary>
    /// Gets or sets the header template.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? headerTemplate;

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
    [DependencyProperty(DefaultValue = "new Thickness(16, 12, 16, 12)")]
    private Thickness headerPadding;

    /// <summary>
    /// Gets or sets the footer content.
    /// </summary>
    [DependencyProperty]
    private object? footer;

    /// <summary>
    /// Gets or sets the footer template.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? footerTemplate;

    /// <summary>
    /// Gets or sets the footer background brush.
    /// </summary>
    [DependencyProperty]
    private Brush? footerBackground;

    /// <summary>
    /// Gets or sets the footer padding.
    /// </summary>
    [DependencyProperty(DefaultValue = "new Thickness(16, 12, 16, 12)")]
    private Thickness footerPadding;

    /// <summary>
    /// Gets or sets whether the header is visible.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showHeader;

    /// <summary>
    /// Gets or sets whether the footer is visible.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showFooter;

    /// <summary>
    /// Gets or sets whether the content area scrolls.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isContentScrollable;

    /// <summary>
    /// Gets or sets the vertical scroll bar visibility.
    /// </summary>
    [DependencyProperty(DefaultValue = ScrollBarVisibility.Auto)]
    private ScrollBarVisibility verticalScrollBarVisibility;

    /// <summary>
    /// Gets or sets the horizontal scroll bar visibility.
    /// </summary>
    [DependencyProperty(DefaultValue = ScrollBarVisibility.Disabled)]
    private ScrollBarVisibility horizontalScrollBarVisibility;

    /// <summary>
    /// Gets or sets the content padding.
    /// </summary>
    [DependencyProperty(DefaultValue = "new Thickness(16)")]
    private Thickness contentPadding;

    /// <summary>
    /// Gets or sets the corner radius for the presenter.
    /// </summary>
    [DependencyProperty]
    private CornerRadius cornerRadius;

    /// <summary>
    /// Gets or sets whether a separator line is shown between header and content.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showHeaderSeparator;

    /// <summary>
    /// Gets or sets whether a separator line is shown between content and footer.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showFooterSeparator;

    /// <summary>
    /// Gets or sets the separator brush.
    /// </summary>
    [DependencyProperty]
    private Brush? separatorBrush;

    /// <summary>
    /// Gets or sets the separator thickness.
    /// </summary>
    [DependencyProperty(DefaultValue = 1.0)]
    private double separatorThickness;

    static FlyoutPresenter()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(FlyoutPresenter),
            new FrameworkPropertyMetadata(typeof(FlyoutPresenter)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlyoutPresenter"/> class.
    /// </summary>
    public FlyoutPresenter()
    {
    }
}