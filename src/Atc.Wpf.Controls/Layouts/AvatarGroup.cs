namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// A control for displaying a group of overlapping avatars with overflow indication.
/// </summary>
/// <remarks>
/// Features:
/// <list type="bullet">
///   <item>Overlapping avatar display</item>
///   <item>Configurable spacing (negative for overlap)</item>
///   <item>Maximum visible count with "+N" overflow indicator</item>
///   <item>Unified size for all child avatars</item>
/// </list>
/// </remarks>
[ContentProperty(nameof(Children))]
public sealed partial class AvatarGroup : Control
{
    private StackPanel? itemsHost;

    /// <summary>
    /// Gets or sets the spacing between avatars. Use negative values for overlapping.
    /// </summary>
    [DependencyProperty(DefaultValue = -12d, PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private double spacing;

    /// <summary>
    /// Gets or sets the maximum number of avatars to display before showing overflow.
    /// </summary>
    [DependencyProperty(DefaultValue = 5, PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private int maxVisible;

    /// <summary>
    /// Gets or sets the size applied to all child avatars.
    /// </summary>
    [DependencyProperty(DefaultValue = AvatarSize.Medium, PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private AvatarSize size;

    /// <summary>
    /// Gets or sets the background color for the overflow indicator.
    /// </summary>
    [DependencyProperty]
    private Brush? overflowBackground;

    /// <summary>
    /// Gets or sets the foreground color for the overflow indicator text.
    /// </summary>
    [DependencyProperty]
    private Brush? overflowForeground;

    /// <summary>
    /// Gets the collection of child avatars.
    /// </summary>
    public ObservableCollection<Avatar> Children { get; } = [];

    static AvatarGroup()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(AvatarGroup),
            new FrameworkPropertyMetadata(typeof(AvatarGroup)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AvatarGroup"/> class.
    /// </summary>
    public AvatarGroup()
    {
        Children.CollectionChanged += OnChildrenCollectionChanged;
        Loaded += OnLoaded;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        itemsHost = GetTemplateChild("PART_ItemsHost") as StackPanel;
        UpdateLayout();
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        UpdateLayout();
    }

    private void OnChildrenCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        UpdateLayout();
    }

    private static void OnLayoutPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is AvatarGroup group)
        {
            group.UpdateLayout();
        }
    }

    private new void UpdateLayout()
    {
        if (itemsHost is null)
        {
            return;
        }

        itemsHost.Children.Clear();

        var visibleCount = System.Math.Min(Children.Count, MaxVisible);
        var overflowCount = Children.Count - visibleCount;
        var (pixels, fontSize, _) = GetSizeValues(Size);

        for (var i = 0; i < visibleCount; i++)
        {
            var avatar = Children[i];
            avatar.Size = Size;

            // Apply margin for overlap (except first item)
            avatar.Margin = i > 0
                ? new Thickness(Spacing, 0, 0, 0)
                : new Thickness(0);

            // Ensure proper z-order (later items on top)
            Panel.SetZIndex(avatar, i);

            itemsHost.Children.Add(avatar);
        }

        // Add overflow indicator if needed
        if (overflowCount > 0)
        {
            var overflowAvatar = new Avatar
            {
                Size = Size,
                Initials = $"+{overflowCount}",
                Background = OverflowBackground ?? new SolidColorBrush(Colors.Gray),
                Foreground = OverflowForeground ?? Brushes.White,
                Margin = new Thickness(Spacing, 0, 0, 0),
            };

            Panel.SetZIndex(overflowAvatar, visibleCount);
            itemsHost.Children.Add(overflowAvatar);
        }
    }

    private static (double Pixels, double FontSize, double StatusSize) GetSizeValues(AvatarSize size)
        => size switch
        {
            AvatarSize.ExtraSmall => (24, 10, 8),
            AvatarSize.Small => (32, 12, 10),
            AvatarSize.Medium => (40, 14, 12),
            AvatarSize.Large => (56, 18, 14),
            AvatarSize.ExtraLarge => (80, 24, 18),
            _ => (40, 14, 12),
        };
}
