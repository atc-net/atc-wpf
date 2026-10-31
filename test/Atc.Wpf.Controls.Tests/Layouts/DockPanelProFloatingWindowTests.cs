namespace Atc.Wpf.Controls.Tests.Layouts;

[Collection(nameof(PackResourceTestGroup))]
public sealed class DockPanelProFloatingWindowTests : IDisposable
{
    public DockPanelProFloatingWindowTests()
    {
        // pack://application URIs need the Application type initialized.
        _ = typeof(Atc.Wpf.MarkupExtensions.StaticResourceExtension).Assembly;
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
    }

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void FloatPane_opens_an_owned_tool_window_hosting_the_pane_and_RedockPane_closes_it()
    {
        var (panel, bottom, _) = DockPanelProPaneTests.CreatePanel();
        var host = Show(panel);
        try
        {
            var errors = panel.FindPane("errors")!;

            panel.FloatPane(errors);
            var window = panel.GetFloatingWindow(errors);

            Assert.NotNull(window);
            Assert.True(window.IsVisible);
            Assert.Same(host, window.Owner);
            Assert.Same(errors, window.Content);
            Assert.Equal("Errors", window.Title);
            Assert.False(window.ShowInTaskbar);

            panel.RedockPane(errors);
            host.UpdateLayout();

            Assert.Null(panel.GetFloatingWindow(errors));
            Assert.False(window.IsVisible);
            Assert.Same(errors, bottom.ActivePane);
            Assert.True(errors.IsVisible);
            Assert.Same(bottom, LogicalTreeHelper.GetParent(errors));
        }
        finally
        {
            panel.RedockAllPanes();
            host.Close();
        }
    }

    [StaFact]
    public void Floating_the_active_tab_hands_its_visual_over_without_a_parent_conflict()
    {
        var (panel, bottom, _) = DockPanelProPaneTests.CreatePanel();
        var host = Show(panel);
        try
        {
            var output = panel.FindPane("output")!;
            Assert.True(output.IsVisible);

            panel.FloatPane(output);
            host.UpdateLayout();
            var window = panel.GetFloatingWindow(output)!;
            window.UpdateLayout();

            Assert.True(output.IsVisible);
            Assert.Same(window, Window.GetWindow(output));
            Assert.Equal("errors", bottom.ActivePane?.PaneId);
            Assert.True(bottom.ActivePane!.IsVisible);
        }
        finally
        {
            panel.RedockAllPanes();
            host.Close();
        }
    }

    [StaFact]
    public void Floating_a_pane_of_a_collapsed_region_works()
    {
        var (panel, bottom, _) = DockPanelProPaneTests.CreatePanel();
        var host = Show(panel);
        try
        {
            bottom.IsCollapsible = true;
            bottom.IsExpanded = false;
            host.UpdateLayout();
            var output = panel.FindPane("output")!;

            var exception = Record.Exception(() =>
            {
                panel.FloatPane(output);
                panel.GetFloatingWindow(output)!.UpdateLayout();
            });

            Assert.Null(exception);
            Assert.True(output.IsVisible);
        }
        finally
        {
            panel.RedockAllPanes();
            host.Close();
        }
    }

    [StaFact]
    public void Closing_the_floating_window_closes_a_closable_pane_and_redocks_the_others()
    {
        var (panel, bottom, _) = DockPanelProPaneTests.CreatePanel();
        var host = Show(panel);
        try
        {
            var output = panel.FindPane("output")!;
            var errors = panel.FindPane("errors")!;
            errors.CanClose = false;
            panel.FloatPane(output);
            panel.FloatPane(errors);

            panel.GetFloatingWindow(output)!.Close();
            panel.GetFloatingWindow(errors)!.Close();

            Assert.Equal(DockPaneState.Closed, output.State);
            Assert.Equal(DockPaneState.Docked, errors.State);
            Assert.Null(panel.GetFloatingWindow(output));
            Assert.Null(panel.GetFloatingWindow(errors));
            Assert.Equal("errors,find", string.Join(",", bottom.DockedPanes.Select(x => x.PaneId)));
        }
        finally
        {
            panel.RedockAllPanes();
            host.Close();
        }
    }

    [StaFact]
    public void Closing_the_owner_closes_the_floating_windows_but_keeps_the_floating_state()
    {
        var (panel, _, _) = DockPanelProPaneTests.CreatePanel();
        var host = Show(panel);
        var output = panel.FindPane("output")!;
        panel.FloatPane(output);
        var window = panel.GetFloatingWindow(output)!;

        host.Close();

        Assert.False(window.IsVisible);
        Assert.Equal(DockPaneState.Floating, output.State);
        Assert.Null(panel.GetFloatingWindow(output));
    }

