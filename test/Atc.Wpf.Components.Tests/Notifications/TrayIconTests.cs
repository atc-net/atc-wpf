namespace Atc.Wpf.Components.Tests.Notifications;

public sealed class TrayIconTests : IDisposable
{
    private readonly List<FakeTrayIconHost> hosts = [];

    public void Dispose()
        => Dispatcher.CurrentDispatcher.InvokeShutdown();

    [StaFact]
    public void Initialized_AddsTheIconWithTheToolTipAndImage()
    {
        var image = new DrawingImage();
        using var trayIcon = CreateTrayIcon(t =>
        {
            t.ToolTipText = "My application";
            t.IconSource = image;
        });

        var host = Assert.Single(hosts);
        Assert.Equal(["Add:My application"], host.Calls);
        Assert.Same(image, host.LastIcon);
        Assert.True(trayIcon.IsCreated);
    }

    [StaFact]
    public void Initialized_WhenTheShellRejectsTheIcon_IsNotCreated()
    {
        using var trayIcon = new TrayIcon(() => new FakeTrayIconHost { AddResult = false });
        trayIcon.BeginInit();
        trayIcon.EndInit();

        Assert.False(trayIcon.IsCreated);
    }

    [StaFact]
    public void ToolTipTextChanged_UpdatesTheIcon()
    {
        using var trayIcon = CreateTrayIcon(t => t.ToolTipText = "Idle");

        trayIcon.ToolTipText = "Busy";

        Assert.Equal(["Add:Idle", "Update:Busy"], hosts[0].Calls);
    }

    [StaFact]
    public void IconSourceChanged_UpdatesTheIcon()
    {
        using var trayIcon = CreateTrayIcon();
        var image = new DrawingImage();

        trayIcon.IconSource = image;

        Assert.Equal("Update:", hosts[0].Calls[^1]);
        Assert.Same(image, hosts[0].LastIcon);
    }

    [StaFact]
    public void PropertyChangedBeforeInitialization_DoesNotAddTheIcon()
    {
        using var trayIcon = new TrayIcon(CreateHost)
        {
            ToolTipText = "Not yet",
        };

        Assert.Empty(hosts);
        Assert.False(trayIcon.IsCreated);
    }

    [StaFact]
    public void CreatedInCode_AddsTheIconWhenTheDispatcherRunsAndFollowsLaterChanges()
    {
        using var trayIcon = new TrayIcon(CreateHost)
        {
            ToolTipText = "Idle",
        };

        Assert.Empty(hosts);

        Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.Background);

        Assert.Equal(["Add:Idle"], hosts[0].Calls);
        Assert.True(trayIcon.IsCreated);

        trayIcon.ToolTipText = "Busy";

        Assert.Equal("Update:Busy", hosts[0].Calls[^1]);

        trayIcon.Visibility = Visibility.Collapsed;

