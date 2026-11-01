namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// Drag-to-dock guides of a <see cref="DockPanelPro"/>: while a floating window is moved, a compass over the panel
/// offers the regions that hold panes; releasing the window over a target docks its pane there.
/// </summary>
public sealed partial class DockPanelPro
{
    private const double DockGuideSize = 32;
    private const double DockGuideGap = 4;
    private const int WmMoving = 0x0216;
    private const int WmExitSizeMove = 0x0232;

    private readonly Dictionary<DockFloatingWindow, System.Windows.Interop.HwndSourceHook> floatingWindowHooks = [];
    private Popup? dockGuidePopup;
    private Canvas? dockGuideCanvas;
    private DockPane? dockGuidePane;

    /// <summary>
    /// Gets a value indicating whether the drag-to-dock guides are shown.
    /// </summary>
    internal bool IsShowingDockGuides => dockGuidePopup?.IsOpen == true;

    /// <summary>
    /// Gets the dock target under the cursor while the guides are shown.
    /// </summary>
    internal DockPosition? HighlightedDockGuide { get; private set; }

    /// <summary>
    /// Gets the guide targets, in panel coordinates: a compass in the middle of the panel with one target per expanded
    /// region that holds panes (a region with plain content is never a target).
    /// </summary>
    internal IReadOnlyDictionary<DockPosition, Rect> GetDockGuideTargets()
    {
        var targets = new Dictionary<DockPosition, Rect>();
        if (ActualWidth <= 0 || ActualHeight <= 0)
        {
            return targets;
        }

        CategorizeChildren();
        var cx = ActualWidth / 2;
        var cy = ActualHeight / 2;
        var half = DockGuideSize / 2;
        var step = DockGuideSize + DockGuideGap;

        Add(DockPosition.Center, centerRegion, cx - half, cy - half);
        Add(DockPosition.Left, leftRegion, cx - half - step, cy - half);
        Add(DockPosition.Right, rightRegion, cx - half + step, cy - half);
        Add(DockPosition.Top, topRegion, cx - half, cy - half - step);
        Add(DockPosition.Bottom, bottomRegion, cx - half, cy - half + step);
        return targets;

        void Add(
            DockPosition position,
            DockRegion? region,
            double left,
            double top)
        {
            if (region is { HasPanes: true, IsExpanded: true })
            {
                targets[position] = new Rect(left, top, DockGuideSize, DockGuideSize);
            }
        }
    }

    /// <summary>
    /// Returns the guide target at a point in panel coordinates.
    /// </summary>
    internal DockPosition? HitTestDockGuide(Point panelPoint)
    {
        foreach (var (position, bounds) in GetDockGuideTargets())
        {
            if (bounds.Contains(panelPoint))
            {
                return position;
            }
        }

        return null;
    }

    /// <summary>
    /// Shows the guides for a floating pane being moved and highlights the target at the point (panel coordinates).
    /// </summary>
    internal void UpdateDockGuides(
        DockPane pane,
        Point panelPoint)
    {
        if (!IsLoaded || pane.State != DockPaneState.Floating || !Owns(pane))
        {
            return;
        }

        var targets = GetDockGuideTargets();
        if (targets.Count == 0)
        {
            HideDockGuides();
            return;
        }

        dockGuidePane = pane;
        HighlightedDockGuide = HitTestDockGuide(panelPoint);
        RenderDockGuides(targets);
    }

    /// <summary>
    /// Ends a move of a floating window: hides the guides and, when the point (panel coordinates) is over a target,
    /// docks the pane into that region as its active tab.
    /// </summary>
    /// <returns><see langword="true"/> when the pane was docked.</returns>
    internal bool CompleteDockDrag(
        DockPane pane,
        Point panelPoint)
    {
        var wasShowing = IsShowingDockGuides;
        var target = HitTestDockGuide(panelPoint);
        HideDockGuides();

        if (!wasShowing ||
            target is null ||
            pane.State != DockPaneState.Floating ||
            GetRegionAt(target.Value) is not { } region)
        {
            return false;
        }

        var hadFocus = HasFocusWithin(pane);
        if (!MovePane(pane, region))
        {
            return false;
        }

        if (hadFocus)
        {
            ownerWindow?.Activate();
            FocusPane(pane);
        }

        return true;
    }

    private DockRegion? GetRegionAt(DockPosition position)
    {
        CategorizeChildren();
        return position switch
        {
            DockPosition.Left => leftRegion,
            DockPosition.Right => rightRegion,
            DockPosition.Top => topRegion,
            DockPosition.Bottom => bottomRegion,
            _ => centerRegion,
        };
    }

    private void HideDockGuides()
    {
        HighlightedDockGuide = null;
        dockGuidePane = null;
        if (dockGuidePopup is not null)
        {
            dockGuidePopup.IsOpen = false;
        }
    }

