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

    /// <summary>
    /// Gets or sets a value indicating whether clicking the notification closes it.
    /// A click that a button inside the notification has already handled does not close it.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
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

    /// <inheritdoc />
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        if (CloseOnClick && !e.Handled)
        {
            Close();
        }
    }

    /// <summary>
    /// Completes when the notification has been shown for <paramref name="expirationTime"/>, or when it starts closing.
    /// With <paramref name="pauseOnHover"/>, the time does not run while the mouse is over the notification.
    /// </summary>
    /// <param name="expirationTime">How long the notification is shown.</param>
    /// <param name="pauseOnHover">Whether the time stops running while the mouse is over the notification.</param>
    /// <returns>A task that completes when the notification has expired.</returns>
    internal Task WaitForExpiryAsync(
        TimeSpan expirationTime,
        bool pauseOnHover)
        => new ToastNotificationExpiry(this, expirationTime, pauseOnHover).Task;

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