        Assert.True(hosts[0].IsDisposed);
        Assert.False(trayIcon.IsCreated);
    }

    [StaFact]
    public void Collapsed_RemovesTheIconAndVisibleAddsItAgain()
    {
        using var trayIcon = CreateTrayIcon(t => t.ToolTipText = "Tip");

        trayIcon.Visibility = Visibility.Collapsed;

        Assert.True(hosts[0].IsDisposed);
        Assert.False(trayIcon.IsCreated);

        trayIcon.Visibility = Visibility.Visible;

        Assert.Equal(2, hosts.Count);
        Assert.Equal(["Add:Tip"], hosts[1].Calls);
        Assert.True(trayIcon.IsCreated);
    }

    [StaFact]
    public void Dispose_RemovesTheIconAndIgnoresLaterChanges()
    {
        using var trayIcon = CreateTrayIcon();

        trayIcon.Dispose();
        trayIcon.ToolTipText = "After dispose";

        Assert.True(hosts[0].IsDisposed);
        Assert.Single(hosts);
        Assert.False(trayIcon.IsCreated);
    }

    [StaFact]
    public void DispatcherShutdown_RemovesTheIcon()
    {
        using var trayIcon = CreateTrayIcon();

        Dispatcher.CurrentDispatcher.InvokeShutdown();

        Assert.True(hosts[0].IsDisposed);
    }

    [StaFact]
    public void Unloaded_RemovesTheIconAndLoadedAddsItAgain()
    {
        using var trayIcon = CreateTrayIcon();

        trayIcon.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));

        Assert.True(hosts[0].IsDisposed);
        Assert.False(trayIcon.IsCreated);

        trayIcon.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

        Assert.Equal(2, hosts.Count);
        Assert.True(trayIcon.IsCreated);
    }

    [StaFact]
    public void TaskbarCreated_AddsTheIconAgain()
    {
        using var trayIcon = CreateTrayIcon(t => t.ToolTipText = "Tip");

        hosts[0].RaiseTaskbarCreated();

        Assert.Equal(["Add:Tip", "Add:Tip"], hosts[0].Calls);
    }

    [StaFact]
    public void ClickAction_RaisesClickAndExecutesTheClickCommand()
    {
        var clicks = 0;
        object? commandParameter = null;
        using var trayIcon = CreateTrayIcon(t =>
        {
            t.ClickCommand = new RelayCommand<object?>(p => commandParameter = p);
            t.CommandParameter = "parameter";
        });
        trayIcon.Click += (_, _) => clicks++;

        hosts[0].RaiseAction(TrayIconAction.Click);

        Assert.Equal(1, clicks);
        Assert.Equal("parameter", commandParameter);
    }

    [StaFact]
    public void DoubleClickAction_RaisesDoubleClickAndExecutesTheDoubleClickCommand()
    {
        var doubleClicks = 0;
        var executed = false;
        using var trayIcon = CreateTrayIcon(t => t.DoubleClickCommand = new RelayCommand(() => executed = true));
        trayIcon.DoubleClick += (_, _) => doubleClicks++;

        hosts[0].RaiseAction(TrayIconAction.DoubleClick);

        Assert.Equal(1, doubleClicks);
        Assert.True(executed);
    }

    [StaFact]
    public void ClickAction_SkipsACommandThatCannotExecute()
    {
        var executed = false;
        using var trayIcon = CreateTrayIcon(t => t.ClickCommand = new RelayCommand(() => executed = true, () => false));

        hosts[0].RaiseAction(TrayIconAction.Click);

        Assert.False(executed);
    }

    [StaFact]
    public void ContextMenuAction_OpensTheContextMenuAtTheMousePoint()
    {
        var menu = new ContextMenu();
        menu.Items.Add(new MenuItem { Header = "Exit" });
        using var trayIcon = CreateTrayIcon(t => t.ContextMenu = menu);

        hosts[0].RaiseAction(TrayIconAction.ContextMenu);

        Assert.True(menu.IsOpen);
        Assert.Equal(PlacementMode.MousePoint, menu.Placement);
        Assert.Same(trayIcon, menu.PlacementTarget);
        menu.IsOpen = false;
    }

    [StaFact]
    public void ContextMenuAction_TheContextMenuInheritsTheDataContext()
    {
        var menu = new ContextMenu();
        using var trayIcon = CreateTrayIcon(t =>
        {
            t.ContextMenu = menu;
            t.DataContext = "view model";
        });

        hosts[0].RaiseAction(TrayIconAction.ContextMenu);

        Assert.Equal("view model", menu.DataContext);
        menu.IsOpen = false;
    }

    [StaFact]
    public void ContextMenuAction_WithoutAContextMenu_DoesNothing()
    {
        using var trayIcon = CreateTrayIcon();

        hosts[0].RaiseAction(TrayIconAction.ContextMenu);

        Assert.Equal(["Add:"], hosts[0].Calls);
    }

    [StaFact]
    public void ShowNotification_ShowsADesktopToastOfTheGivenType()
    {
        var service = new FakeToastNotificationService();
        using var trayIcon = CreateTrayIcon(t => t.NotificationService = service);
        Action onClick = () => { };

        trayIcon.ShowNotification("Title", "Message", ToastNotificationType.Warning, TimeSpan.FromSeconds(3), onClick);

        var call = Assert.Single(service.Calls);
        Assert.Equal(("Warning", "Title", "Message", TimeSpan.FromSeconds(3), true), (call.Type, call.Title, call.Message, call.ExpirationTime, call.UseDesktop));
        Assert.Same(onClick, call.OnClick);
    }

    [StaTheory]
    [InlineData(ToastNotificationType.Information, "Information")]
    [InlineData(ToastNotificationType.Success, "Success")]
    [InlineData(ToastNotificationType.Warning, "Warning")]
    [InlineData(ToastNotificationType.Error, "Error")]
    public void ShowNotification_MapsTheType(
        ToastNotificationType type,
        string expected)
    {
        var service = new FakeToastNotificationService();
        using var trayIcon = CreateTrayIcon(t => t.NotificationService = service);

        trayIcon.ShowNotification("Title", "Message", type);

        Assert.Equal(expected, Assert.Single(service.Calls).Type);
    }

    private TrayIcon CreateTrayIcon(Action<TrayIcon>? configure = null)
    {
        var trayIcon = new TrayIcon(CreateHost);
        trayIcon.BeginInit();
        configure?.Invoke(trayIcon);
        trayIcon.EndInit();
        return trayIcon;
    }

    private FakeTrayIconHost CreateHost()
    {
        var host = new FakeTrayIconHost();
        hosts.Add(host);
        return host;
    }

    private sealed class FakeTrayIconHost : ITrayIconHost
    {
        public List<string> Calls { get; } = [];

        public ImageSource? LastIcon { get; private set; }

        public bool AddResult { get; init; } = true;

        public bool IsDisposed { get; private set; }

        public Action<TrayIconAction>? ActionReceived { get; set; }

        public Action? TaskbarCreated { get; set; }

        public bool Add(
            ImageSource? icon,
            string toolTip)
        {
            Calls.Add($"Add:{toolTip}");
            LastIcon = icon;
            return AddResult;
        }

        public void Update(
            ImageSource? icon,
            string toolTip)
        {
            Calls.Add($"Update:{toolTip}");
            LastIcon = icon;
        }

        public void SetForegroundWindow(nint handle)
            => Calls.Add("SetForegroundWindow");

        public void RaiseAction(TrayIconAction action)
            => ActionReceived?.Invoke(action);

        public void RaiseTaskbarCreated()
            => TaskbarCreated?.Invoke();

        public void Dispose()
            => IsDisposed = true;
    }

    private sealed class FakeToastNotificationService : IToastNotificationService
    {
        public List<(string Type, string Title, string Message, TimeSpan? ExpirationTime, bool UseDesktop, Action? OnClick)> Calls { get; } = [];

        public void ShowInformation(
            string title,
            string message,
            string areaName = "",
            TimeSpan? expirationTime = null,
            bool useDesktop = false,
            Action? onClick = null,
            Action? onClose = null)
            => Calls.Add(("Information", title, message, expirationTime, useDesktop, onClick));

        public void ShowSuccess(
            string title,
            string message,
            string areaName = "",
            TimeSpan? expirationTime = null,
            bool useDesktop = false,
            Action? onClick = null,
            Action? onClose = null)
            => Calls.Add(("Success", title, message, expirationTime, useDesktop, onClick));

        public void ShowWarning(
            string title,
            string message,
            string areaName = "",
            TimeSpan? expirationTime = null,
            bool useDesktop = false,
            Action? onClick = null,
            Action? onClose = null)
            => Calls.Add(("Warning", title, message, expirationTime, useDesktop, onClick));

        public void ShowError(
            string title,
            string message,
            string areaName = "",
            TimeSpan? expirationTime = null,
            bool useDesktop = false,
            Action? onClick = null,
            Action? onClose = null)
            => Calls.Add(("Error", title, message, expirationTime, useDesktop, onClick));
    }
}