namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// Floating tool windows of a <see cref="DockPanelPro"/>.
/// </summary>
public sealed partial class DockPanelPro
{
    private const double DefaultFloatingWidth = 320;
    private const double DefaultFloatingHeight = 240;
    private const double MinimumFloatingWidth = 160;
    private const double MinimumFloatingHeight = 120;

    private static readonly DependencyProperty[] InheritedWindowProperties =
    [
        FlowDirectionProperty,
        LanguageProperty,
        TextElement.FontFamilyProperty,
        TextElement.FontSizeProperty,
        TextElement.FontStyleProperty,
        TextElement.FontWeightProperty,
        TextElement.FontStretchProperty,
    ];

    private readonly Dictionary<DockPane, DockFloatingWindow> floatingWindows = [];
    private Window? ownerWindow;
    private bool isOwnerClosing;

    /// <summary>
    /// Gets the floating tool window that hosts a pane, if any.
    /// </summary>
    internal Window? GetFloatingWindow(DockPane pane)
        => floatingWindows.GetValueOrDefault(pane);

    /// <summary>
    /// Floats a pane while its tab or header is dragged, and lets the user keep dragging its new window.
    /// </summary>
    internal void FloatPaneByDrag(DockPane pane)
    {
        if (!CanFloatPane(pane) || !IsLoaded)
        {
            return;
        }

        var cursor = DockScreenHelper.GetCursorPosition(this);
        var size = GetFloatingSize(pane);
        pane.FloatingBounds = new Rect(
            cursor.X - (size.Width / 2),
            cursor.Y - 12,
            size.Width,
            size.Height);

        Mouse.Capture(null);
        if (!FloatPane(pane) ||
            !floatingWindows.TryGetValue(pane, out var window) ||
            Mouse.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        try
        {
            window.DragMove();
        }
        catch (InvalidOperationException)
        {
            // The mouse button was released before the move started.
        }
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        AttachOwnerWindow(Window.GetWindow(this));
        LoadStoredLayoutOnce();
        CultureManager.UiCultureChanged -= OnUiCultureChanged;
        CultureManager.UiCultureChanged += OnUiCultureChanged;

        // Windows hidden while the panel was unloaded (e.g. on another tab) come back with it.
        foreach (var window in floatingWindows.Values.ToList())
        {
            ShowHiddenFloatingWindow(window);
        }

        foreach (var pane in GetPanes().Where(x => x.State == DockPaneState.Floating))
        {
            OpenFloatingWindow(pane);
        }
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        CultureManager.UiCultureChanged -= OnUiCultureChanged;
        FlushLayoutChanged();
        HideDockGuides();

        // The panes stay floating and their windows are only hidden, so switching to another tab (or swapping the view
        // out and back) keeps them as they are. The owner subscription stays: when the owner window closes, it closes the
        // hidden windows and the panes keep their floating state.
        foreach (var window in floatingWindows.Values)
        {
            if (!window.IsClosed)
            {
                window.Hide();
            }
        }

        if (floatingWindows.Count == 0)
        {
            AttachOwnerWindow(null);
        }
    }

    private void ShowHiddenFloatingWindow(DockFloatingWindow window)
    {
        if (window.IsClosed || window.IsVisible)
        {
            return;
        }

        if (ownerWindow is not null &&
            ownerWindow.IsLoaded &&
            !ReferenceEquals(window.Owner, ownerWindow) &&
            !ReferenceEquals(ownerWindow, window))
        {
            window.Owner = ownerWindow;
        }

        window.ApplyTexts();
        window.Show();
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        foreach (var window in floatingWindows.Values)
        {
            window.ApplyTexts();
        }

        ApplySplitterTexts();
    }

    private void AttachOwnerWindow(Window? window)
    {
        if (ReferenceEquals(ownerWindow, window))
        {
            return;
        }

        if (ownerWindow is not null)
        {
            ownerWindow.Closing -= OnOwnerWindowClosing;
        }

        ownerWindow = window;
        isOwnerClosing = false;

        if (ownerWindow is not null)
        {
            ownerWindow.Closing += OnOwnerWindowClosing;
        }
    }

    private void OnOwnerWindowClosing(
        object? sender,
        CancelEventArgs e)
    {
        if (e.Cancel)
        {
            return;
        }

        // Save a pending change while the floating windows are still open.
        FlushLayoutChanged();

        // The owner closes its owned windows right after this; that must not close or dock the panes.
        isOwnerClosing = true;
        _ = Dispatcher.BeginInvoke(DispatcherPriority.Background, () => isOwnerClosing = false);
    }

    private void OpenFloatingWindow(DockPane pane)
    {
        if (!IsLoaded ||
            pane.Region is null ||
            floatingWindows.ContainsKey(pane))
        {
            return;
        }

        var bounds = pane.FloatingBounds.IsEmpty
            ? GetDefaultFloatingBounds(pane)
            : pane.FloatingBounds;
        bounds = DockScreenHelper.ClampToWorkAreas(bounds, DockScreenHelper.GetWorkAreas(this));
        pane.FloatingBounds = bounds;

        pane.Region.ReleasePaneToWindow(pane);
        var window = new DockFloatingWindow(pane)
        {
            Left = bounds.Left,
            Top = bounds.Top,
            Width = bounds.Width,
            Height = bounds.Height,
        };

        // The window becomes the logical parent of the pane: keep the DataContext of the pane's region flowing in.
        window.SetBinding(DataContextProperty, new Binding(nameof(DataContext)) { Source = pane.Region });
        InheritAncestorSettings(window, pane.Region);

        if (ownerWindow is not null && ownerWindow.IsLoaded && !ReferenceEquals(ownerWindow, window))
        {
            window.Owner = ownerWindow;
        }

        AddFloatingWindowCommandBindings(window, pane);

        window.BoundsChanged += OnFloatingWindowBoundsChanged;
        window.Closing += OnFloatingWindowClosing;
        window.Closed += OnFloatingWindowClosed;
        floatingWindows[pane] = window;
        window.Show();
        HookFloatingWindow(window);

        // A window created at the DPI of one monitor and placed on a monitor with another DPI is rescaled by WPF when it
        // gets there; now that it has that monitor's DPI, apply the bounds again (a no-op when nothing moved).
        if (!window.IsClosed && window.CurrentBounds != bounds)
        {
            window.Left = bounds.Left;
            window.Top = bounds.Top;
            window.Width = bounds.Width;
            window.Height = bounds.Height;
        }
    }

    /// <summary>
    /// Routes the Dock and Close commands of a floating window (outside the panel's element tree) to this panel.
    /// </summary>
    private void AddFloatingWindowCommandBindings(
        DockFloatingWindow window,
        DockPane pane)
    {
        window.CommandBindings.Add(new CommandBinding(
            DockPanelProCommands.RedockPane,
            (_, e) =>
            {
                RedockPane(pane);
                e.Handled = true;
            },
            (_, e) =>
            {
                e.CanExecute = pane.State == DockPaneState.Floating;
                e.Handled = true;
            }));
        window.CommandBindings.Add(new CommandBinding(
            DockPanelProCommands.ClosePane,
            (_, e) =>
            {
                ClosePane(pane);
                e.Handled = true;
            },
            (_, e) =>
            {
                e.CanExecute = CanClosePane(pane);
                e.Handled = true;
            }));
    }

    /// <summary>
    /// A floating window is outside the element tree of the panel: give it the resource dictionaries of the
    /// pane's ancestors (implicit styles, DynamicResources) and follow their flow direction, language and font.
    /// </summary>
    private static void InheritAncestorSettings(
        DockFloatingWindow window,
        FrameworkElement start)
    {
        var dictionaries = new List<ResourceDictionary>();
        DependencyObject? current = start;
        while (current is not null)
        {
            if (current is FrameworkElement element)
            {
                dictionaries.Add(element.Resources);
            }

            current = current is Visual
                ? VisualTreeHelper.GetParent(current) ?? LogicalTreeHelper.GetParent(current)
                : LogicalTreeHelper.GetParent(current);
        }

        // A later merged dictionary wins, so the nearest ancestor goes last.
        dictionaries.Reverse();
        window.InheritResources(dictionaries);

        foreach (var property in InheritedWindowProperties)
        {
            window.SetBinding(property, new Binding { Path = new PropertyPath(property), Source = start, Mode = BindingMode.OneWay });
        }
    }

    private void CloseFloatingWindow(DockPane pane)
    {
        if (!floatingWindows.Remove(pane, out var window))
        {
            return;
        }

        DetachFloatingWindow(window);
        window.CloseIfOpen();
    }

    private void DetachFloatingWindow(DockFloatingWindow window)
    {
        UnhookFloatingWindow(window);
        window.BoundsChanged -= OnFloatingWindowBoundsChanged;
        window.Closing -= OnFloatingWindowClosing;
        window.Closed -= OnFloatingWindowClosed;
        var pane = window.Pane;
        pane.FloatingBounds = window.CurrentBounds;
        window.ReleasePane();
        window.ReleaseResources();
        pane.Region?.AcceptPaneFromWindow(pane);
    }

    private void OnFloatingWindowBoundsChanged(
        object? sender,
        EventArgs e)
    {
        if (sender is DockFloatingWindow window)
        {
            window.Pane.FloatingBounds = window.CurrentBounds;
            NotifyLayoutChanged();
        }
    }

    private void OnFloatingWindowClosing(
        object? sender,
        CancelEventArgs e)
    {
        if (!e.Cancel && sender is DockFloatingWindow window)
        {
            OnFloatingWindowClosedExternally(window);
        }
    }

    private void OnFloatingWindowClosed(
        object? sender,
        EventArgs e)
    {
        if (sender is DockFloatingWindow window)
        {
            OnFloatingWindowClosedExternally(window);
        }
    }

    /// <summary>
    /// Handles a floating window closed by the user (its close button, Alt+F4) or by its owner closing.
    /// </summary>
    private void OnFloatingWindowClosedExternally(DockFloatingWindow window)
    {
        var pane = window.Pane;
        if (!floatingWindows.TryGetValue(pane, out var tracked) ||
            !ReferenceEquals(tracked, window))
        {
            return;
        }

        floatingWindows.Remove(pane);
        DetachFloatingWindow(window);

        if (isOwnerClosing || (ownerWindow is not null && !ownerWindow.IsVisible))
        {
            // The application is closing: keep the floating state for a layout saved afterwards.
            return;
        }

        // The window's close button: close the pane when it may close, otherwise dock it back.
        if (CanClosePane(pane))
        {
            ChangePaneState(pane, DockPaneState.Closed);
        }
        else
        {
            ChangePaneState(pane, DockPaneState.Docked, activate: true);
        }
    }

    private Rect GetDefaultFloatingBounds(DockPane pane)
    {
        var size = GetFloatingSize(pane);
        var origin = pane.IsVisible
            ? DockScreenHelper.ToScreenDip(pane, new Point(0, 0))
            : IsVisible
                ? DockScreenHelper.ToScreenDip(this, new Point((ActualWidth - size.Width) / 2, (ActualHeight - size.Height) / 2))
                : new Point(100, 100);
        return new Rect(origin, size);
    }

    private static Size GetFloatingSize(DockPane pane)
        => pane.IsVisible && pane.ActualWidth > 0 && pane.ActualHeight > 0
            ? new Size(
                System.Math.Max(MinimumFloatingWidth, pane.ActualWidth),
                System.Math.Max(MinimumFloatingHeight, pane.ActualHeight + DockFloatingWindow.TitleBarExtent))
            : new Size(DefaultFloatingWidth, DefaultFloatingHeight);
}