    [StaFact]
    public void Unloading_the_panel_closes_the_windows_and_loading_it_again_reopens_them()
    {
        var (panel, _, _) = DockPanelProPaneTests.CreatePanel();
        var host = Show(panel);
        try
        {
            var output = panel.FindPane("output")!;
            panel.FloatPane(output);
            var first = panel.GetFloatingWindow(output)!;

            host.Content = null;
            DispatchPending();

            Assert.False(first.IsVisible);
            Assert.Null(panel.GetFloatingWindow(output));
            Assert.Equal(DockPaneState.Floating, output.State);

            host.Content = panel;
            DispatchPending();

            var second = panel.GetFloatingWindow(output);
            Assert.NotNull(second);
            Assert.True(second.IsVisible);
        }
        finally
        {
            panel.RedockAllPanes();
            host.Close();
        }
    }

    [StaFact]
    public void FloatingBounds_follow_the_window_and_are_used_when_floating_again()
    {
        var (panel, _, _) = DockPanelProPaneTests.CreatePanel();
        var host = Show(panel);
        try
        {
            var output = panel.FindPane("output")!;
            output.FloatingBounds = new Rect(120, 130, 310, 220);
            panel.FloatPane(output);
            var window = panel.GetFloatingWindow(output)!;

            Assert.Equal(120, window.Left);
            Assert.Equal(130, window.Top);
            Assert.Equal(310, window.Width);

            window.Left = 140;
            window.Width = 330;
            panel.RedockPane(output);

            Assert.Equal(new Rect(140, 130, 330, 220), output.FloatingBounds);
        }
        finally
        {
            panel.RedockAllPanes();
            host.Close();
        }
    }

    [StaFact]
    public void LoadLayout_keeps_an_already_floating_pane_in_its_window_and_moves_it_to_the_saved_bounds()
    {
        var (panel, _, _) = DockPanelProPaneTests.CreatePanel();
        var host = Show(panel);
        try
        {
            var output = panel.FindPane("output")!;
            output.FloatingBounds = new Rect(100, 110, 300, 200);
            panel.FloatPane(output);
            var json = panel.SaveLayout();
            var window = panel.GetFloatingWindow(output)!;
            window.Left = 180;
            window.Width = 340;

            panel.LoadLayout(json);

            var reloaded = panel.GetFloatingWindow(output);
            Assert.NotNull(reloaded);
            Assert.True(reloaded.IsVisible);
            Assert.Same(output, reloaded.Content);
            Assert.Equal(100, reloaded.Left);
            Assert.Equal(110, reloaded.Top);
            Assert.Equal(300, reloaded.Width);
        }
        finally
        {
            panel.RedockAllPanes();
            host.Close();
        }
    }

    [StaFact]
    public void LoadLayout_opens_windows_for_panes_that_float_in_the_layout()
    {
        var (source, _, _) = DockPanelProPaneTests.CreatePanel();
        source.FindPane("find")!.FloatingBounds = new Rect(50, 60, 280, 190);
        source.FloatPane(source.FindPane("find")!);
        var json = source.SaveLayout();
        var (panel, bottom, _) = DockPanelProPaneTests.CreatePanel();
        var host = Show(panel);
        try
        {
            panel.LoadLayout(json);

            var window = panel.GetFloatingWindow(panel.FindPane("find")!);
            Assert.NotNull(window);
            Assert.True(window.IsVisible);
            Assert.Equal("output,errors", string.Join(",", bottom.DockedPanes.Select(x => x.PaneId)));
        }
        finally
        {
            panel.RedockAllPanes();
            host.Close();
        }
    }

    [StaFact]
    public void A_floating_pane_keeps_the_DataContext_of_its_region()
    {
        var (panel, _, _) = DockPanelProPaneTests.CreatePanel();
        var host = Show(panel);
        try
        {
            panel.DataContext = "model";
            var output = panel.FindPane("output")!;

            panel.FloatPane(output);

            Assert.Equal("model", output.DataContext);
            panel.DataContext = "changed";
            Assert.Equal("changed", output.DataContext);
        }
        finally
        {
            panel.RedockAllPanes();
            host.Close();
        }
    }

