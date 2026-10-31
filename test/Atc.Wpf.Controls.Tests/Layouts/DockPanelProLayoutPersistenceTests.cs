namespace Atc.Wpf.Controls.Tests.Layouts;

public sealed class DockPanelProLayoutPersistenceTests
{
    [StaFact]
    public void SaveLayout_writes_version_2_with_pane_groups_and_states()
    {
        var (panel, _, _) = DockPanelProPaneTests.CreatePanel();

        var json = panel.SaveLayout();

        using var document = System.Text.Json.JsonDocument.Parse(json);
        var root = document.RootElement;
        Assert.Equal(2, root.GetProperty("version").GetInt32());
        var bottom = root.GetProperty("regions").EnumerateArray().First(x => x.GetProperty("regionId").GetString() == "bottom");
        Assert.Equal("output,errors,find", string.Join(",", bottom.GetProperty("paneIds").EnumerateArray().Select(x => x.GetString())));
        Assert.Equal("output", bottom.GetProperty("activePaneId").GetString());
        Assert.Equal(5, root.GetProperty("panes").GetArrayLength());
    }

    [StaFact]
    public void Round_trip_restores_active_tab_closed_floating_and_moved_panes()
    {
        var (source, _, sourceRight) = DockPanelProPaneTests.CreatePanel();
        source.ActivatePane(source.FindPane("find")!);
        source.ClosePane(source.FindPane("errors")!);
        var toolbox = source.FindPane("toolbox")!;
        toolbox.FloatingBounds = new Rect(100, 120, 320, 240);
        source.FloatPane(toolbox);
        source.MovePane(source.FindPane("output")!, sourceRight, 0);
        source.ActivatePane(source.FindPane("properties")!);
        var json = source.SaveLayout();

        var (target, bottom, right) = DockPanelProPaneTests.CreatePanel();
        target.LoadLayout(json);

        Assert.Equal("find", string.Join(",", bottom.DockedPanes.Select(x => x.PaneId)));
        Assert.Equal("errors,find", string.Join(",", bottom.Panes.Select(x => x.PaneId)));
        Assert.Equal("find", bottom.ActivePane?.PaneId);
        Assert.Equal(DockPaneState.Closed, target.FindPane("errors")!.State);
        Assert.Equal("output,properties", string.Join(",", right.DockedPanes.Select(x => x.PaneId)));
        Assert.Equal("output,properties,toolbox", string.Join(",", right.Panes.Select(x => x.PaneId)));
        Assert.Equal("properties", right.ActivePane?.PaneId);
        var floating = target.FindPane("toolbox")!;
        Assert.Equal(DockPaneState.Floating, floating.State);
        Assert.Equal(320, floating.FloatingBounds.Width);
        Assert.Equal(240, floating.FloatingBounds.Height);
        _ = sourceRight;
    }

    [StaFact]
    public void SaveLayout_writes_floating_panes_with_their_window_bounds()
    {
        var (panel, _, _) = DockPanelProPaneTests.CreatePanel();
        var output = panel.FindPane("output")!;
        output.FloatingBounds = new Rect(10, 20, 300, 200);
        panel.FloatPane(output);

        var json = panel.SaveLayout();

        using var document = System.Text.Json.JsonDocument.Parse(json);
        var state = document.RootElement.GetProperty("panes").EnumerateArray().First(x => x.GetProperty("paneId").GetString() == "output");
        Assert.Equal((int)DockPaneState.Floating, state.GetProperty("state").GetInt32());
        Assert.Equal(10, state.GetProperty("floatingLeft").GetDouble());
        Assert.Equal(20, state.GetProperty("floatingTop").GetDouble());
        Assert.Equal(300, state.GetProperty("floatingWidth").GetDouble());
        Assert.Equal(200, state.GetProperty("floatingHeight").GetDouble());
    }

