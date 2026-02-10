namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// A control for displaying prominent inline messages with severity levels, dismissibility, and action buttons.
/// </summary>
[ContentProperty(nameof(Content))]
[TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
[TemplatePart(Name = "PART_Border", Type = typeof(Border))]
public sealed partial class Alert : ContentControl
{
    private Border? partBorder;
    private Storyboard? pulseStoryboard;

    /// <summary>
    /// The severity level of the alert, which determines colors and default icon.
    /// </summary>
    [DependencyProperty(DefaultValue = AlertSeverity.Info)]
    private AlertSeverity severity;

    /// <summary>
    /// The visual variant of the alert.
    /// </summary>
    [DependencyProperty(DefaultValue = AlertVariant.Filled)]
    private AlertVariant variant;

    /// <summary>
    /// Whether the alert can be dismissed (shows a close button).
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isDismissible;

    /// <summary>
    /// Whether the alert uses reduced padding.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isDense;

    /// <summary>
    /// Whether the alert plays a subtle breathing/pulse animation to draw attention.
    /// </summary>
    [DependencyProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(OnIsPulsingChanged))]
    private bool isPulsing;

    /// <summary>
    /// The half-cycle duration of the pulse animation (full cycle = 2x with auto-reverse).
    /// </summary>
    [DependencyProperty(
        DefaultValue = "new TimeSpan(0, 0, 0, 1, 500)",
        PropertyChangedCallback = nameof(OnPulseParameterChanged))]
    private TimeSpan pulseDuration;

    /// <summary>
    /// The minimum opacity during the pulse animation (0.0–1.0).
    /// </summary>
    [DependencyProperty(
        DefaultValue = 0.65,
        PropertyChangedCallback = nameof(OnPulseParameterChanged))]
    private double pulseOpacity;

    /// <summary>
    /// Whether the severity icon is shown.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showIcon;

    /// <summary>
    /// Custom icon content to override the default severity icon.
    /// </summary>
    [DependencyProperty]
    private object? icon;

    /// <summary>
    /// Template for the custom icon content.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? iconTemplate;

    /// <summary>
    /// Content for the action button area.
    /// </summary>
    [DependencyProperty]
    private object? actions;

    /// <summary>
    /// Template for the action content.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? actionsTemplate;

    /// <summary>
    /// Corner radius for the alert border.
    /// </summary>
    [DependencyProperty(DefaultValue = "new CornerRadius(4)")]
    private CornerRadius cornerRadius;

    /// <summary>
    /// Override brush for the alert background.
    /// </summary>
    [DependencyProperty]
    private Brush? alertBackground;

    /// <summary>
    /// Override brush for the alert foreground.
    /// </summary>
    [DependencyProperty]
    private Brush? alertForeground;

    /// <summary>
    /// Override brush for the alert border.
    /// </summary>
    [DependencyProperty]
    private Brush? alertBorderBrush;

    /// <summary>
    /// Occurs when the close button is clicked.
    /// </summary>
    public event RoutedEventHandler? CloseClick;

    static Alert()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Alert),
            new FrameworkPropertyMetadata(typeof(Alert)));
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        partBorder = GetTemplateChild("PART_Border") as Border;

        if (GetTemplateChild("PART_CloseButton") is Button closeButton)
        {
            closeButton.Click += OnCloseButtonClick;
        }

        if (IsPulsing)
        {
            StartPulseAnimation();
        }
    }

    private static void OnIsPulsingChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Alert alert)
        {
            if ((bool)e.NewValue)
            {
                alert.StartPulseAnimation();
            }
            else
            {
                alert.StopPulseAnimation();
            }
        }
    }

    private static void OnPulseParameterChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Alert { IsPulsing: true } alert)
        {
            alert.StopPulseAnimation();
            alert.StartPulseAnimation();
        }
    }

    private void StartPulseAnimation()
    {
        if (partBorder is null)
        {
            return;
        }

        var animation = new DoubleAnimation
        {
            From = 1.0,
            To = PulseOpacity,
            Duration = new Duration(PulseDuration),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever,
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut },
        };

        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);

        Storyboard.SetTarget(animation, partBorder);
        Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));

        storyboard.Begin();
        pulseStoryboard = storyboard;
    }

    private void StopPulseAnimation()
    {
        if (pulseStoryboard is not null)
        {
            pulseStoryboard.Stop();
            pulseStoryboard = null;
        }

        if (partBorder is not null)
        {
            partBorder.Opacity = 1.0;
        }
    }

    private void OnCloseButtonClick(
        object sender,
        RoutedEventArgs e)
    {
        e.Handled = true;
        CloseClick?.Invoke(this, new RoutedEventArgs());
    }
}