    private void RenderDockGuides(
        IReadOnlyDictionary<DockPosition, Rect> targets)
    {
        if (dockGuidePopup is null)
        {
            dockGuideCanvas = new Canvas { IsHitTestVisible = false };
            dockGuidePopup = new Popup
            {
                AllowsTransparency = true,
                IsHitTestVisible = false,
                Focusable = false,
                StaysOpen = true,
                Placement = PlacementMode.Relative,
                PlacementTarget = this,
                Child = dockGuideCanvas,
            };
        }

        var canvas = dockGuideCanvas!;
        canvas.Children.Clear();
        canvas.Width = ActualWidth;
        canvas.Height = ActualHeight;
        canvas.FlowDirection = FlowDirection;

        if (HighlightedDockGuide is { } highlighted &&
            GetRegionAt(highlighted) is { } region &&
            region.IsVisible)
        {
            var bounds = region.TransformToAncestor(this).TransformBounds(new Rect(region.RenderSize));
            canvas.Children.Add(CreatePreview(bounds));
        }

        foreach (var (position, bounds) in targets)
        {
            canvas.Children.Add(CreateGuide(position, bounds, position == HighlightedDockGuide));
        }

        dockGuidePopup!.IsOpen = true;
    }

    private Rectangle CreatePreview(Rect bounds)
    {
        var preview = new Rectangle
        {
            Width = bounds.Width,
            Height = bounds.Height,
            Opacity = 0.25,
        };
        preview.SetResourceReference(Shape.FillProperty, "AtcApps.Brushes.Accent");
        Canvas.SetLeft(preview, bounds.Left);
        Canvas.SetTop(preview, bounds.Top);
        return preview;
    }

    private Border CreateGuide(
        DockPosition position,
        Rect bounds,
        bool isHighlighted)
    {
        // A window outline with the part the pane would take filled in.
        var fill = new Rectangle();
        fill.SetResourceReference(Shape.FillProperty, "AtcApps.Brushes.Accent");
        const double part = (DockGuideSize - 12) / 2;
        switch (position)
        {
            case DockPosition.Left:
                fill.Width = part;
                fill.HorizontalAlignment = HorizontalAlignment.Left;
                break;
            case DockPosition.Right:
                fill.Width = part;
                fill.HorizontalAlignment = HorizontalAlignment.Right;
                break;
            case DockPosition.Top:
                fill.Height = part;
                fill.VerticalAlignment = VerticalAlignment.Top;
                break;
            case DockPosition.Bottom:
                fill.Height = part;
                fill.VerticalAlignment = VerticalAlignment.Bottom;
                break;
        }

        var outline = new Border
        {
            BorderThickness = new Thickness(1.5),
            Child = fill,
        };
        outline.SetResourceReference(Border.BorderBrushProperty, "AtcApps.Brushes.Accent");

        var guide = new Border
        {
            Width = bounds.Width,
            Height = bounds.Height,
            Padding = new Thickness(5),
            CornerRadius = new CornerRadius(3),
            BorderThickness = new Thickness(1),
            Opacity = isHighlighted ? 1 : 0.85,
            Child = outline,
        };
        guide.SetResourceReference(Border.BackgroundProperty, "AtcApps.Brushes.ThemeBackground");
        guide.SetResourceReference(
            Border.BorderBrushProperty,
            isHighlighted ? "AtcApps.Brushes.Accent" : "AtcApps.Brushes.Gray8");
        Canvas.SetLeft(guide, bounds.Left);
        Canvas.SetTop(guide, bounds.Top);
        return guide;
    }

    /// <summary>
    /// Follows the system move loop of a floating window (title bar drag or <see cref="Window.DragMove"/>) to show
    /// the guides and dock the pane when it is released over one.
    /// </summary>
    private void HookFloatingWindow(DockFloatingWindow window)
    {
        var handle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
        if (handle == IntPtr.Zero ||
            System.Windows.Interop.HwndSource.FromHwnd(handle) is not { } source ||
            floatingWindowHooks.ContainsKey(window))
        {
            return;
        }

        System.Windows.Interop.HwndSourceHook hook = (IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
        {
            switch (msg)
            {
                case WmMoving:
                    UpdateDockGuides(window.Pane, GetCursorInPanel());
                    break;
                case WmExitSizeMove when IsShowingDockGuides:
                    var point = GetCursorInPanel();
                    var pane = window.Pane;

                    // Docking closes this window: leave its move loop first.
                    _ = Dispatcher.BeginInvoke(DispatcherPriority.Input, () => CompleteDockDrag(pane, point));
                    break;
            }

            return IntPtr.Zero;
        };

        source.AddHook(hook);
        floatingWindowHooks[window] = hook;
    }

    private void UnhookFloatingWindow(DockFloatingWindow window)
    {
        if (!floatingWindowHooks.Remove(window, out var hook))
        {
            return;
        }

        var handle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
        if (handle != IntPtr.Zero &&
            System.Windows.Interop.HwndSource.FromHwnd(handle) is { } source)
        {
            source.RemoveHook(hook);
        }

        if (dockGuidePane is not null && ReferenceEquals(dockGuidePane, window.Pane))
        {
            HideDockGuides();
        }
    }

    private Point GetCursorInPanel()
        => PresentationSource.FromVisual(this) is not null &&
           DockGuideNativeMethods.GetCursorPos(out var cursor)
            ? PointFromScreen(new Point(cursor.X, cursor.Y))
            : new Point(double.NaN, double.NaN);

    [SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute'", Justification = "LibraryImport requires AllowUnsafeBlocks; DllImport is fine for this user32 call.")]
    private static class DockGuideNativeMethods
    {
        [DllImport("user32.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out NativePoint point);

        [StructLayout(LayoutKind.Sequential)]
        public struct NativePoint
        {
            public int X;
            public int Y;
        }
    }
}