    [StaFact]
    public void A_floating_pane_in_the_layout_docks_when_the_target_panel_does_not_allow_floating()
    {
        var (source, _, _) = DockPanelProPaneTests.CreatePanel();
        source.FloatPane(source.FindPane("output")!);
        var json = source.SaveLayout();

        var (target, bottom, _) = DockPanelProPaneTests.CreatePanel();
        target.AllowFloating = false;
        target.LoadLayout(json);

        Assert.Equal(DockPaneState.Docked, target.FindPane("output")!.State);
        Assert.Equal(3, bottom.DockedPanes.Count);
    }

    [StaFact]
    public void A_floating_pane_in_the_target_redocks_when_the_layout_says_docked()
    {
        var (source, _, _) = DockPanelProPaneTests.CreatePanel();
        var json = source.SaveLayout();
        var (target, bottom, _) = DockPanelProPaneTests.CreatePanel();
        target.FloatPane(target.FindPane("errors")!);

        target.LoadLayout(json);

        Assert.Equal("output,errors,find", string.Join(",", bottom.DockedPanes.Select(x => x.PaneId)));
    }

    [StaFact]
    public void A_version_1_layout_without_pane_data_loads_and_leaves_the_panes_alone()
    {
        var (panel, bottom, _) = DockPanelProPaneTests.CreatePanel();
        panel.ActivatePane(panel.FindPane("errors")!);

        const string json = """
            {
              "layoutId": "MyLayout",
              "regions": [
                {
                  "regionId": "bottom",
                  "dock": 3,
                  "width": 0,
                  "height": 180,
                  "isExpanded": false
                }
              ]
            }
            """;

        panel.LoadLayout(json);

        Assert.Equal(180, bottom.Height);
        Assert.False(bottom.IsExpanded);
        Assert.Equal(3, bottom.DockedPanes.Count);
        Assert.Equal("errors", bottom.ActivePane?.PaneId);
    }

    [StaFact]
    public void Unknown_pane_ids_and_regions_in_the_layout_are_ignored()
    {
        var (panel, bottom, _) = DockPanelProPaneTests.CreatePanel();

        const string json = """
            {
              "version": 2,
              "regions": [
                { "regionId": "bottom", "dock": 3, "height": 150, "isExpanded": true, "paneIds": [ "ghost", "find", "output" ], "activePaneId": "ghost" },
                { "regionId": "nowhere", "dock": 0, "width": 100, "isExpanded": true, "paneIds": [ "errors" ] }
              ],
              "panes": [
                { "paneId": "ghost", "state": 1 },
                { "paneId": "find", "state": 0 }
              ]
            }
            """;

        var exception = Record.Exception(() => panel.LoadLayout(json));

        Assert.Null(exception);
        Assert.Equal("find,output,errors", string.Join(",", bottom.Panes.Select(x => x.PaneId)));
        Assert.Equal("output", bottom.ActivePane?.PaneId);
    }

    [StaFact]
    public void Panes_without_an_id_are_not_saved_and_not_touched_on_load()
    {
        var panel = new DockPanelPro { AllowFloating = true };
        var region = new DockRegion { RegionId = "r" };
        var anonymous = new DockPane { Header = "No id" };
        region.Panes.Add(anonymous);
        panel.Children.Add(region);

        var json = panel.SaveLayout();
        panel.LoadLayout(json);

        Assert.DoesNotContain("No id", json, StringComparison.Ordinal);
        Assert.Same(anonymous, region.ActivePane);
    }

    [Fact]
    public void ClampToWorkAreas_keeps_bounds_that_are_inside_a_screen()
    {
        var bounds = new Rect(100, 100, 300, 200);

        var result = DockScreenHelper.ClampToWorkAreas(bounds, [new Rect(0, 0, 1920, 1040)]);

        Assert.Equal(bounds, result);
    }

