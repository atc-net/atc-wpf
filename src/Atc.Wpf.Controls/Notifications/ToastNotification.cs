namespace Atc.Wpf.Controls.Notifications;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
[TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
public class ToastNotification : ContentControl
{
    private TimeSpan closingAnimationTime = TimeSpan.Zero;

    public static readonly RoutedEvent NotificationCloseInvokedEvent = EventManager.RegisterRoutedEvent(
        "NotificationCloseInvoked",
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(ToastNotification));

    public static readonly RoutedEvent NotificationClosedEvent = EventManager.RegisterRoutedEvent(
        "NotificationClosed",
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(ToastNotification));

    public static readonly DependencyProperty CloseOnClickProperty = DependencyProperty.RegisterAttached(
        "CloseOnClick",
        typeof(bool),
        typeof(ToastNotification),
        new FrameworkPropertyMetadata(
            defaultValue: false,
            CloseOnClickChanged));

    public bool IsClosing { get; set; }

    public event RoutedEventHandler NotificationCloseInvoked
    {
        add => AddHandler(NotificationCloseInvokedEvent, value);
        remove => RemoveHandler(NotificationCloseInvokedEvent, value);
    }

    public event RoutedEventHandler NotificationClosed
    {
        add => AddHandler(NotificationClosedEvent, value);
        remove => RemoveHandler(NotificationClosedEvent, value);
    }

    static ToastNotification()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ToastNotification),
            new FrameworkPropertyMetadata(typeof(ToastNotification)));
    }

    public static bool GetCloseOnClick(
        DependencyObject obj)
        => (bool)obj.GetValue(CloseOnClickProperty);

    public static void SetCloseOnClick(
        DependencyObject obj,
        bool value)
        => obj.SetValue(CloseOnClickProperty, value);

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("PART_CloseButton") is Button closeButton)
        {
            closeButton.Click += OnCloseButtonOnClick;
        }

        var storyboards = Template.Triggers.OfType<EventTrigger>()
            .FirstOrDefault(x => x.RoutedEvent == NotificationCloseInvokedEvent)?.Actions.OfType<BeginStoryboard>()
            .Select(a => a.Storyboard);

        closingAnimationTime = new TimeSpan(
            storyboards?.Max(x => System.Math.Min((x.Duration.HasTimeSpan ? x.Duration.TimeSpan + (x.BeginTime ?? TimeSpan.Zero) : TimeSpan.MaxValue).Ticks, x.Children.Select(ch => ch.Duration.TimeSpan + (x.BeginTime ?? TimeSpan.Zero)).Max().Ticks)) ?? 0);
    }

    [SuppressMessage("AsyncUsage", "AsyncFixer03:Fire-and-forget async-void methods or delegates", Justification = "OK - Need 'async void' and not 'async Task'")]
    [SuppressMessage("Major Bug", "S3168:\"async\" methods should not return \"void\"", Justification = "OK - Need 'async void' and not 'async Task'")]
    public async void Close()
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