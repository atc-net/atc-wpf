namespace Atc.Wpf.Controls.Tests.Layouts;

public sealed class DockPanelProPaneTests
{
    [StaFact]
    public void A_region_with_panes_docks_them_in_order_and_activates_the_first()
    {
        var (_, bottom, _) = CreatePanel();

        Assert.True(bottom.HasPanes);
        Assert.Equal("output,errors,find", string.Join(",", bottom.DockedPanes.Select(x => x.PaneId)));
        Assert.Equal("output", bottom.ActivePane?.PaneId);
        Assert.All(bottom.Panes, x => Assert.Equal(DockPaneState.Docked, x.State));
        Assert.All(bottom.Panes, x => Assert.Same(bottom, x.Region));
    }

    [StaFact]
    public void A_region_without_panes_keeps_its_plain_content()
    {
        var region = new DockRegion { Content = "plain" };

        Assert.False(region.HasPanes);
        Assert.Empty(region.DockedPanes);
        Assert.Null(region.ActivePane);
        Assert.Equal("plain", region.Content);
    }

    [StaFact]
    public void ActivatePane_selects_the_tab_and_raises_ActivePaneChanged()
    {
        var (panel, bottom, _) = CreatePanel();
        var errors = panel.FindPane("errors")!;
        DockPane? raised = null;
        bottom.ActivePaneChanged += (_, e) => raised = e.NewValue;

        var result = panel.ActivatePane(errors);

        Assert.True(result);
        Assert.Same(errors, bottom.ActivePane);
        Assert.Same(errors, raised);
    }

    [StaFact]
    public void SelectNextPane_cycles_forward_and_backward_and_wraps()
    {
        var (_, bottom, _) = CreatePanel();

        Assert.True(bottom.SelectNextPane());
        Assert.Equal("errors", bottom.ActivePane?.PaneId);
        Assert.True(bottom.SelectNextPane());
        Assert.True(bottom.SelectNextPane());
        Assert.Equal("output", bottom.ActivePane?.PaneId);
        Assert.True(bottom.SelectNextPane(reverse: true));
        Assert.Equal("find", bottom.ActivePane?.PaneId);
    }

    [StaFact]
    public void SelectNextPane_returns_false_with_a_single_docked_pane()
    {
        var region = new DockRegion();
        region.Panes.Add(new DockPane { PaneId = "only" });

        Assert.False(region.SelectNextPane());
        Assert.Equal("only", region.ActivePane?.PaneId);
    }

    [StaFact]
    public void ClosePane_hides_the_tab_activates_the_neighbour_and_raises_PaneStateChanged()
    {
        var (panel, bottom, _) = CreatePanel();
        var output = panel.FindPane("output")!;
        var events = new List<DockPaneStateChangedEventArgs>();
        panel.PaneStateChanged += (_, e) => events.Add(e);

        var result = panel.ClosePane(output);

        Assert.True(result);
        Assert.Equal(DockPaneState.Closed, output.State);
        Assert.Equal("errors,find", string.Join(",", bottom.DockedPanes.Select(x => x.PaneId)));
        Assert.Contains(output, bottom.Panes);
        Assert.Equal("errors", bottom.ActivePane?.PaneId);
        var raised = Assert.Single(events);
        Assert.Same(output, raised.Pane);
        Assert.Equal(DockPaneState.Docked, raised.OldState);
        Assert.Equal(DockPaneState.Closed, raised.NewState);
    }

    [StaFact]
    public void ClosePane_of_the_last_tab_activates_the_previous_tab()
    {
        var (panel, bottom, _) = CreatePanel();
        var find = panel.FindPane("find")!;
        panel.ActivatePane(find);

        panel.ClosePane(find);

        Assert.Equal("errors", bottom.ActivePane?.PaneId);
    }

    [StaFact]
    public void ClosePane_is_refused_when_the_pane_cannot_close()
    {
        var (panel, _, _) = CreatePanel();
        var errors = panel.FindPane("errors")!;
        errors.CanClose = false;

        Assert.False(panel.ClosePane(errors));
        Assert.Equal(DockPaneState.Docked, errors.State);
    }

    [StaFact]
    public void RestorePane_puts_a_closed_pane_back_at_its_position_and_activates_it()
    {
        var (panel, bottom, _) = CreatePanel();
        panel.ClosePane(panel.FindPane("errors")!);

        var result = panel.RestorePane("errors");

        Assert.True(result);
        Assert.Equal("output,errors,find", string.Join(",", bottom.DockedPanes.Select(x => x.PaneId)));
        Assert.Equal("errors", bottom.ActivePane?.PaneId);
        Assert.Equal(DockPaneState.Docked, panel.FindPane("errors")!.State);
    }