    [Fact]
    public void ClampToWorkAreas_moves_bounds_from_a_missing_monitor_onto_the_nearest_screen()
    {
        var bounds = new Rect(2500, 300, 300, 200);

        var result = DockScreenHelper.ClampToWorkAreas(bounds, [new Rect(0, 0, 1920, 1040)]);

        Assert.Equal(new Rect(1620, 300, 300, 200), result);
    }

    [Fact]
    public void ClampToWorkAreas_shrinks_bounds_larger_than_the_screen()
    {
        var bounds = new Rect(-50, -50, 4000, 3000);

        var result = DockScreenHelper.ClampToWorkAreas(bounds, [new Rect(0, 0, 1920, 1040)]);

        Assert.Equal(new Rect(0, 0, 1920, 1040), result);
    }

    [Fact]
    public void DeviceToDip_converts_each_monitor_with_its_own_dpi()
    {
        var primary = new Rect(0, 0, 2560, 1400);
        var secondary = new Rect(2560, 0, 1920, 1400);

        var primaryDip = DockScreenHelper.DeviceToDip(primary, 96, 96, Matrix.Identity);
        var secondaryDip = DockScreenHelper.DeviceToDip(secondary, 144, 144, Matrix.Identity);

        Assert.Equal(new Rect(0, 0, 2560, 1400), primaryDip);
        Assert.Equal(2560 / 1.5, secondaryDip.Left, 6);
        Assert.Equal(1920 / 1.5, secondaryDip.Width, 6);
        Assert.Equal(1400 / 1.5, secondaryDip.Height, 6);
    }

    [Fact]
    public void DeviceToDip_without_a_monitor_dpi_uses_the_fallback_transform()
    {
        var fromDevice = new Matrix(1 / 1.25, 0, 0, 1 / 1.25, 0, 0);

        var result = DockScreenHelper.DeviceToDip(new Rect(0, 0, 1000, 500), 0, 0, fromDevice);

        Assert.Equal(new Rect(0, 0, 800, 400), result);
    }

    [Fact]
    public void SelectWorkArea_picks_the_monitor_in_device_pixels_although_the_dip_areas_overlap()
    {
        IReadOnlyList<(Rect DeviceWork, double DpiX, double DpiY)> monitors =
        [
            (new Rect(0, 0, 2560, 1400), 96, 96),
            (new Rect(2560, 0, 1920, 1400), 144, 144),
        ];

        var onSecondary = DockScreenHelper.SelectWorkArea(new Point(3000, 200), monitors, Matrix.Identity);
        var onPrimary = DockScreenHelper.SelectWorkArea(new Point(100, 100), monitors, Matrix.Identity);
        var offScreen = DockScreenHelper.SelectWorkArea(new Point(5000, 100), monitors, Matrix.Identity);

        Assert.Equal(1280, onSecondary.Width, 6);
        Assert.Equal(2560 / 1.5, onSecondary.Left, 6);
        Assert.Equal(new Rect(0, 0, 2560, 1400), onPrimary);
        Assert.Equal(onSecondary, offScreen);
    }

    [Fact]
    public void GetWorkAreas_returns_at_least_one_non_empty_area()
    {
        var areas = DockScreenHelper.GetWorkAreas(reference: null);

        Assert.NotEmpty(areas);
        Assert.All(areas, x => Assert.True(x.Width > 0 && x.Height > 0));
    }

    [Fact]
    public void ClampToWorkAreas_uses_the_screen_with_the_largest_overlap()
    {
        var left = new Rect(0, 0, 1920, 1040);
        var right = new Rect(1920, 0, 1280, 984);
        var bounds = new Rect(1700, 900, 400, 200);

        var result = DockScreenHelper.ClampToWorkAreas(bounds, [left, right]);

        Assert.Equal(new Rect(1520, 840, 400, 200), result);
    }

    [Fact]
    public void ClampToWorkAreas_without_screens_returns_the_bounds_unchanged()
    {
        var bounds = new Rect(10, 10, 100, 100);

        Assert.Equal(bounds, DockScreenHelper.ClampToWorkAreas(bounds, []));
    }
}