namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// An icon in the Windows notification area (system tray), with a tooltip, click commands,
/// a context menu and desktop toast notifications.
/// </summary>
/// <remarks>
/// The icon is shown while <see cref="UIElement.Visibility"/> is <see cref="Visibility.Visible"/>.
/// When the element is in the visual tree, it follows the tree: unloading removes the icon and loading adds it again.
/// The icon is removed on <see cref="Dispose()"/> and when the dispatcher shuts down.
/// </remarks>
public partial class TrayIcon : FrameworkElement, IDisposable
{
    private static readonly DependencyPropertyKey IsCreatedPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsCreated),
        typeof(bool),
        typeof(TrayIcon),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Identifies the <see cref="IsCreated"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsCreatedProperty = IsCreatedPropertyKey.DependencyProperty;

    private readonly Func<ITrayIconHost> hostFactory;
    private ITrayIconHost? host;
    private Dispatcher? shutdownDispatcher;
    private bool isActive;
    private bool isUnloaded;
    private bool isDisposed;

    /// <summary>
    /// The image shown in the notification area; the application icon when <see langword="null"/>.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnIconPropertyChanged))]
    private ImageSource? iconSource;

    /// <summary>
    /// The tooltip shown when the mouse hovers over the icon; the shell shows at most 127 characters.
    /// </summary>
    [DependencyProperty(DefaultValue = "", PropertyChangedCallback = nameof(OnIconPropertyChanged))]
    private string toolTipText;

    /// <summary>
    /// The command executed when the icon is clicked or selected with the keyboard.
    /// </summary>
    [DependencyProperty]
    private ICommand? clickCommand;

    /// <summary>
    /// The command executed when the icon is double-clicked.
    /// </summary>
    [DependencyProperty]
    private ICommand? doubleClickCommand;

    /// <summary>
    /// The parameter passed to <see cref="ClickCommand"/> and <see cref="DoubleClickCommand"/>.
    /// </summary>
    [DependencyProperty]
    private object? commandParameter;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrayIcon"/> class.
    /// </summary>
    public TrayIcon()
        : this(static () => new ShellTrayIconHost())
    {
    }

    internal TrayIcon(Func<ITrayIconHost> hostFactory)
    {
        this.hostFactory = hostFactory;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        // Created in code and never added to a tree, the element is never initialized; add the icon once
        // the caller has set the properties.
        _ = Dispatcher.BeginInvoke(DispatcherPriority.Loaded, UpdateIcon);
    }

    /// <summary>
    /// Occurs when the icon is clicked or selected with the keyboard.
    /// </summary>
    /// <remarks>
    /// The first click of a double-click raises this event too.
    /// </remarks>
    public event EventHandler? Click;

    /// <summary>
    /// Occurs when the icon is double-clicked.
    /// </summary>
    public event EventHandler? DoubleClick;

    /// <summary>
    /// Gets a value indicating whether the icon is currently shown in the notification area.
    /// </summary>
    public bool IsCreated
    {
        get => (bool)GetValue(IsCreatedProperty);
        private set => SetValue(IsCreatedPropertyKey, BooleanBoxes.Box(value));
    }

    /// <summary>
    /// Gets or sets the service used by <see cref="ShowNotification"/>; a <see cref="ToastNotificationService"/>
    /// when not set.
    /// </summary>
    public IToastNotificationService? NotificationService { get; set; }

    /// <summary>
    /// Shows a desktop toast notification, typically to report something while the application is in the tray.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <param name="type">The notification type.</param>
    /// <param name="expirationTime">How long the notification is shown; the service default when <see langword="null"/>.</param>
    /// <param name="onClick">Called when the user clicks the notification, for example to restore the main window.</param>
    public void ShowNotification(
        string title,
        string message,
        ToastNotificationType type = ToastNotificationType.Information,
        TimeSpan? expirationTime = null,
        Action? onClick = null)
    {
        var service = NotificationService ??= new ToastNotificationService(Dispatcher);
        switch (type)
        {
            case ToastNotificationType.Success:
                service.ShowSuccess(title, message, expirationTime: expirationTime, useDesktop: true, onClick: onClick);
                break;
            case ToastNotificationType.Warning:
                service.ShowWarning(title, message, expirationTime: expirationTime, useDesktop: true, onClick: onClick);
                break;
            case ToastNotificationType.Error:
                service.ShowError(title, message, expirationTime: expirationTime, useDesktop: true, onClick: onClick);
                break;
            default:
                service.ShowInformation(title, message, expirationTime: expirationTime, useDesktop: true, onClick: onClick);
                break;
        }
    }

    /// <summary>
    /// Removes the icon from the notification area.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Removes the icon from the notification area.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> when called from <see cref="Dispose()"/>.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (isDisposed)
        {
            return;
        }

        isDisposed = true;
        if (disposing)
        {
            ReleaseHost();
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
        }
    }

    /// <inheritdoc />
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        UpdateIcon();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == VisibilityProperty && isActive)
        {
            UpdateIcon();
        }
    }

    private static void OnIconPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var trayIcon = (TrayIcon)d;
        if (trayIcon.isActive)
        {
            trayIcon.UpdateIcon();
        }
    }

    private void UpdateIcon()
    {
        if (isDisposed || DesignerProperties.GetIsInDesignMode(this))
        {
            return;
        }

        // From here on, property changes update the icon right away.
        isActive = true;

        if (isUnloaded || Visibility != Visibility.Visible)
        {
            ReleaseHost();
            return;
        }

        if (IsCreated && host is not null)
        {
            host.Update(IconSource, ToolTipText);
            return;
        }

        host ??= CreateHost();
        IsCreated = host.Add(IconSource, ToolTipText);
    }

    private ITrayIconHost CreateHost()
    {
        var newHost = hostFactory();
        newHost.ActionReceived = OnActionReceived;
        newHost.TaskbarCreated = OnTaskbarCreated;

        shutdownDispatcher = Dispatcher;
        shutdownDispatcher.ShutdownStarted += OnDispatcherShutdownStarted;
        return newHost;
    }

    private void ReleaseHost()
    {
        if (shutdownDispatcher is not null)
        {
            shutdownDispatcher.ShutdownStarted -= OnDispatcherShutdownStarted;
            shutdownDispatcher = null;
        }

        if (host is not null)
        {
            host.ActionReceived = null;
            host.TaskbarCreated = null;
            host.Dispose();
            host = null;
        }

        IsCreated = false;
    }

    private void OnActionReceived(TrayIconAction action)
    {
        switch (action)
        {
            case TrayIconAction.Click:
                Click?.Invoke(this, EventArgs.Empty);
                ExecuteCommand(ClickCommand);
                break;
            case TrayIconAction.DoubleClick:
                DoubleClick?.Invoke(this, EventArgs.Empty);
                ExecuteCommand(DoubleClickCommand);
                break;
            case TrayIconAction.ContextMenu:
                OpenContextMenu();
                break;
        }
    }

    private void ExecuteCommand(ICommand? command)
    {
        if (command?.CanExecute(CommandParameter) == true)
        {
            command.Execute(CommandParameter);
        }
    }

    private void OpenContextMenu()
    {
        var menu = ContextMenu;
        if (menu is null)
        {
            return;
        }

        // The menu is not opened through ContextMenuService, so it does not pick up the DataContext by itself.
        if (menu.ReadLocalValue(DataContextProperty) == DependencyProperty.UnsetValue)
        {
            menu.SetBinding(DataContextProperty, new Binding(nameof(DataContext)) { Source = this });
        }

        menu.PlacementTarget = this;
        menu.Placement = PlacementMode.MousePoint;
        menu.Opened -= OnContextMenuOpened;
        menu.Opened += OnContextMenuOpened;
        menu.IsOpen = true;
    }

    private void OnContextMenuOpened(
        object sender,
        RoutedEventArgs e)
    {
        // Without a foreground window, the menu does not close when the user clicks outside it.
        if (PresentationSource.FromVisual((Visual)sender) is HwndSource source)
        {
            host?.SetForegroundWindow(source.Handle);
        }
    }

    private void OnTaskbarCreated()
    {
        IsCreated = false;
        UpdateIcon();
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        isUnloaded = false;
        UpdateIcon();
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        isUnloaded = true;
        UpdateIcon();
    }

    private void OnDispatcherShutdownStarted(
        object? sender,
        EventArgs e)
        => Dispose();
}