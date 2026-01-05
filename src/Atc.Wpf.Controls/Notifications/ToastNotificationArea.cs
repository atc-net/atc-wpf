namespace Atc.Wpf.Controls.Notifications;

public sealed partial class ToastNotificationArea : Control
{
    private readonly Lock itemsLock = new();
    private IList? items;

    [DependencyProperty(DefaultValue = ToastNotificationPosition.BottomRight)]
    private ToastNotificationPosition position;

    [DependencyProperty(DefaultValue = int.MaxValue)]
    private int maxItems;

    public ToastNotificationArea()
    {
        ToastNotificationManager.AddArea(this);
    }

    static ToastNotificationArea()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ToastNotificationArea),
            new FrameworkPropertyMetadata(typeof(ToastNotificationArea)));
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        var itemsControl = GetTemplateChild("PART_Items") as Panel;
        items = itemsControl?.Children;
    }

    [SuppressMessage("AsyncUsage", "AsyncFixer03:Fire-and-forget async-void methods or delegates", Justification = "OK - Need 'async void' and not 'async Task'")]
    [SuppressMessage("Major Bug", "S3168:\"async\" methods should not return \"void\"", Justification = "OK - Need 'async void' and not 'async Task'")]
    [SuppressMessage("ReSharper", "AsyncVoidMethod", Justification = "OK - Need 'async void' and not 'async Task'")]
    public async void Show(
        ContentControl contentControl,
        TimeSpan expirationTime,
        Action? onClick,
        Action? onClose)
    {
        var toastNotification = new ToastNotification
        {
            Content = contentControl,
        };

        toastNotification.MouseLeftButtonDown += (sender, _) =>
        {
            if (onClick == null)
            {
                return;
            }

            onClick.Invoke();
            (sender as ToastNotification)?.Close();
        };

        toastNotification.NotificationClosed += (_, _) => onClose?.Invoke();
        toastNotification.NotificationClosed += OnNotificationClosed;

        if (!IsLoaded)
        {
            return;
        }

        var w = Window.GetWindow(this);
        if (w is null)
        {
            return;
        }

        var x = PresentationSource.FromVisual(w);
        if (x is null)
        {
            return;
        }

        ToastNotification? toastNotificationToClose = null;

        lock (itemsLock)
        {
            items ??= new List<ToastNotification>();
            items.Add(toastNotification);

            var notifications = items.OfType<ToastNotification>().Where(i => !i.IsClosing).Skip(MaxItems).ToList();
            if (notifications.Count > 0)
            {
                toastNotificationToClose = notifications[0];
            }
        }

        toastNotificationToClose?.Close();

        if (expirationTime == TimeSpan.MaxValue)
        {
            return;
        }

        await Task.Delay(expirationTime).ConfigureAwait(true);
        toastNotification.Close();
    }

    private void OnNotificationClosed(
        object sender,
        RoutedEventArgs routedEventArgs)
    {
        var notification = sender as ToastNotification;
        lock (itemsLock)
        {
            items?.Remove(notification);
        }
    }
}