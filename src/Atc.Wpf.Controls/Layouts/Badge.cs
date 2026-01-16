namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// A control that overlays content with a small badge indicator (count, status dot, etc.).
/// </summary>
[ContentProperty(nameof(Content))]
public sealed partial class Badge : ContentControl
{
    /// <summary>
    /// Content displayed in the badge (e.g., a number or text).
    /// </summary>
    [DependencyProperty]
    private object? badgeContent;

    /// <summary>
    /// Custom template for badge content.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? badgeContentTemplate;

    /// <summary>
    /// Background brush for the badge.
    /// </summary>
    [DependencyProperty]
    private Brush? badgeBackground;

    /// <summary>
    /// Foreground brush for the badge text.
    /// </summary>
    [DependencyProperty]
    private Brush? badgeForeground;

    /// <summary>
    /// Border brush for the badge.
    /// </summary>
    [DependencyProperty]
    private Brush? badgeBorderBrush;

    /// <summary>
    /// Border thickness for the badge.
    /// </summary>
    [DependencyProperty]
    private Thickness badgeBorderThickness;

    /// <summary>
    /// Corner radius for the badge (for rounded corners).
    /// </summary>
    [DependencyProperty(DefaultValue = "new CornerRadius(8)")]
    private CornerRadius badgeCornerRadius;

    /// <summary>
    /// Position of the badge relative to the content.
    /// </summary>
    [DependencyProperty(DefaultValue = BadgePlacementMode.TopRight)]
    private BadgePlacementMode badgePlacementMode;

    /// <summary>
    /// Additional margin for fine-tuning badge position.
    /// </summary>
    [DependencyProperty]
    private Thickness badgeMargin;

    /// <summary>
    /// Font size for the badge content.
    /// </summary>
    [DependencyProperty(DefaultValue = 10d)]
    private double badgeFontSize;

    /// <summary>
    /// Minimum width for the badge.
    /// </summary>
    [DependencyProperty(DefaultValue = 16d)]
    private double badgeMinWidth;

    /// <summary>
    /// Minimum height for the badge.
    /// </summary>
    [DependencyProperty(DefaultValue = 16d)]
    private double badgeMinHeight;

    /// <summary>
    /// Inner padding for the badge content.
    /// </summary>
    [DependencyProperty(DefaultValue = "new Thickness(4, 2, 4, 2)")]
    private Thickness badgePadding;

    /// <summary>
    /// Gets or sets whether the badge is visible.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isBadgeVisible;

    static Badge()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Badge),
            new FrameworkPropertyMetadata(typeof(Badge)));
    }
}