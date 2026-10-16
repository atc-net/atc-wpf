namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// Shows toast notifications in the registered <see cref="ToastNotificationArea"/> controls or in a desktop overlay window.
/// </summary>
/// <remarks>
/// An area is registered while it is loaded (see <see cref="ToastNotificationArea"/>), and the registry holds it weakly,
/// so a view that hosted an area does not stay alive because of the manager.
/// </remarks>
public sealed class ToastNotificationManager : IToastNotificationManager
{
    private const string DesktopAreaName = "DesktopArea";
    private static readonly Lock AreasLock = new();
    private static readonly List<WeakReference<ToastNotificationArea>> Areas = [];
    private static ToastNotificationsOverlayWindow? window;
    private readonly Dispatcher dispatcher;

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

    /// <summary>
    /// Shows a toast notification with the given content in the first loaded area that matches.
    /// </summary>
    /// <remarks>
    /// A WPF element can only have one parent, so when several loaded areas match the name only the first one shows
    /// <paramref name="content"/>; the others are skipped. Use the overload that takes a content factory to show a
    /// notification in every matching area.
    /// </remarks>
    /// <param name="useDesktop"><see langword="true"/> to show the notification in a desktop overlay window; otherwise it is shown in the matching notification area.</param>
    /// <param name="content">The content to display.</param>
    /// <param name="areaName">The name of the <see cref="ToastNotificationArea"/> to show the notification in; when no area matches, all non-desktop areas are used.</param>
    /// <param name="expirationTime">The time before the notification closes automatically; defaults to 5 seconds.</param>
    /// <param name="onClick">An optional action invoked when the notification is clicked.</param>
    /// <param name="onClose">An optional action invoked when the notification is closed.</param>
    public void Show(
        bool useDesktop,
        UserControl content,
        string areaName = "",
        TimeSpan? expirationTime = null,
        Action? onClick = null,
        Action? onClose = null)
    {
        var handedOut = false;
        ShowCore(
            useDesktop,
            () =>
            {
                if (handedOut)
                {
                    return null;
                }

                handedOut = true;
                return content;
            },
            areaName,
            expirationTime,
            onClick,
            onClose);
    }

    /// <summary>
    /// Shows a toast notification in every loaded area that matches, creating the content once per area.
    /// </summary>
    /// <param name="useDesktop"><see langword="true"/> to show the notification in a desktop overlay window; otherwise it is shown in the matching notification areas.</param>
    /// <param name="contentFactory">Creates the content to display; it is called once for each area that shows the notification.</param>
    /// <param name="areaName">The name of the <see cref="ToastNotificationArea"/> to show the notification in; when no area matches, all non-desktop areas are used.</param>
    /// <param name="expirationTime">The time before the notification closes automatically; defaults to 5 seconds.</param>
    /// <param name="onClick">An optional action invoked when a notification is clicked.</param>
    /// <param name="onClose">An optional action invoked when a notification is closed.</param>
    public void Show(
        bool useDesktop,
        Func<UserControl> contentFactory,
        string areaName = "",
        TimeSpan? expirationTime = null,
        Action? onClick = null,
        Action? onClose = null)
    {
        ArgumentNullException.ThrowIfNull(contentFactory);

        ShowCore(
            useDesktop,
            contentFactory,
            areaName,
            expirationTime,
            onClick,
            onClose);
    }

    /// <summary>
    /// Registers an area. Called by <see cref="ToastNotificationArea"/> when it is loaded.
    /// </summary>
    internal static void AddArea(ToastNotificationArea area)
    {
        lock (AreasLock)
        {
            if (GetLiveAreasUnlocked().Contains(area))
            {
                return;
            }

            Areas.Add(new WeakReference<ToastNotificationArea>(area));
        }
    }

    /// <summary>
    /// Removes an area. Called by <see cref="ToastNotificationArea"/> when it is unloaded.
    /// </summary>
    internal static void RemoveArea(ToastNotificationArea area)
    {
        lock (AreasLock)
        {
            Areas.RemoveAll(reference =>
                !reference.TryGetTarget(out var target) ||
                ReferenceEquals(target, area));
        }
    }

    /// <summary>
    /// Whether the area is currently registered.
    /// </summary>
    internal static bool IsAreaRegistered(ToastNotificationArea area)
        => GetLiveAreas().Contains(area);

    private static List<ToastNotificationArea> GetLiveAreas()
    {
        lock (AreasLock)
        {
            return GetLiveAreasUnlocked();
        }
    }

