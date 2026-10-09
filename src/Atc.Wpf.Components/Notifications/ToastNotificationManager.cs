namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// Shows toast notifications in the registered <see cref="ToastNotificationArea"/> controls or in a desktop overlay window.
/// </summary>
public sealed class ToastNotificationManager : IToastNotificationManager
{
    private readonly Dispatcher dispatcher;
    private static readonly ConcurrentBag<ToastNotificationArea> Areas = [];
    private static ToastNotificationsOverlayWindow? window;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToastNotificationManager"/> class.
    /// </summary>
    /// <param name="dispatcher">The dispatcher used to show notifications, or <see langword="null"/> to use the application dispatcher.</param>
    public ToastNotificationManager(Dispatcher? dispatcher = null)
    {
        dispatcher ??= Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        this.dispatcher = dispatcher;
    }

    /// <summary>
    /// Shows a toast notification built from the given <see cref="ToastNotificationMessage"/>.
    /// </summary>
    /// <param name="useDesktop"><see langword="true"/> to show the notification in a desktop overlay window; otherwise it is shown in the matching notification area.</param>
    /// <param name="message">The message whose type, title and text are displayed.</param>
    /// <param name="areaName">The name of the <see cref="ToastNotificationArea"/> to show the notification in; when no area matches, all non-desktop areas are used.</param>
    /// <param name="expirationTime">The time before the notification closes automatically; defaults to 5 seconds.</param>
    /// <param name="onClick">An optional action invoked when the notification is clicked.</param>
    /// <param name="onClose">An optional action invoked when the notification is closed.</param>
    public void Show(
        bool useDesktop,
        ToastNotificationMessage message,
        string areaName = "",
        TimeSpan? expirationTime = null,
        Action? onClick = null,
        Action? onClose = null)
        => Show(
            useDesktop,
            message.ToToastNotificationContent(),
            areaName,
            expirationTime,
            onClick,
            onClose);

    /// <summary>
    /// Shows a toast notification with the given <see cref="ToastNotificationContent"/>.
    /// </summary>
    /// <param name="useDesktop"><see langword="true"/> to show the notification in a desktop overlay window; otherwise it is shown in the matching notification area.</param>
    /// <param name="content">The content to display.</param>
    /// <param name="areaName">The name of the <see cref="ToastNotificationArea"/> to show the notification in; when no area matches, all non-desktop areas are used.</param>
    /// <param name="expirationTime">The time before the notification closes automatically; defaults to 5 seconds.</param>
    /// <param name="onClick">An optional action invoked when the notification is clicked.</param>
    /// <param name="onClose">An optional action invoked when the notification is closed.</param>
    public void Show(
        bool useDesktop,
        ToastNotificationContent content,
        string areaName = "",
        TimeSpan? expirationTime = null,
        Action? onClick = null,
        Action? onClose = null)
        => Show(
            useDesktop,
            (UserControl)content,
            areaName,
            expirationTime,
            onClick,
            onClose);

    /// <inheritdoc />
    [SuppressMessage("Critical Code Smell", "S2696:Instance members should not write to \"static\" fields", Justification = "OK.")]
    public void Show(
        bool useDesktop,
        UserControl content,
        string areaName = "",
        TimeSpan? expirationTime = null,
        Action? onClick = null,
        Action? onClose = null)
    {
        expirationTime ??= TimeSpan.FromSeconds(5);

        if (!dispatcher.CheckAccess())
        {
            _ = dispatcher.BeginInvoke(
                new Action(() => Show(
                    useDesktop,
                    content,
                    areaName,
                    expirationTime,
                    onClick,
                    onClose)));
            return;
        }

        if (useDesktop)
        {
            if (window is null)
            {
                var workArea = SystemParameters.WorkArea;
                window = new ToastNotificationsOverlayWindow
                {
                    Left = workArea.Left,
                    Top = workArea.Top,
                    Width = workArea.Width,
                    Height = workArea.Height,
                };
            }

            // The desktop overlay has no owner, so it follows the main window's direction.
            window.FlowDirection = Application.Current?.MainWindow?.FlowDirection ?? FlowDirection.LeftToRight;
            window.Show();
            areaName = "DesktopArea";
        }

        var toastNotificationAreas = Areas
            .Where(a => a.Name == areaName)
            .ToArray();

        if (!toastNotificationAreas.Any())
        {
            toastNotificationAreas = Areas
                .Where(a => a.Name != "DesktopArea")
                .ToArray();
        }

        foreach (var area in toastNotificationAreas)
        {
            area.Show(
                content,
                (TimeSpan)expirationTime,
                onClick,
                onClose);
        }
    }

    internal static void AddArea(ToastNotificationArea area)
        => Areas.Add(area);
}