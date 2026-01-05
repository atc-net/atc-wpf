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

    /// <summary>
    /// Shows a toast notification. Fire-and-forget wrapper for <see cref="ShowAsync"/>.
    /// </summary>
    /// <param name="contentControl">The content to display.</param>
    /// <param name="expirationTime">Time before auto-close. Use TimeSpan.MaxValue for no auto-close.</param>
    /// <param name="onClick">Action to invoke when clicked.</param>
    /// <param name="onClose">Action to invoke when closed.</param>
    public void Show(
        ContentControl contentControl,
        TimeSpan expirationTime,
        Action? onClick,
        Action? onClose)
        => _ = ShowAsync(contentControl, expirationTime, onClick, onClose);

    /// <summary>
    /// Shows a toast notification asynchronously.
    /// </summary>
    /// <param name="contentControl">The content to display.</param>
    /// <param name="expirationTime">Time before auto-close. Use TimeSpan.MaxValue for no auto-close.</param>
    /// <param name="onClick">Action to invoke when clicked.</param>
    /// <param name="onClose">Action to invoke when closed.</param>
    /// <returns>A task that completes when the notification expires or is closed.</returns>
    public async Task ShowAsync(
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

        if (toastNotificationToClose is not null)
        {
            await toastNotificationToClose.CloseAsync().ConfigureAwait(true);
        }

        if (expirationTime == TimeSpan.MaxValue)
        {
            return;
        }

        await Task.Delay(expirationTime).ConfigureAwait(true);
        await toastNotification.CloseAsync().ConfigureAwait(true);
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