    /// <summary>
    /// The areas that are still alive; collected ones are pruned from the registry on the way.
    /// </summary>
    private static List<ToastNotificationArea> GetLiveAreasUnlocked()
    {
        var live = new List<ToastNotificationArea>(Areas.Count);
        for (var i = Areas.Count - 1; i >= 0; i--)
        {
            if (Areas[i].TryGetTarget(out var area))
            {
                live.Insert(0, area);
            }
            else
            {
                Areas.RemoveAt(i);
            }
        }

        return live;
    }

    /// <summary>
    /// The loaded areas on the calling thread that match <paramref name="areaName"/>, or all loaded non-desktop areas when none matches.
    /// </summary>
    private static ToastNotificationArea[] GetLoadedAreas(string areaName)
    {
        var loaded = GetLiveAreas()
            .Where(a => a.Dispatcher.CheckAccess() && a.IsLoaded)
            .ToArray();

        var matching = loaded
            .Where(a => string.Equals(a.Name, areaName, StringComparison.Ordinal))
            .ToArray();

        return matching.Length > 0
            ? matching
            : loaded
                .Where(a => !string.Equals(a.Name, DesktopAreaName, StringComparison.Ordinal))
                .ToArray();
    }

    private static ToastNotificationsOverlayWindow EnsureOverlayWindow()
    {
        if (window is null)
        {
            window = new ToastNotificationsOverlayWindow();
            window.Closed += OnOverlayWindowClosed;

            // Belt and braces: an overlay that could not be owned must not outlive the application.
            if (Application.Current is { } application)
            {
                application.Exit += OnApplicationExit;
            }
        }

        // The work area changes when displays or the taskbar change, so it is read on every show.
        var workArea = SystemParameters.WorkArea;
        window.Left = workArea.Left;
        window.Top = workArea.Top;
        window.Width = workArea.Width;
        window.Height = workArea.Height;

        var mainWindow = Application.Current?.MainWindow;
        if (window.Owner is null &&
            mainWindow is not null &&
            !ReferenceEquals(mainWindow, window) &&
            HasWindowHandle(mainWindow))
        {
            // Owned by the main window the overlay closes with it, so it no longer keeps the process alive
            // under the default ShutdownMode. A window without a handle (not shown yet, or closed) cannot own another one.
            window.Owner = mainWindow;
        }

        window.FlowDirection = mainWindow?.FlowDirection ?? FlowDirection.LeftToRight;
        window.Show();
        return window;
    }

    private static bool HasWindowHandle(Window candidate)
        => new WindowInteropHelper(candidate).Handle != IntPtr.Zero &&
           PresentationSource.FromVisual(candidate) is not null;

    private static void OnOverlayWindowClosed(
        object? sender,
        EventArgs e)
    {
        if (sender is not ToastNotificationsOverlayWindow closedWindow)
        {
            return;
        }

        closedWindow.Closed -= OnOverlayWindowClosed;
        if (Application.Current is { } application)
        {
            application.Exit -= OnApplicationExit;
        }

        if (ReferenceEquals(window, closedWindow))
        {
            window = null;
        }
    }

    private static void OnApplicationExit(
        object sender,
        ExitEventArgs e)
        => window?.Close();

    private void ShowCore(
        bool useDesktop,
        Func<UserControl?> contentFactory,
        string areaName,
        TimeSpan? expirationTime,
        Action? onClick,
        Action? onClose)
    {
        expirationTime ??= TimeSpan.FromSeconds(5);

        if (!dispatcher.CheckAccess())
        {
            _ = dispatcher.BeginInvoke(
                new Action(() => ShowCore(
                    useDesktop,
                    contentFactory,
                    areaName,
                    expirationTime,
                    onClick,
                    onClose)));
            return;
        }

        if (useDesktop)
        {
            var overlay = EnsureOverlayWindow();
            if (!overlay.IsLoaded)
            {
                // The overlay was just shown; its area registers itself when Loaded is raised, so show the notification then.
                RoutedEventHandler? onLoaded = null;
                onLoaded = (_, _) =>
                {
                    overlay.Loaded -= onLoaded;
                    ShowCore(
                        useDesktop,
                        contentFactory,
                        areaName,
                        expirationTime,
                        onClick,
                        onClose);
                };

                overlay.Loaded += onLoaded;
                return;
            }

            areaName = DesktopAreaName;
        }

        foreach (var area in GetLoadedAreas(areaName))
        {
            var content = contentFactory();
            if (content is null)
            {
                Debug.WriteLine(
                    $"{nameof(ToastNotificationManager)}: the same content instance cannot be shown in more than one area; " +
                    $"area '{area.Name}' is skipped. Use the Show overload that takes a content factory to show a notification in every area.");
                break;
            }

            area.Show(
                content,
                expirationTime.Value,
                onClick,
                onClose);
        }
    }
}