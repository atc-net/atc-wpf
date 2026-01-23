namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// A control that overlays content with a small badge indicator (count, status dot, etc.).
/// </summary>
[ContentProperty(nameof(Content))]
public sealed partial class Badge : ContentControl
{
    /// <summary>
    /// Content displayed in the badge (e.g., a number or text).
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnBadgeDisplayPropertiesChanged))]
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
    [DependencyProperty(DefaultValue = true, PropertyChangedCallback = nameof(OnBadgeDisplayPropertiesChanged))]
    private bool isBadgeVisible;

    /// <summary>
    /// Gets or sets whether the badge is displayed as a simple dot indicator.
    /// When true, BadgeContent is ignored and only a dot is shown.
    /// </summary>
    [DependencyProperty(DefaultValue = false, PropertyChangedCallback = nameof(OnBadgeDisplayPropertiesChanged))]
    private bool isDot;

    /// <summary>
    /// Gets or sets the maximum value to display. When BadgeContent exceeds this value,
    /// the display shows "Max+" (e.g., "99+" when max is 99).
    /// Set to null or 0 to disable this behavior.
    /// </summary>
    [DependencyProperty(DefaultValue = 0, PropertyChangedCallback = nameof(OnBadgeDisplayPropertiesChanged))]
    private int badgeMaxValue;

    /// <summary>
    /// Gets or sets whether the badge should be hidden when the value is zero.
    /// Only applies when BadgeContent is an integer or can be parsed as one.
    /// </summary>
    [DependencyProperty(DefaultValue = false, PropertyChangedCallback = nameof(OnBadgeDisplayPropertiesChanged))]
    private bool hideWhenZero;

    /// <summary>
    /// Gets the computed content to display in the badge.
    /// This handles IsDot mode and BadgeMaxValue formatting.
    /// </summary>
    [DependencyProperty]
    private object? displayBadgeContent;

    /// <summary>
    /// Gets whether the badge should be visible based on all visibility conditions.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool computedBadgeVisibility;

    static Badge()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Badge),
            new FrameworkPropertyMetadata(typeof(Badge)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Badge"/> class.
    /// </summary>
    public Badge()
    {
        Loaded += OnLoaded;
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        UpdateDisplayProperties();
    }

    private static void OnBadgeDisplayPropertiesChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Badge badge)
        {
            badge.UpdateDisplayProperties();
        }
    }

    private void UpdateDisplayProperties()
    {
        // Handle IsDot mode
        if (IsDot)
        {
            DisplayBadgeContent = null;
            ComputedBadgeVisibility = IsBadgeVisible;
            return;
        }

        // Get numeric value if possible
        var numericValue = GetNumericValue(BadgeContent);

        // Handle HideWhenZero
        if (HideWhenZero && numericValue == 0)
        {
            ComputedBadgeVisibility = false;
            DisplayBadgeContent = BadgeContent;
            return;
        }

        // Handle BadgeMaxValue
        if (BadgeMaxValue > 0 && numericValue > BadgeMaxValue)
        {
            DisplayBadgeContent = $"{BadgeMaxValue}+";
        }
        else
        {
            DisplayBadgeContent = BadgeContent;
        }

        ComputedBadgeVisibility = IsBadgeVisible && HasValidContent(BadgeContent);
    }

    private static bool HasValidContent(object? content)
    {
        if (content is null)
        {
            return false;
        }

        if (content is string stringValue)
        {
            return !string.IsNullOrEmpty(stringValue);
        }

        return true;
    }

    private static int? GetNumericValue(object? content)
    {
        if (content is null)
        {
            return null;
        }

        if (content is int intValue)
        {
            return intValue;
        }

        if (content is long longValue)
        {
            return (int)longValue;
        }

        if (content is double doubleValue)
        {
            return (int)doubleValue;
        }

        if (content is string stringValue && int.TryParse(stringValue, out var parsed))
        {
            return parsed;
        }

        return null;
    }
}