    [StaFact]
    public void RestorePane_returns_false_for_an_unknown_or_docked_pane()
    {
        var (panel, _, _) = CreatePanel();

        Assert.False(panel.RestorePane("unknown"));
        Assert.False(panel.RestorePane("output"));
    }

    [StaFact]
    public void FloatPane_is_refused_unless_floating_is_allowed()
    {
        var (panel, _, _) = CreatePanel();
        panel.AllowFloating = false;
        var output = panel.FindPane("output")!;

        Assert.False(panel.FloatPane(output));
        Assert.Equal(DockPaneState.Docked, output.State);
    }

    [StaFact]
    public void FloatPane_is_refused_when_the_pane_opts_out()
    {
        var (panel, _, _) = CreatePanel();
        var output = panel.FindPane("output")!;
        output.CanFloat = false;

        Assert.False(panel.FloatPane(output));
        Assert.Equal(DockPaneState.Docked, output.State);
    }

    [StaFact]
    public void FloatPane_takes_the_tab_out_but_keeps_its_home_region()
    {
        var (panel, bottom, _) = CreatePanel();
        var errors = panel.FindPane("errors")!;
        panel.ActivatePane(errors);

        var result = panel.FloatPane(errors);

        Assert.True(result);
        Assert.True(errors.IsFloating);
        Assert.Equal(DockPaneState.Floating, errors.State);
        Assert.Equal("output,find", string.Join(",", bottom.DockedPanes.Select(x => x.PaneId)));
        Assert.Same(bottom, errors.Region);
        Assert.Equal("find", bottom.ActivePane?.PaneId);
    }

    [StaFact]
    public void RedockPane_returns_a_floating_pane_to_its_previous_region_and_position()
    {
        var (panel, bottom, _) = CreatePanel();
        var errors = panel.FindPane("errors")!;
        panel.FloatPane(errors);

        var result = panel.RedockPane(errors);

        Assert.True(result);
        Assert.False(errors.IsFloating);
        Assert.Equal("output,errors,find", string.Join(",", bottom.DockedPanes.Select(x => x.PaneId)));
        Assert.Same(errors, bottom.ActivePane);
    }

    [StaFact]
    public void RedockAllPanes_docks_every_floating_pane()
    {
        var (panel, bottom, right) = CreatePanel();
        panel.FloatPane(panel.FindPane("output")!);
        panel.FloatPane(panel.FindPane("properties")!);

        panel.RedockAllPanes();

        Assert.Equal(3, bottom.DockedPanes.Count);
        Assert.Equal(2, right.DockedPanes.Count);
    }

    [StaFact]
    public void MovePane_moves_a_pane_into_another_region_as_the_active_tab()
    {
        var (panel, bottom, right) = CreatePanel();
        var find = panel.FindPane("find")!;

        var result = panel.MovePane(find, right);

        Assert.True(result);
        Assert.Same(right, find.Region);
        Assert.DoesNotContain(find, bottom.Panes);
        Assert.Equal("properties,toolbox,find", string.Join(",", right.DockedPanes.Select(x => x.PaneId)));
        Assert.Same(find, right.ActivePane);
        Assert.Equal(DockPaneState.Docked, find.State);
    }

    [StaFact]
    public void MovePane_with_an_index_inserts_the_tab_there()
    {
        var (panel, _, right) = CreatePanel();

        panel.MovePane(panel.FindPane("find")!, right, 0);

        Assert.Equal("find,properties,toolbox", string.Join(",", right.DockedPanes.Select(x => x.PaneId)));
    }

    [StaFact]
    public void A_side_region_whose_panes_are_all_away_takes_no_space()
    {
        var (panel, _, right) = CreatePanel();
        var center = (DockRegion)panel.Children[2];
        Arrange(panel);
        var centerWidthBefore = center.RenderSize.Width;

        panel.ClosePane(panel.FindPane("properties")!);
        panel.FloatPane(panel.FindPane("toolbox")!);
        Arrange(panel);

        Assert.Equal(0, right.RenderSize.Width);
        Assert.True(center.RenderSize.Width > centerWidthBefore);
    }

