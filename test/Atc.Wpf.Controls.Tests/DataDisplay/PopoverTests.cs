namespace Atc.Wpf.Controls.Tests.DataDisplay;

[Collection(nameof(PackResourceTestGroup))]
public sealed class PopoverTests : IDisposable
{
    public PopoverTests()
    {
        // pack://application URIs need the Application type initialized.
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
    }

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void ClickMode_ButtonAnchorClicked_Opens()
    {
        // A keyboard Enter/Space on a Button raises only Click, not a mouse button down.
        var button = new Button { Content = "Open" };
        var sut = CreatePopover(button);

        RunInWindow(sut, () =>
        {
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, button));

            Assert.True(sut.IsOpen);
            sut.IsOpen = false;
        });
    }

    [StaFact]
    public void ClickMode_ButtonAnchorClickedWithTheMouse_TogglesOnlyOnce()
    {
        var button = new Button { Content = "Open" };
        var sut = CreatePopover(button);

        RunInWindow(sut, () =>
        {
            button.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
            {
                RoutedEvent = Mouse.MouseDownEvent,
            });
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, button));

            Assert.True(sut.IsOpen);
            sut.IsOpen = false;
        });
    }

    [StaFact]
    public void ClickMode_NonButtonAnchorPressed_StillOpens()
    {
        var anchor = new Border { Width = 50, Height = 20, Background = Brushes.Gray };
        var sut = CreatePopover(anchor);

        RunInWindow(sut, () =>
        {
            anchor.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
            {
                RoutedEvent = Mouse.MouseDownEvent,
            });

            Assert.True(sut.IsOpen);
            sut.IsOpen = false;
        });
    }

    [StaFact]
    public void ClickMode_FocusableAnchorEnterPressed_Opens()
    {
        var anchor = new Border { Width = 50, Height = 20, Focusable = true };
        var sut = CreatePopover(anchor);

        RunInWindow(sut, () =>
        {
            anchor.RaiseEvent(KeyPress.CreateArgs(Keyboard.KeyDownEvent, Key.Enter));

            Assert.True(sut.IsOpen);
            sut.IsOpen = false;
        });
    }

    [StaFact]
    public void HoverMode_AnchorGetsKeyboardFocus_Opens()
    {
        var anchor = new Button { Content = "Info" };
        var sut = CreatePopover(anchor);
        sut.TriggerMode = PopoverTriggerMode.Hover;

        RunInWindow(sut, () =>
        {
            anchor.RaiseEvent(new KeyboardFocusChangedEventArgs(Keyboard.PrimaryDevice, 0, oldFocus: null, newFocus: anchor)
            {
                RoutedEvent = Keyboard.GotKeyboardFocusEvent,
            });

            Assert.True(sut.IsOpen);
            sut.IsOpen = false;
        });
    }

    [StaFact]
    public void Unloaded_WhileOpen_ClosesAndDetachesFromTheWindow()
    {
        var window = CreateWindow();
        window.Show();
        try
        {
            var button = new Button { Content = "Open" };
            var popover = CreatePopover(button);
            var closed = false;
            popover.Closed += (_, _) => closed = true;
            window.Content = popover;
            window.UpdateLayout();
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, button));
            Assert.True(popover.IsOpen);
            Assert.Same(window, DismissWindow(popover));

            window.Content = null;
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));

            Assert.False(popover.IsOpen);
            Assert.True(closed);
            Assert.Null(DismissWindow(popover));
        }
        finally
        {
            window.Close();
        }
    }

    // The window the open popover listens to for light dismiss; it must be released when the popover goes away.
    private static Window? DismissWindow(Popover popover)
        => (Window?)typeof(Popover)
            .GetField("dismissWindow", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(popover);

    private static Popover CreatePopover(object anchor)
    {
        var dictionary = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Controls;component/DataDisplay/Popover.xaml", UriKind.Absolute),
        };

        return new Popover
        {
            Style = (Style)dictionary["AtcApps.Styles.Popover"],
            Anchor = anchor,
            PopoverContent = new TextBlock { Text = "Content" },
        };
    }

    private static Window CreateWindow()
        => new()
        {
            Width = 300,
            Height = 200,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };

    private static void RunInWindow(
        FrameworkElement content,
        Action action)
    {
        var window = CreateWindow();
        window.Content = content;
        window.Show();
        try
        {
            window.UpdateLayout();
            action();
        }
        finally
        {
            window.Close();
        }
    }
}