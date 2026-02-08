namespace Atc.Wpf.Controls.Progressing;

/// <summary>
/// A general-purpose overlay control that displays a translucent dimming layer
/// over its content with optional centered overlay content.
/// Useful for modals, loading states, and interaction blocking.
/// </summary>
[TemplatePart(Name = PartOverlayPanel, Type = typeof(Grid))]
[TemplatePart(Name = PartDimmingRect, Type = typeof(Rectangle))]
[TemplatePart(Name = PartOverlayContentPresenter, Type = typeof(ContentPresenter))]
[ContentProperty(nameof(Content))]
public partial class Overlay : ContentControl
{
    private const string PartOverlayPanel = "PART_OverlayPanel";
    private const string PartDimmingRect = "PART_DimmingRect";
    private const string PartOverlayContentPresenter = "PART_OverlayContentPresenter";

    private Grid? overlayPanel;
    private Rectangle? dimmingRect;

    /// <summary>
    /// Identifies the Activated routed event.
    /// </summary>
    public static readonly RoutedEvent ActivatedEvent = EventManager.RegisterRoutedEvent(
        nameof(Activated),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(Overlay));

    /// <summary>
    /// Identifies the Deactivated routed event.
    /// </summary>
    public static readonly RoutedEvent DeactivatedEvent = EventManager.RegisterRoutedEvent(
        nameof(Deactivated),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(Overlay));

    /// <summary>
    /// Gets or sets whether the overlay is currently active (visible).
    /// </summary>
    [DependencyProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(OnIsActiveChanged))]
    private bool isActive;

    /// <summary>
    /// Gets or sets the brush used for the dimming layer background.
    /// </summary>
    [DependencyProperty]
    private Brush? overlayBrush;

    /// <summary>
    /// Gets or sets the opacity of the dimming rectangle.
    /// </summary>
    [DependencyProperty(DefaultValue = 0.5)]
    private double overlayOpacity;

    /// <summary>
    /// Gets or sets the content displayed centered on the overlay.
    /// </summary>
    [DependencyProperty]
    private object? overlayContent;

    /// <summary>
    /// Gets or sets the data template for the overlay content.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? overlayContentTemplate;

    /// <summary>
    /// Gets or sets whether clicking the dimming area closes the overlay.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool autoClose;

    /// <summary>
    /// Gets or sets the duration of the fade-in animation.
    /// </summary>
    [DependencyProperty(DefaultValue = "new TimeSpan(0, 0, 0, 0, 200)")]
    private TimeSpan fadeInDuration;

    /// <summary>
    /// Gets or sets the duration of the fade-out animation.
    /// </summary>
    [DependencyProperty(DefaultValue = "new TimeSpan(0, 0, 0, 0, 150)")]
    private TimeSpan fadeOutDuration;

    /// <summary>
    /// Occurs when the overlay becomes active.
    /// </summary>
    public event RoutedEventHandler Activated
    {
        add => AddHandler(ActivatedEvent, value);
        remove => RemoveHandler(ActivatedEvent, value);
    }

    /// <summary>
    /// Occurs when the overlay becomes inactive.
    /// </summary>
    public event RoutedEventHandler Deactivated
    {
        add => AddHandler(DeactivatedEvent, value);
        remove => RemoveHandler(DeactivatedEvent, value);
    }

    static Overlay()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Overlay),
            new FrameworkPropertyMetadata(typeof(Overlay)));
    }

    public override void OnApplyTemplate()
    {
        if (dimmingRect is not null)
        {
            dimmingRect.MouseLeftButtonDown -= OnDimmingRectMouseLeftButtonDown;
        }

        base.OnApplyTemplate();

        overlayPanel = GetTemplateChild(PartOverlayPanel) as Grid;
        dimmingRect = GetTemplateChild(PartDimmingRect) as Rectangle;

        if (dimmingRect is not null)
        {
            dimmingRect.MouseLeftButtonDown += OnDimmingRectMouseLeftButtonDown;
        }

        if (overlayPanel is not null)
        {
            overlayPanel.Visibility = IsActive ? Visibility.Visible : Visibility.Collapsed;
            overlayPanel.Opacity = IsActive ? 1.0 : 0.0;
        }
    }

    private static void OnIsActiveChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Overlay overlay)
        {
            overlay.OnIsActiveChanged((bool)e.NewValue);
        }
    }

    private void OnIsActiveChanged(bool newValue)
    {
        if (overlayPanel is null)
        {
            return;
        }

        if (newValue)
        {
            overlayPanel.Visibility = Visibility.Visible;

            var fadeIn = new DoubleAnimation(0.0, 1.0, new Duration(FadeInDuration))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            };

            overlayPanel.BeginAnimation(OpacityProperty, fadeIn);
            RaiseEvent(new RoutedEventArgs(ActivatedEvent, this));
        }
        else
        {
            var fadeOut = new DoubleAnimation(1.0, 0.0, new Duration(FadeOutDuration))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn },
            };

            fadeOut.Completed += (_, _) =>
            {
                if (!IsActive && overlayPanel is not null)
                {
                    overlayPanel.Visibility = Visibility.Collapsed;
                }
            };

            overlayPanel.BeginAnimation(OpacityProperty, fadeOut);
            RaiseEvent(new RoutedEventArgs(DeactivatedEvent, this));
        }
    }

    private void OnDimmingRectMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (AutoClose && e.Source == dimmingRect)
        {
            IsActive = false;
            e.Handled = true;
        }
    }
}