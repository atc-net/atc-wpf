namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// A control for displaying prominent inline messages with severity levels, dismissibility, and action buttons.
/// </summary>
[ContentProperty(nameof(Content))]
[TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
public sealed partial class Alert : ContentControl
{
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

        if (GetTemplateChild("PART_CloseButton") is Button closeButton)
        {
            closeButton.Click += OnCloseButtonClick;
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