namespace Atc.Wpf.Components.Notifications;

public sealed class ToastNotificationManager : IToastNotificationManager
{
    private readonly Dispatcher dispatcher;
    private static readonly ConcurrentBag<ToastNotificationArea> Areas = [];
    private static ToastNotificationsOverlayWindow? window;

    public ToastNotificationManager(Dispatcher? dispatcher = null)
    {
        dispatcher ??= Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        this.dispatcher = dispatcher;
    }

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