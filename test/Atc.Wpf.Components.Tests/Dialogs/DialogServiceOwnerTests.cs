namespace Atc.Wpf.Components.Tests.Dialogs;

/// <summary>
/// <see cref="DialogService"/> owns a dialog by the window from its owner resolver, otherwise by the active
/// window (a dialog raised from a second window belongs to that window), otherwise by the main window.
/// </summary>
public sealed class DialogServiceOwnerTests
{
    [WpfFact]
    public void ResolveOwnerWindow_ActiveSecondWindow_IsTheOwner()
    {
        var main = CreateShownWindow();
        var second = CreateShownWindow();

        try
        {
            var owner = DialogService.ResolveOwnerWindow(
                resolvedOwner: null,
                [main, second],
                main,
                window => ReferenceEquals(window, second));

            Assert.Same(second, owner);
        }
        finally
        {
            second.Close();
            main.Close();
        }
    }

    [WpfFact]
    public void ResolveOwnerWindow_NoActiveWindow_FallsBackToTheMainWindow()
    {
        var main = CreateShownWindow();
        var second = CreateShownWindow();

        try
        {
            var owner = DialogService.ResolveOwnerWindow(
                resolvedOwner: null,
                [main, second],
                main,
                _ => false);

            Assert.Same(main, owner);
        }
        finally
        {
            second.Close();
            main.Close();
        }
    }

    [WpfFact]
    public void ResolveOwnerWindow_ActiveWindowHidden_FallsBackToTheMainWindow()
    {
        var main = CreateShownWindow();
        var hidden = new Window();

        try
        {
            var owner = DialogService.ResolveOwnerWindow(
                resolvedOwner: null,
                [main, hidden],
                main,
                window => ReferenceEquals(window, hidden));

            Assert.Same(main, owner);
        }
        finally
        {
            hidden.Close();
            main.Close();
        }
    }

    [WpfFact]
    public void ResolveOwnerWindow_OwnerResolverWindow_WinsOverTheActiveWindow()
    {
        var main = CreateShownWindow();
        var second = CreateShownWindow();
        var resolved = new Window();

        try
        {
            var owner = DialogService.ResolveOwnerWindow(
                resolved,
                [main, second],
                main,
                window => ReferenceEquals(window, second));

            Assert.Same(resolved, owner);
        }
        finally
        {
            resolved.Close();
            second.Close();
            main.Close();
        }
    }

    [WpfFact]
    public void ResolveOwnerWindow_NoWindows_ReturnsNull()
    {
        var owner = DialogService.ResolveOwnerWindow(
            resolvedOwner: null,
            [],
            mainWindow: null,
            _ => true);

        Assert.Null(owner);
    }

    private static Window CreateShownWindow()
    {
        var window = new Window
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

        window.Show();
        return window;
    }
}