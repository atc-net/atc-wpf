// ReSharper disable AsyncVoidMethod
namespace Atc.Wpf.Controls.Notifications;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
[TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
public sealed partial class ToastNotification : ContentControl
{
    private TimeSpan closingAnimationTime = TimeSpan.Zero;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedEventHandler))]
    private static readonly RoutedEvent notificationCloseInvoked;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedEventHandler))]
    private static readonly RoutedEvent notificationClosed;

    [DependencyProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(CloseOnClickChanged))]
    private bool closeOnClick;

    public bool IsClosing { get; set; }

    static ToastNotification()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ToastNotification),
            new FrameworkPropertyMetadata(typeof(ToastNotification)));
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("PART_CloseButton") is Button closeButton)
        {
            closeButton.Click += OnCloseButtonOnClick;
        }

        var storyboards = Template.Triggers.OfType<System.Windows.EventTrigger>()
            .FirstOrDefault(x => x.RoutedEvent == NotificationCloseInvokedEvent)?.Actions.OfType<BeginStoryboard>()
            .Select(a => a.Storyboard);

        closingAnimationTime = new TimeSpan(
            storyboards?.Max(x =>
                System.Math.Min(
                    (x.Duration.HasTimeSpan
                        ? x.Duration.TimeSpan + (x.BeginTime ?? TimeSpan.Zero)
                        : TimeSpan.MaxValue).Ticks,
                    x.Children.Select(ch => ch.Duration.TimeSpan + (x.BeginTime ?? TimeSpan.Zero)).Max().Ticks)) ?? 0);
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