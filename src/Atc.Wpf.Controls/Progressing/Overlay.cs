namespace Atc.Wpf.Controls.Progressing;

/// <summary>
/// A general-purpose overlay control that displays a translucent dimming layer
/// over its content with optional centered overlay content.
/// Useful for modals, loading states, and interaction blocking.
/// </summary>
[TemplatePart(Name = PartOverlayContentPresenter, Type = typeof(ContentPresenter))]
[ContentProperty(nameof(Content))]
[DefaultStyleKey]
public partial class Overlay : ContentControl
{
    private const string PartOverlayPanel = "PART_OverlayPanel";
    private const string PartDimmingRect = "PART_DimmingRect";
    private const string PartOverlayContentPresenter = "PART_OverlayContentPresenter";

    [TemplatePartField(PartOverlayPanel)]
    private Grid? overlayPanel;

    [TemplatePartField(PartDimmingRect)]
    private Rectangle? dimmingRect;

    /// <summary>
    /// Occurs when the overlay becomes active.
    /// </summary>
    [RoutedEvent(RoutingStrategy.Bubble)]
    private static readonly RoutedEvent activated;

    /// <summary>
    /// Occurs when the overlay becomes inactive.
    /// </summary>
    [RoutedEvent(RoutingStrategy.Bubble)]
    private static readonly RoutedEvent deactivated;

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

    static Overlay()
    {
        OverrideDefaultStyleKey();
        StyleCheck.Register(typeof(Overlay));
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        if (dimmingRect is not null)
        {
            dimmingRect.MouseLeftButtonDown -= OnDimmingRectMouseLeftButtonDown;
        }

        base.OnApplyTemplate();

        ResolveTemplateParts();

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

    [SuppressMessage("Usage", "MA0134:Observe result of async calls", Justification = "Fire-and-forget animation")]
    private void OnIsActiveChanged(bool newValue)
    {
        if (overlayPanel is null)
        {
            return;
        }

        if (newValue)
        {
            var parameters = new AnimationParameters { Duration = FadeInDuration };
            _ = overlayPanel.FadeInAsync(parameters);
            RaiseEvent(new RoutedEventArgs(ActivatedEvent, this));
        }
        else
        {
            var parameters = new AnimationParameters
            {
                Duration = FadeOutDuration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn },
            };

            _ = overlayPanel.FadeOutAsync(parameters);
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