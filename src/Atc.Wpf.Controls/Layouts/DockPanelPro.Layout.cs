namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// Pane persistence of a <see cref="DockPanelPro"/> layout.
/// </summary>
public sealed partial class DockPanelPro
{
    private static readonly JsonSerializerOptions LayoutWriteOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private static readonly JsonSerializerOptions LayoutReadOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
    };

    private void SavePaneStates(Internal.DockPanelLayout layout)
    {
        layout.Panes = [];
        foreach (var pane in GetPanes().Where(x => !string.IsNullOrEmpty(x.PaneId)))
        {
            var bounds = floatingWindows.TryGetValue(pane, out var window)
                ? window.CurrentBounds
                : pane.FloatingBounds;
            var hasBounds = !bounds.IsEmpty && IsFinite(bounds);
            layout.Panes.Add(new Internal.DockPaneLayoutState
            {
                PaneId = pane.PaneId,
                State = pane.State,
                FloatingLeft = hasBounds ? bounds.Left : 0,
                FloatingTop = hasBounds ? bounds.Top : 0,
                FloatingWidth = hasBounds ? bounds.Width : 0,
                FloatingHeight = hasBounds ? bounds.Height : 0,
            });
        }
    }

    private void LoadPaneStates(Internal.DockPanelLayout layout)
    {
        if (layout.Version < 2)
        {
            return;
        }

        LoadPaneGroups(layout);
        LoadPaneStateList(layout);
        LoadActivePanes(layout);
    }

    /// <summary>
    /// Restores which region each pane belongs to and the tab order; panes the layout does not list keep their place after the listed ones.
    /// </summary>
    private void LoadPaneGroups(Internal.DockPanelLayout layout)
    {
        foreach (var regionState in layout.Regions.Where(x => x.PaneIds is not null))
        {
            var region = FindRegionById(regionState.RegionId);
            if (region is null)
            {
                continue;
            }

            var index = 0;
            foreach (var paneId in regionState.PaneIds!)
            {
                var pane = FindPane(paneId);
                if (pane is null)
                {
                    continue;
                }

                CloseFloatingWindow(pane);
                if (!ReferenceEquals(pane.Region, region))
                {
                    pane.Region!.Panes.Remove(pane);
                    region.Panes.Insert(System.Math.Min(index, region.Panes.Count), pane);
                }
                else
                {
                    var current = region.Panes.IndexOf(pane);
                    var target = System.Math.Min(index, region.Panes.Count - 1);
                    if (current != target)
                    {
                        region.Panes.Move(current, target);
                    }
                }

                index++;
            }
        }
    }

    /// <summary>
    /// Restores docked, floating (with the window bounds moved onto the screens) and closed panes.
    /// </summary>
    private void LoadPaneStateList(Internal.DockPanelLayout layout)
    {
        foreach (var paneState in layout.Panes ?? [])
        {
            if (string.IsNullOrEmpty(paneState.PaneId) || FindPane(paneState.PaneId) is not { } pane)
            {
                continue;
            }

            if (paneState.FloatingWidth > 0 && paneState.FloatingHeight > 0)
            {
                var bounds = new Rect(paneState.FloatingLeft, paneState.FloatingTop, paneState.FloatingWidth, paneState.FloatingHeight);
                if (IsFinite(bounds))
                {
                    pane.FloatingBounds = bounds;
                }
            }

            var state = paneState.State;
            if (state == DockPaneState.Floating && !(AllowFloating && pane.CanFloat))
            {
                state = DockPaneState.Docked;
            }

            if (state == pane.State)
            {
                continue;
            }

            CloseFloatingWindow(pane);
            ChangePaneState(pane, state);
            if (state == DockPaneState.Floating)
            {
                OpenFloatingWindow(pane);
            }
        }
    }

    private void LoadActivePanes(Internal.DockPanelLayout layout)
    {
        foreach (var regionState in layout.Regions)
        {
            var region = FindRegionById(regionState.RegionId);
            if (region is null || string.IsNullOrEmpty(regionState.ActivePaneId))
            {
                continue;
            }

            var pane = region.DockedPanes.FirstOrDefault(x => string.Equals(x.PaneId, regionState.ActivePaneId, StringComparison.Ordinal));
            if (pane is not null)
            {
                region.SetCurrentValue(DockRegion.ActivePaneProperty, pane);
            }
        }
    }

    private static bool IsFinite(Rect bounds)
        => double.IsFinite(bounds.Left) &&
           double.IsFinite(bounds.Top) &&
           double.IsFinite(bounds.Width) &&
           double.IsFinite(bounds.Height);
}