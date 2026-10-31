namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// Pane operations of a <see cref="DockPanelPro"/>: activate, close, restore, float, dock back and move.
/// </summary>
public sealed partial class DockPanelPro
{
    /// <summary>
    /// Occurs when a pane is floated, docked, closed or restored.
    /// </summary>
    public event EventHandler<DockPaneStateChangedEventArgs>? PaneStateChanged;

    /// <summary>
    /// Gets every pane of every region, in region and tab order.
    /// </summary>
    /// <returns>The panes.</returns>
    public IEnumerable<DockPane> GetPanes()
        => GetRegions().SelectMany(x => x.Panes).ToList();

    /// <summary>
    /// Finds a pane by its <see cref="DockPane.PaneId"/>.
    /// </summary>
    /// <param name="paneId">The pane identifier.</param>
    /// <returns>The pane, or <see langword="null"/> when no pane has the identifier.</returns>
    public DockPane? FindPane(string paneId)
        => string.IsNullOrEmpty(paneId)
            ? null
            : GetPanes().FirstOrDefault(x => string.Equals(x.PaneId, paneId, StringComparison.Ordinal));

    /// <summary>
    /// Activates a pane: selects its tab when it is docked, or activates its window when it floats.
    /// </summary>
    /// <param name="pane">The pane.</param>
    /// <returns><see langword="true"/> when the pane was activated; <see langword="false"/> when it is closed or not in this panel.</returns>
    public bool ActivatePane(DockPane pane)
    {
        ArgumentNullException.ThrowIfNull(pane);

        if (!Owns(pane))
        {
            return false;
        }

        switch (pane.State)
        {
            case DockPaneState.Docked:
                pane.Region!.SetCurrentValue(DockRegion.ActivePaneProperty, pane);
                return true;
            case DockPaneState.Floating:
                if (floatingWindows.TryGetValue(pane, out var window))
                {
                    window.Activate();
                }

                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Closes (hides) a pane. A closed pane keeps its region and position and can be restored with
    /// <see cref="RestorePane(DockPane)"/>.
    /// </summary>
    /// <param name="pane">The pane.</param>
    /// <returns><see langword="true"/> when the pane was closed; <see langword="false"/> when it cannot close, is already closed or is not in this panel.</returns>
    public bool ClosePane(DockPane pane)
    {
        ArgumentNullException.ThrowIfNull(pane);

        if (!CanClosePane(pane))
        {
            return false;
        }

        CloseFloatingWindow(pane);
        ChangePaneState(pane, DockPaneState.Closed);
        return true;
    }

    /// <summary>
    /// Restores a closed pane to its region and position, and activates it.
    /// </summary>
    /// <param name="pane">The pane.</param>
    /// <returns><see langword="true"/> when the pane was restored; <see langword="false"/> when it is not closed or not in this panel.</returns>
    public bool RestorePane(DockPane pane)
    {
        ArgumentNullException.ThrowIfNull(pane);

        if (!Owns(pane) || pane.State != DockPaneState.Closed)
        {
            return false;
        }

        ChangePaneState(pane, DockPaneState.Docked, activate: true);
        return true;
    }

    /// <summary>
    /// Restores a closed pane, found by its <see cref="DockPane.PaneId"/>, to its region and position, and activates it.
    /// </summary>
    /// <param name="paneId">The pane identifier.</param>
    /// <returns><see langword="true"/> when the pane was restored.</returns>
    public bool RestorePane(string paneId)
        => FindPane(paneId) is { } pane && RestorePane(pane);

    /// <summary>
    /// Floats a docked pane into its own tool window, owned by the window of this panel.
    /// The pane remembers its region and position, so <see cref="RedockPane"/> puts it back there.
    /// </summary>
    /// <param name="pane">The pane.</param>
    /// <returns><see langword="true"/> when the pane was floated; <see langword="false"/> when floating is not allowed
    /// (<see cref="AllowFloating"/>, <see cref="DockPane.CanFloat"/>), the pane is not docked or not in this panel.</returns>
    public bool FloatPane(DockPane pane)
    {
        ArgumentNullException.ThrowIfNull(pane);

        if (!CanFloatPane(pane))
        {
            return false;
        }

        ChangePaneState(pane, DockPaneState.Floating);
        OpenFloatingWindow(pane);
        return true;
    }

    /// <summary>
    /// Docks a floating pane back to its previous region and position, and activates it.
    /// </summary>
    /// <param name="pane">The pane.</param>
    /// <returns><see langword="true"/> when the pane was docked; <see langword="false"/> when it is not floating or not in this panel.</returns>
    public bool RedockPane(DockPane pane)
    {
        ArgumentNullException.ThrowIfNull(pane);

        if (!Owns(pane) || pane.State != DockPaneState.Floating)
        {
            return false;
        }

        CloseFloatingWindow(pane);
        ChangePaneState(pane, DockPaneState.Docked, activate: true);
        return true;
    }

    /// <summary>
    /// Docks every floating pane back to its region.
    /// </summary>
    public void RedockAllPanes()
    {
        foreach (var pane in GetPanes().Where(x => x.State == DockPaneState.Floating))
        {
            RedockPane(pane);
        }
    }

    /// <summary>
    /// Moves a pane into another region of this panel as a docked, active tab.
    /// </summary>
    /// <param name="pane">The pane.</param>
    /// <param name="region">The target region; it must be a child of this panel.</param>
    /// <param name="index">The tab position in the target region's <see cref="DockRegion.Panes"/>; a negative value appends.</param>
    /// <returns><see langword="true"/> when the pane was moved; <see langword="false"/> when the pane or region is not in this panel.</returns>
    public bool MovePane(
        DockPane pane,
        DockRegion region,
        int index = -1)
    {
        ArgumentNullException.ThrowIfNull(pane);
        ArgumentNullException.ThrowIfNull(region);

        if (!Owns(pane) || !ReferenceEquals(region.Parent, this))
        {
            return false;
        }

        CloseFloatingWindow(pane);

        var oldState = pane.State;
        var source = pane.Region!;
        if (!ReferenceEquals(source, region))
        {
            source.Panes.Remove(pane);
            pane.SetState(DockPaneState.Docked);
            InsertPane(region, pane, index);
        }
        else
        {
            var currentIndex = region.Panes.IndexOf(pane);
            var targetIndex = index < 0 || index >= region.Panes.Count ? region.Panes.Count - 1 : index;
            pane.SetState(DockPaneState.Docked);
            if (currentIndex != targetIndex)
            {
                region.Panes.Move(currentIndex, targetIndex);
            }
        }

        region.RefreshDockedPanes(activate: pane);
        RaisePaneStateChanged(pane, oldState, DockPaneState.Docked);
        return true;
    }

    internal bool CanFloatPane(DockPane? pane)
        => pane is not null &&
           AllowFloating &&
           pane.CanFloat &&
           pane.State == DockPaneState.Docked &&
           Owns(pane);

    internal bool CanClosePane(DockPane? pane)
        => pane is not null &&
           pane.CanClose &&
           pane.State != DockPaneState.Closed &&
           Owns(pane);

    private static void InsertPane(
        DockRegion region,
        DockPane pane,
        int index)
    {
        if (index < 0 || index >= region.Panes.Count)
        {
            region.Panes.Add(pane);
        }
        else
        {
            region.Panes.Insert(index, pane);
        }
    }

    private IEnumerable<DockRegion> GetRegions()
        => InternalChildren.OfType<DockRegion>();

    private bool Owns(DockPane pane)
        => pane.Region is not null && ReferenceEquals(pane.Region.Parent, this);

    private void ChangePaneState(
        DockPane pane,
        DockPaneState newState,
        bool activate = false)
    {
        var oldState = pane.State;
        pane.SetState(newState);
        pane.Region?.RefreshDockedPanes(activate ? pane : null);
        RaisePaneStateChanged(pane, oldState, newState);
    }

    private void RaisePaneStateChanged(
        DockPane pane,
        DockPaneState oldState,
        DockPaneState newState)
    {
        CommandManager.InvalidateRequerySuggested();
        NotifyLayoutChanged();
        if (oldState != newState)
        {
            PaneStateChanged?.Invoke(this, new DockPaneStateChangedEventArgs(pane, oldState, newState));
        }
    }

    private static void RegisterCommandBindings()
    {
        Register(DockPanelProCommands.FloatPane, (panel, pane, _) => panel.FloatPane(pane!), (panel, pane, _) => panel.CanFloatPane(pane));
        Register(DockPanelProCommands.RedockPane, (panel, pane, _) => panel.RedockPane(pane!), (panel, pane, _) => pane is not null && pane.State == DockPaneState.Floating && panel.Owns(pane));
        Register(DockPanelProCommands.ClosePane, (panel, pane, _) => panel.ClosePane(pane!), (panel, pane, _) => panel.CanClosePane(pane));
        Register(DockPanelProCommands.NextPane, (_, _, region) => region!.SelectNextPane(), (_, _, region) => region?.DockedPanes.Count > 1);
        Register(DockPanelProCommands.PreviousPane, (_, _, region) => region!.SelectNextPane(reverse: true), (_, _, region) => region?.DockedPanes.Count > 1);

        static void Register(
            RoutedCommand command,
            Action<DockPanelPro, DockPane?, DockRegion?> execute,
            Func<DockPanelPro, DockPane?, DockRegion?, bool> canExecute)
            => CommandManager.RegisterClassCommandBinding(
                typeof(DockPanelPro),
                new CommandBinding(
                    command,
                    (sender, e) =>
                    {
                        var panel = (DockPanelPro)sender;
                        var (pane, region) = ResolveCommandTarget(e.Parameter, e.OriginalSource);
                        if (canExecute(panel, pane, region))
                        {
                            execute(panel, pane, region);
                            e.Handled = true;
                        }
                    },
                    (sender, e) =>
                    {
                        var panel = (DockPanelPro)sender;
                        var (pane, region) = ResolveCommandTarget(e.Parameter, e.OriginalSource);
                        e.CanExecute = canExecute(panel, pane, region);
                        e.Handled = true;
                    }));
    }

    /// <summary>
    /// Finds the pane and region a command acts on: the parameter pane (or region), otherwise the pane or region
    /// the command was raised from (using the region's active pane).
    /// </summary>
    private static (DockPane? Pane, DockRegion? Region) ResolveCommandTarget(
        object? parameter,
        object? source)
    {
        switch (parameter)
        {
            case DockPane parameterPane:
                return (parameterPane, parameterPane.Region);
            case DockRegion parameterRegion:
                return (parameterRegion.ActivePane, parameterRegion);
        }

        DockPane? pane = null;
        var current = source as DependencyObject;
        while (current is not null)
        {
            switch (current)
            {
                case DockPane foundPane:
                    pane ??= foundPane;
                    break;
                case DockPaneTabItem { Pane: { } tabPane }:
                    pane ??= tabPane;
                    break;
                case DockRegion foundRegion:
                    return (pane ?? foundRegion.ActivePane, foundRegion);
            }

            current = current is Visual or System.Windows.Media.Media3D.Visual3D
                ? VisualTreeHelper.GetParent(current) ?? LogicalTreeHelper.GetParent(current)
                : LogicalTreeHelper.GetParent(current);
        }

        return (pane, pane?.Region);
    }
}