    [StaFact]
    public void The_floating_window_context_menu_docks_and_closes_the_pane()
    {
        var (panel, bottom, _) = DockPanelProPaneTests.CreatePanel();
        var host = Show(panel);
        try
        {
            var output = panel.FindPane("output")!;
            var find = panel.FindPane("find")!;
            panel.FloatPane(output);
            panel.FloatPane(find);
            var outputMenu = panel.GetFloatingWindow(output)!.ContextMenu;
            var findMenu = panel.GetFloatingWindow(find)!.ContextMenu;

            var dock = (MenuItem)outputMenu.Items[0];
            ((RoutedCommand)dock.Command).Execute(dock.CommandParameter, dock.CommandTarget);
            var close = (MenuItem)findMenu.Items[1];
            ((RoutedCommand)close.Command).Execute(close.CommandParameter, close.CommandTarget);

            Assert.Equal(DockPaneState.Docked, output.State);
            Assert.Equal(DockPaneState.Closed, find.State);
            Assert.Null(panel.GetFloatingWindow(output));
            Assert.Null(panel.GetFloatingWindow(find));
            Assert.Same(output, bottom.ActivePane);
        }
        finally
        {
            panel.RedockAllPanes();
            host.Close();
        }
    }

    [StaFact]
    public void The_tab_strip_exposes_the_panes_as_selectable_tabs_with_named_float_and_close_buttons()
    {
        var (panel, bottom, _) = DockPanelProPaneTests.CreatePanel();
        var host = Show(panel);
        try
        {
            var tabControl = FindVisualChild<TabControl>(bottom);
            Assert.NotNull(tabControl);
            var peer = UIElementAutomationPeer.CreatePeerForElement(tabControl);
            var tabs = peer.GetChildren().Where(x => x.GetAutomationControlType() == AutomationControlType.TabItem).ToList();

            Assert.Equal("Output,Errors,Find", string.Join(",", tabs.Select(x => x.GetName())));
            var errorsTab = (ISelectionItemProvider)tabs[1].GetPattern(PatternInterface.SelectionItem);
            errorsTab.Select();
            Assert.Equal("errors", bottom.ActivePane?.PaneId);

            var buttons = FindVisualChildren<Button>(bottom).ToList();
            var floatName = string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.Float1, "Errors");
            var closeName = string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.Close1, "Errors");
            var floatButton = buttons.Single(x => System.Windows.Automation.AutomationProperties.GetName(x) == floatName);
            Assert.Contains(buttons, x => System.Windows.Automation.AutomationProperties.GetName(x) == closeName);

            var invoke = (IInvokeProvider)UIElementAutomationPeer.CreatePeerForElement(floatButton).GetPattern(PatternInterface.Invoke);
            invoke.Invoke();
            DispatchPending();

            Assert.True(panel.FindPane("errors")!.IsFloating);
        }
        finally
        {
            panel.RedockAllPanes();
            host.Close();
        }
    }

    [StaFact]
    public void Regions_and_panes_have_automation_peers()
    {
        var (panel, bottom, _) = DockPanelProPaneTests.CreatePanel();
        var host = Show(panel);
        try
        {
            var regionPeer = UIElementAutomationPeer.CreatePeerForElement(bottom);
            var panePeer = UIElementAutomationPeer.CreatePeerForElement(bottom.ActivePane!);
            var panelPeer = UIElementAutomationPeer.CreatePeerForElement(panel);

            Assert.Equal(AutomationControlType.Pane, panelPeer.GetAutomationControlType());
            Assert.Equal(AutomationControlType.Group, regionPeer.GetAutomationControlType());
            Assert.Equal("bottom", regionPeer.GetAutomationId());
            Assert.Equal(AutomationControlType.Pane, panePeer.GetAutomationControlType());
            Assert.Equal("Output", panePeer.GetName());
            Assert.Equal("output", panePeer.GetAutomationId());
        }
        finally
        {
            host.Close();
        }
    }

    private static Window Show(DockPanelPro panel)
    {
        var dictionary = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Controls;component/Layouts/DockPanelPro.xaml", UriKind.Absolute),
        };
        var resources = new ResourceDictionary();
        resources.MergedDictionaries.Add(dictionary);
        resources[typeof(DockPanelPro)] = dictionary["AtcApps.Styles.DockPanelPro"];
        resources[typeof(DockRegion)] = dictionary["AtcApps.Styles.DockRegion"];
        resources[typeof(DockPane)] = dictionary["AtcApps.Styles.DockPane"];

        var window = new Window
        {
            Resources = resources,
            Content = panel,
            Width = 800,
            Height = 600,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };
        window.Show();
        window.UpdateLayout();
        return window;
    }

    private static void DispatchPending()
        => Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);

    private static T? FindVisualChild<T>(DependencyObject parent)
        where T : DependencyObject
        => FindVisualChildren<T>(parent).FirstOrDefault();

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent)
        where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T match)
            {
                yield return match;
            }

            foreach (var nested in FindVisualChildren<T>(child))
            {
                yield return nested;
            }
        }
    }
}