namespace Atc.Wpf.Controls.Notifications;

public sealed partial class ToastNotificationArea : Control
{
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
        items ??= new List<ToastNotification>();

        lock (items)
        {
            items.Add(toastNotification);

            if (items.OfType<ToastNotification>().Where(i => !i.IsClosing).Skip(MaxItems).Any())
            {
                toastNotificationToClose = items.OfType<ToastNotification>().First(i => !i.IsClosing);
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
        items?.Remove(notification);
    }
}