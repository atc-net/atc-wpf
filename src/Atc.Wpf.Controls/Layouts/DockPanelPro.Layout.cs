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

    private static readonly TimeSpan LayoutChangedDelay = TimeSpan.FromMilliseconds(300);

    private DispatcherTimer? layoutChangedTimer;
    private bool hasLoadedStoredLayout;
    private bool isLoadingStoredLayout;

    /// <summary>
    /// Occurs after the layout changed: a pane was floated, docked, closed, restored or moved, the active tab changed,
    /// a region was expanded or collapsed, a splitter was moved, a floating window was moved or resized, or the layout
    /// was loaded or reset. Changes in quick succession are coalesced into one event, raised shortly after the last one;
    /// a pending change is raised at once when the owner window closes or the panel is unloaded.
    /// Changes made before the panel is loaded are not reported.
    /// </summary>
    public event EventHandler? LayoutChanged;

    /// <summary>
    /// Gets a value indicating whether a <see cref="LayoutChanged"/> is waiting to be raised.
    /// </summary>
    internal bool IsLayoutChangePending
        => layoutChangedTimer?.IsEnabled == true;

    /// <summary>
    /// Raises a pending <see cref="LayoutChanged"/> now (and auto-saves).
    /// </summary>
    internal void FlushLayoutChanged()
    {
        if (!IsLayoutChangePending)
        {
            return;
        }

        layoutChangedTimer!.Stop();
        LayoutChanged?.Invoke(this, EventArgs.Empty);

        if (AutoSaveLayout && !string.IsNullOrEmpty(LayoutId))
        {
            (LayoutStore ?? FileDockLayoutStore.Default).Save(LayoutId, SaveLayout());
        }
    }

    /// <summary>
    /// Starts (or restarts) the delay of a coalesced <see cref="LayoutChanged"/>.
    /// </summary>
    private void NotifyLayoutChanged()
    {
        if (!IsLoaded || isLoadingStoredLayout)
        {
            return;
        }

        if (layoutChangedTimer is null)
        {
            layoutChangedTimer = new DispatcherTimer(DispatcherPriority.Background, Dispatcher)
            {
                Interval = LayoutChangedDelay,
            };
            layoutChangedTimer.Tick += (_, _) => FlushLayoutChanged();
        }

        layoutChangedTimer.Stop();
        layoutChangedTimer.Start();
    }

    /// <summary>
    /// With <see cref="AutoSaveLayout"/>, loads the stored layout the first time the panel is loaded.
    /// </summary>
    private void LoadStoredLayoutOnce()
    {
        if (hasLoadedStoredLayout ||
            !AutoSaveLayout ||
            string.IsNullOrEmpty(LayoutId))
        {
            return;
        }

        hasLoadedStoredLayout = true;
        var json = (LayoutStore ?? FileDockLayoutStore.Default).Load(LayoutId);
        if (string.IsNullOrWhiteSpace(json))
        {
            return;
        }

        isLoadingStoredLayout = true;
        try
        {
            LoadLayout(json);
        }
        finally
        {
            isLoadingStoredLayout = false;
        }
    }

    private void OnRegionLayoutEvent(
        object sender,
        RoutedEventArgs e)
    {
        if (e.OriginalSource is DockRegion region && ReferenceEquals(region.Parent, this))
        {
            NotifyLayoutChanged();
        }
    }

    private void OnRegionActivePaneChanged(
        object sender,
        RoutedPropertyChangedEventArgs<DockPane?> e)
        => OnRegionLayoutEvent(sender, e);

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
        ApplyFloatingWindows();
    }

    /// <summary>
    /// Opens a window for every floating pane that has none, and moves the open windows to the (clamped) restored bounds.
    /// </summary>
    private void ApplyFloatingWindows()
    {
        foreach (var pane in GetPanes().Where(x => x.State == DockPaneState.Floating))
        {
            if (!floatingWindows.TryGetValue(pane, out var window))
            {
                OpenFloatingWindow(pane);
                continue;
            }

            if (pane.FloatingBounds.IsEmpty)
            {
                continue;
            }

            var bounds = DockScreenHelper.ClampToWorkAreas(pane.FloatingBounds, DockScreenHelper.GetWorkAreas(this));
            window.WindowState = WindowState.Normal;
            window.Left = bounds.Left;
            window.Top = bounds.Top;
            window.Width = bounds.Width;
            window.Height = bounds.Height;
            pane.FloatingBounds = bounds;
        }
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