namespace Atc.Wpf.Components.Tests.Notifications;

/// <summary>
/// Covers the area registry of <see cref="ToastNotificationManager"/> and how content is handed to the areas.
/// Areas live in hidden windows, so they are really loaded; no theme resources are needed.
/// </summary>
public sealed class ToastNotificationManagerTests
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

    [WpfFact]
    public void Area_LoadedThenUnloaded_LeavesTheRegistry()
    {
        var area = new ToastNotificationArea { Name = NewAreaName() };
        Assert.False(ToastNotificationManager.IsAreaRegistered(area));

        area.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
        Assert.True(ToastNotificationManager.IsAreaRegistered(area));

        area.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));
        Assert.False(ToastNotificationManager.IsAreaRegistered(area));
    }

    [WpfFact]
    public async Task Area_InClosedWindow_LeavesTheRegistry()
    {
        var (window, area) = await ShowAreaAsync(NewAreaName());
        Assert.True(ToastNotificationManager.IsAreaRegistered(area));

        var unloaded = new TaskCompletionSource();
        area.Unloaded += (_, _) => unloaded.TrySetResult();
        window.Close();
        await unloaded.Task.WaitAsync(TestTimeout);

        Assert.False(ToastNotificationManager.IsAreaRegistered(area));
    }

    [WpfFact]
    public async Task Show_ContentFactory_EveryLoadedAreaGetsItsOwnInstance()
    {
        var areaName = NewAreaName();
        var (window1, _) = await ShowAreaAsync(areaName);
        var (window2, _) = await ShowAreaAsync(areaName);
        _ = new ToastNotificationArea { Name = areaName }; // never loaded, must be skipped
        var created = new List<UserControl>();
        var manager = new ToastNotificationManager(Dispatcher.CurrentDispatcher);

        try
        {
            manager.Show(
                useDesktop: false,
                () =>
                {
                    var control = new UserControl();
                    created.Add(control);
                    return control;
                },
                areaName,
                TimeSpan.MaxValue);

            Assert.Equal(2, created.Count);
            Assert.NotSame(created[0], created[1]);
            var hosts = created.Select(c => Assert.IsType<ToastNotification>(c.Parent)).ToArray();
            Assert.NotSame(hosts[0], hosts[1]);
        }
        finally
        {
            window1.Close();
            window2.Close();
        }
    }

    [WpfFact]
    public async Task Show_SingleInstance_TwoLoadedAreas_ShowsItOnceWithoutReparenting()
    {
        var areaName = NewAreaName();
        var (window1, _) = await ShowAreaAsync(areaName);
        var (window2, _) = await ShowAreaAsync(areaName);
        var content = new UserControl();
        var manager = new ToastNotificationManager(Dispatcher.CurrentDispatcher);

        try
        {
            manager.Show(
                useDesktop: false,
                content,
                areaName,
                TimeSpan.MaxValue);

            Assert.IsType<ToastNotification>(content.Parent);
        }
        finally
        {
            window1.Close();
            window2.Close();
        }
    }

    private static string NewAreaName()
        => $"Area{Guid.NewGuid():N}";

    private static async Task<(Window Window, ToastNotificationArea Area)> ShowAreaAsync(
        string areaName)
    {
        var area = new ToastNotificationArea { Name = areaName };
        var loaded = new TaskCompletionSource();
        area.Loaded += (_, _) => loaded.TrySetResult();

        var window = CreateHiddenWindow();
        window.Content = area;
        window.Show();
        await loaded.Task.WaitAsync(TestTimeout);

        return (window, area);
    }

    private static Window CreateHiddenWindow()
        => new()
        {
            Width = 100,
            Height = 100,
            Left = -10000,
            Top = -10000,
            WindowStartupLocation = WindowStartupLocation.Manual,
            WindowStyle = WindowStyle.None,
            ShowInTaskbar = false,
            ShowActivated = false,
        };
}