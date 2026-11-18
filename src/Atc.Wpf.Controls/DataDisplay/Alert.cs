namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// A control for displaying prominent inline messages with severity levels, dismissibility, and action buttons.
/// </summary>
[ContentProperty(nameof(Content))]
[TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
[DefaultStyleKey]
public sealed partial class Alert : ContentControl
{
    [TemplatePartField("PART_Border")]
    private Border? partBorder;

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
        OverrideDefaultStyleKey();
        StyleCheck.Register(typeof(Alert));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Alert"/> class.
    /// </summary>
    public Alert()
    {
        // The pulse repeats forever, so it runs only while the alert is shown.
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        StopPulseAnimation();
        ResolveTemplateParts();

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

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        if (IsPulsing)
        {
            StartPulseAnimation();
        }
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
        => StopPulseAnimation();

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

        // Animated on the border itself so it can be removed again: a storyboard started with Begin() and no
        // containing element cannot be stopped.
        partBorder.BeginAnimation(OpacityProperty, animation);
    }

    private void StopPulseAnimation()
    {
        if (partBorder is null)
        {
            return;
        }

        partBorder.BeginAnimation(OpacityProperty, animation: null);
        partBorder.Opacity = 1.0;
    }

    private void OnCloseButtonClick(
        object sender,
        RoutedEventArgs e)
    {
        e.Handled = true;
        CloseClick?.Invoke(this, new RoutedEventArgs());
    }
}