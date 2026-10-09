namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// A single toast notification shown in a <see cref="ToastNotificationArea"/>, which closes with an animation.
/// </summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
[TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
public sealed partial class ToastNotification : ContentControl
{
    private TimeSpan closingAnimationTime = TimeSpan.Zero;

    /// <summary>
    /// Occurs when closing of the notification starts, before the closing animation runs.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedEventHandler))]
    private static readonly RoutedEvent notificationCloseInvoked;

    /// <summary>
    /// Occurs when the notification has closed, after the closing animation has finished.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedEventHandler))]
    private static readonly RoutedEvent notificationClosed;

    [DependencyProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(CloseOnClickChanged))]
    private bool closeOnClick;

    /// <summary>
    /// Gets or sets a value indicating whether the notification is closing.
    /// </summary>
    public bool IsClosing { get; set; }

    static ToastNotification()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ToastNotification),
            new FrameworkPropertyMetadata(typeof(ToastNotification)));
    }

    /// <inheritdoc />
    [SuppressMessage("", "SA1118:The parameter spans multiple lines", Justification = "OK")]
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("PART_CloseButton") is Button closeButton)
        {
            closeButton.Click += OnCloseButtonOnClick;
        }

        var storyboards = Template.Triggers
            .OfType<EventTrigger>()
            .FirstOrDefault(x => x.RoutedEvent == NotificationCloseInvokedEvent)?
            .Actions
            .OfType<BeginStoryboard>()
            .Select(a => a.Storyboard);

        closingAnimationTime = new TimeSpan(
            storyboards?.Max(x =>
                System.Math.Min(
                    (x.Duration.HasTimeSpan
                        ? x.Duration.TimeSpan + (x.BeginTime ?? TimeSpan.Zero)
                        : TimeSpan.MaxValue).Ticks,
                    x.Children
                        .Select(ch => ch.Duration.TimeSpan + (x.BeginTime ?? TimeSpan.Zero))
                        .Max()
                        .Ticks)) ?? 0);
    }

    /// <summary>
    /// Closes the notification with animation. Fire-and-forget wrapper for <see cref="CloseAsync"/>.
    /// </summary>
    public void Close()
        => _ = CloseAsync();

    /// <summary>
    /// Closes the notification with animation asynchronously.
    /// </summary>
    /// <returns>A task that completes when the notification is fully closed.</returns>
    public async Task CloseAsync()
    {
        if (IsClosing)
        {
            return;
        }

        IsClosing = true;

        RaiseEvent(new RoutedEventArgs(NotificationCloseInvokedEvent));
        await Task.Delay(closingAnimationTime).ConfigureAwait(true);
        RaiseEvent(new RoutedEventArgs(NotificationClosedEvent));
    }

    private static void CloseOnClickChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not Button button)
        {
            return;
        }

        var value = (bool)e.NewValue;

        if (value)
        {
            button.Click += (_, _) =>
            {
                var notification = VisualTreeHelperEx.GetParent<ToastNotification>(button);
                notification?.Close();
            };
        }
    }

    private void OnCloseButtonOnClick(
        object sender,
        RoutedEventArgs args)
    {
        if (sender is not Button button)
        {
            return;
        }

        button.Click -= OnCloseButtonOnClick;
        Close();
    }
}