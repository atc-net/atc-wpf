namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// A control for displaying user profile pictures with fallback options for initials.
/// </summary>
/// <remarks>
/// Features:
/// <list type="bullet">
///   <item>Image source support</item>
///   <item>Initials fallback when no image</item>
///   <item>Background color generated from name hash</item>
///   <item>Size presets (ExtraSmall, Small, Medium, Large, ExtraLarge)</item>
///   <item>Status indicator (Online, Offline, Busy, Away, DoNotDisturb)</item>
/// </list>
/// </remarks>
public sealed partial class Avatar : Control
{
    /// <summary>
    /// Gets or sets the image source for the avatar.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnDisplayPropertiesChanged))]
    private ImageSource? imageSource;

    /// <summary>
    /// Gets or sets the initials to display when no image is provided.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnDisplayPropertiesChanged))]
    private string? initials;

    /// <summary>
    /// Gets or sets the display name used for auto-generating initials and background color.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnDisplayPropertiesChanged))]
    private string? displayName;

    /// <summary>
    /// Gets or sets the size preset for the avatar.
    /// </summary>
    [DependencyProperty(DefaultValue = AvatarSize.Medium, PropertyChangedCallback = nameof(OnSizeChanged))]
    private AvatarSize size;

    /// <summary>
    /// Gets or sets the status indicator for the avatar.
    /// </summary>
    [DependencyProperty(DefaultValue = AvatarStatus.None)]
    private AvatarStatus status;

    /// <summary>
    /// Gets or sets the corner radius. When not set, the avatar is circular.
    /// </summary>
    [DependencyProperty]
    private CornerRadius? cornerRadius;

    /// <summary>
    /// Gets or sets the border brush for the status indicator.
    /// </summary>
    [DependencyProperty]
    private Brush? statusBorderBrush;

    /// <summary>
    /// Gets or sets the border thickness for the status indicator.
    /// </summary>
    [DependencyProperty(DefaultValue = 2d)]
    private double statusBorderThickness;

    /// <summary>
    /// Gets the computed initials to display.
    /// </summary>
    [DependencyProperty]
    private string? computedInitials;

    /// <summary>
    /// Gets the computed background brush (auto-generated from name if not explicitly set).
    /// </summary>
    [DependencyProperty]
    private Brush? computedBackground;

    /// <summary>
    /// Gets the computed size in pixels.
    /// </summary>
    [DependencyProperty(DefaultValue = 40d)]
    private double computedSize;

    /// <summary>
    /// Gets the computed font size for initials.
    /// </summary>
    [DependencyProperty(DefaultValue = 14d)]
    private double computedFontSize;

    /// <summary>
    /// Gets the computed status indicator size.
    /// </summary>
    [DependencyProperty(DefaultValue = 12d)]
    private double computedStatusSize;

    /// <summary>
    /// Gets the computed corner radius (circular by default).
    /// </summary>
    [DependencyProperty]
    private CornerRadius computedCornerRadius;

    /// <summary>
    /// Gets whether the image should be shown.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool showImage;

    static Avatar()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Avatar),
            new FrameworkPropertyMetadata(typeof(Avatar)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Avatar"/> class.
    /// </summary>
    public Avatar()
    {
        Loaded += OnLoaded;
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        UpdateDisplayProperties();
        UpdateSizeProperties();
    }

    private static void OnDisplayPropertiesChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Avatar avatar)
        {
            avatar.UpdateDisplayProperties();
        }
    }

    private static void OnSizeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Avatar avatar)
        {
            avatar.UpdateSizeProperties();
        }
    }

    private void UpdateDisplayProperties()
    {
        // Determine if we should show image or initials
        ShowImage = ImageSource is not null;

        // Compute initials
        if (!string.IsNullOrWhiteSpace(Initials))
        {
            ComputedInitials = Initials.Length > 2
                ? Initials[..2].ToUpperInvariant()
                : Initials.ToUpperInvariant();
        }
        else if (!string.IsNullOrWhiteSpace(DisplayName))
        {
            ComputedInitials = GenerateInitialsFromName(DisplayName);
        }
        else
        {
            ComputedInitials = "?";
        }

        // Compute background color
        if (Background is not null)
        {
            ComputedBackground = Background;
        }
        else if (!string.IsNullOrWhiteSpace(DisplayName))
        {
            ComputedBackground = new SolidColorBrush(GenerateColorFromName(DisplayName));
        }
        else if (!string.IsNullOrWhiteSpace(Initials))
        {
            ComputedBackground = new SolidColorBrush(GenerateColorFromName(Initials));
        }
        else
        {
            ComputedBackground = new SolidColorBrush(Colors.Gray);
        }
    }

    private void UpdateSizeProperties()
    {
        var (pixels, fontSize, statusSize) = GetSizeValues(Size);
        ComputedSize = pixels;
        ComputedFontSize = fontSize;
        ComputedStatusSize = statusSize;

        // Update corner radius (circular if not explicitly set)
        ComputedCornerRadius = CornerRadius ?? new CornerRadius(pixels / 2);
    }

    private static (double Pixels, double FontSize, double StatusSize) GetSizeValues(
        AvatarSize size)
        => size switch
        {
            AvatarSize.ExtraSmall => (24, 10, 8),
            AvatarSize.Small => (32, 12, 10),
            AvatarSize.Medium => (40, 14, 12),
            AvatarSize.Large => (56, 18, 14),
            AvatarSize.ExtraLarge => (80, 24, 18),
            _ => (40, 14, 12),
        };

    /// <summary>
    /// Generates initials from a display name.
    /// </summary>
    /// <param name="name">The display name.</param>
    /// <returns>One or two character initials.</returns>
    internal static string GenerateInitialsFromName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "?";
        }

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
        {
            return "?";
        }

        if (parts.Length == 1)
        {
            return parts[0].Length >= 2
                ? parts[0][..2].ToUpperInvariant()
                : parts[0].ToUpperInvariant();
        }

        // Use first and last name initials
        return $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant();
    }

    /// <summary>
    /// Generates a deterministic color from a name using HSV color space.
    /// </summary>
    /// <param name="name">The name to generate a color for.</param>
    /// <returns>A color derived from the name's hash.</returns>
    internal static Color GenerateColorFromName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Colors.Gray;
        }

        // Use a stable hash algorithm
        var hash = GetStableHashCode(name);

        // Map hash to hue (0-360)
        var hue = System.Math.Abs(hash % 360);

        // Use fixed saturation and value for pleasant, readable colors
        return ColorHelper.GetColorFromHsv(hue, 0.65, 0.75);
    }

    /// <summary>
    /// Gets a stable hash code that doesn't change between runs.
    /// </summary>
    private static int GetStableHashCode(string str)
    {
        unchecked
        {
            var hash1 = 5381;
            var hash2 = hash1;

            for (var i = 0; i < str.Length && str[i] != '\0'; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1 || str[i + 1] == '\0')
                {
                    break;
                }

                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }
}