    [StaFact]
    public void GetPanes_lists_every_pane_of_every_region()
    {
        var (panel, _, _) = CreatePanel();

        Assert.Equal("output,errors,find,properties,toolbox", string.Join(",", panel.GetPanes().Select(x => x.PaneId)));
    }

    [StaFact]
    public void Panes_inherit_the_DataContext_of_their_region_through_the_logical_tree()
    {
        var (panel, _, _) = CreatePanel();
        panel.DataContext = "model";

        Assert.Equal("model", panel.FindPane("output")!.DataContext);
    }

    [StaFact]
    public void FloatPane_command_floats_the_parameter_pane_and_can_execute_follows_AllowFloating()
    {
        var (panel, _, _) = CreatePanel();
        var output = panel.FindPane("output")!;

        Assert.True(DockPanelProCommands.FloatPane.CanExecute(output, output));
        panel.AllowFloating = false;
        Assert.False(DockPanelProCommands.FloatPane.CanExecute(output, output));
        panel.AllowFloating = true;

        DockPanelProCommands.FloatPane.Execute(output, output);

        Assert.True(output.IsFloating);
        Assert.False(DockPanelProCommands.FloatPane.CanExecute(output, panel));
        Assert.True(DockPanelProCommands.RedockPane.CanExecute(output, panel));

        DockPanelProCommands.RedockPane.Execute(output, panel);

        Assert.False(output.IsFloating);
    }

    [StaFact]
    public void ClosePane_command_without_a_parameter_closes_the_active_pane_of_the_target_region()
    {
        var (panel, bottom, _) = CreatePanel();
        panel.ActivatePane(panel.FindPane("errors")!);

        DockPanelProCommands.ClosePane.Execute(null, bottom);

        Assert.Equal(DockPaneState.Closed, panel.FindPane("errors")!.State);
    }

    [StaFact]
    public void ClosePane_command_cannot_execute_for_a_pane_that_cannot_close()
    {
        var (panel, _, _) = CreatePanel();
        var output = panel.FindPane("output")!;
        output.CanClose = false;

        Assert.False(DockPanelProCommands.ClosePane.CanExecute(output, output));
    }

    [StaFact]
    public void NextPane_and_PreviousPane_commands_cycle_the_tabs_of_the_target_region()
    {
        var (_, bottom, _) = CreatePanel();

        DockPanelProCommands.NextPane.Execute(null, bottom);
        Assert.Equal("errors", bottom.ActivePane?.PaneId);

        DockPanelProCommands.PreviousPane.Execute(null, bottom);
        DockPanelProCommands.PreviousPane.Execute(null, bottom);
        Assert.Equal("find", bottom.ActivePane?.PaneId);
    }

    [StaFact]
    public void ResetLayout_redocks_floating_panes_and_restores_closed_panes()
    {
        var (panel, bottom, _) = CreatePanel();
        panel.FloatPane(panel.FindPane("output")!);
        panel.ClosePane(panel.FindPane("find")!);

        panel.ResetLayout();

        Assert.Equal("output,errors,find", string.Join(",", bottom.DockedPanes.Select(x => x.PaneId)));
    }

    internal static (DockPanelPro Panel, DockRegion Bottom, DockRegion Right) CreatePanel()
    {
        var panel = new DockPanelPro { AllowFloating = true };

        var bottom = new DockRegion { RegionId = "bottom", Height = 150 };
        DockPanelPro.SetDock(bottom, DockPosition.Bottom);
        bottom.Panes.Add(new DockPane { PaneId = "output", Header = "Output", Content = "output text" });
        bottom.Panes.Add(new DockPane { PaneId = "errors", Header = "Errors", Content = "error list" });
        bottom.Panes.Add(new DockPane { PaneId = "find", Header = "Find", Content = "find results" });

        var right = new DockRegion { RegionId = "right", Width = 200 };
        DockPanelPro.SetDock(right, DockPosition.Right);
        right.Panes.Add(new DockPane { PaneId = "properties", Header = "Properties" });
        right.Panes.Add(new DockPane { PaneId = "toolbox", Header = "Toolbox" });

        var center = new DockRegion { RegionId = "center", Content = "editor" };

        panel.Children.Add(bottom);
        panel.Children.Add(right);
        panel.Children.Add(center);
        return (panel, bottom, right);
    }

    private static void Arrange(DockPanelPro panel)
    {
        panel.Measure(new Size(800, 600));
        panel.Arrange(new Rect(0, 0, 800, 600));
    }
}