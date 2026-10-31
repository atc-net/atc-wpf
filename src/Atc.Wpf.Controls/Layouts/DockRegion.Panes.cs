namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// Tabbed pane groups of a <see cref="DockRegion"/>.
/// </summary>
[TemplatePart(Name = PartPaneHost, Type = typeof(Decorator))]
[TemplatePart(Name = PartHeaderBorder, Type = typeof(Border))]
[TemplatePart(Name = PartFloatButton, Type = typeof(ButtonBase))]
[TemplatePart(Name = PartCloseButton, Type = typeof(ButtonBase))]
public sealed partial class DockRegion
{
    private const string PartPaneHost = "PART_PaneHost";
    private const string PartHeaderBorder = "HeaderBorder";
    private const string PartFloatButton = "PART_FloatButton";
    private const string PartCloseButton = "PART_CloseButton";

    private static readonly DependencyPropertyKey HasPanesPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(HasPanes),
        typeof(bool),
        typeof(DockRegion),
        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Identifies the <see cref="HasPanes"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HasPanesProperty = HasPanesPropertyKey.DependencyProperty;

    /// <summary>
    /// Identifies the <see cref="ActivePaneChanged"/> routed event.
    /// </summary>
    public static readonly RoutedEvent ActivePaneChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ActivePaneChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<DockPane?>),
        typeof(DockRegion));

    private readonly ObservableCollection<DockPane> dockedPanes = [];
    private readonly List<DockPane> attachedPanes = [];

    /// <summary>
    /// Gets or sets the active (selected) docked pane of this region.
    /// </summary>
    [DependencyProperty(
        PropertyChangedCallback = nameof(OnActivePaneChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private DockPane? activePane;

    /// <summary>
    /// Gets or sets where the tab strip of the panes is placed. The tab strip is hidden while only one pane is docked.
    /// </summary>
    [DependencyProperty(DefaultValue = Dock.Top)]
    private Dock tabStripPlacement;

    private DockPaneTabControl? tabControl;
    private Decorator? paneHost;
    private Border? headerBorder;
    private ButtonBase? floatButton;
    private ButtonBase? closeButton;
    private bool isSyncingPanes;
    private Point? headerDragStart;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockRegion"/> class.
    /// </summary>
    public DockRegion()
    {
        DockedPanes = new ReadOnlyObservableCollection<DockPane>(dockedPanes);
        Panes.CollectionChanged += OnPanesCollectionChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Occurs when the active pane of the region changes.
    /// </summary>
    public event RoutedPropertyChangedEventHandler<DockPane?> ActivePaneChanged
    {
        add => AddHandler(ActivePaneChangedEvent, value);
        remove => RemoveHandler(ActivePaneChangedEvent, value);
    }

    /// <summary>
    /// Gets the panes of this region, in tab order. A region with panes shows them as tabs instead of its
    /// <see cref="ContentControl.Content"/>. Floating and closed panes stay in this collection, so they return to
    /// their position when docked again.
    /// </summary>
    public ObservableCollection<DockPane> Panes { get; } = [];

    /// <summary>
    /// Gets the panes that are currently docked (shown as tabs), in tab order.
    /// </summary>
    public ReadOnlyObservableCollection<DockPane> DockedPanes { get; }

    /// <summary>
    /// Gets a value indicating whether the region has panes (and shows them as tabs).
    /// </summary>
    public bool HasPanes => (bool)GetValue(HasPanesProperty);

    /// <summary>
    /// Gets a value indicating whether a region with panes has none docked, so it takes no space in the panel.
    /// </summary>
    internal bool IsEmptyPaneGroup => HasPanes && dockedPanes.Count == 0;

    internal DockPanelPro? OwnerPanel => Parent as DockPanelPro;

    /// <summary>
    /// Activates the next (or previous) docked pane, wrapping around.
    /// </summary>
    /// <param name="reverse"><see langword="true"/> to go to the previous pane.</param>
    /// <returns><see langword="true"/> when another pane was activated.</returns>
    public bool SelectNextPane(bool reverse = false)
    {
        var count = dockedPanes.Count;
        if (count < 2)
        {
            return false;
        }

        var index = ActivePane is null ? -1 : dockedPanes.IndexOf(ActivePane);
        var next = index < 0
            ? 0
            : (index + (reverse ? -1 : 1) + count) % count;
        SetCurrentValue(ActivePaneProperty, dockedPanes[next]);
        return true;
    }

    /// <inheritdoc />
    protected override System.Collections.IEnumerator LogicalChildren
    {
        get
        {
            var children = new List<object>();
            var baseChildren = base.LogicalChildren;
            while (baseChildren?.MoveNext() == true)
            {
                children.Add(baseChildren.Current);
            }

            children.AddRange(attachedPanes.Where(x => !x.IsHostedInWindow));
            return children.GetEnumerator();
        }
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        if (headerBorder is not null)
        {
            headerBorder.MouseLeftButtonDown -= OnHeaderMouseLeftButtonDown;
            headerBorder.MouseMove -= OnHeaderMouseMove;
            headerBorder.MouseLeftButtonUp -= OnHeaderMouseLeftButtonUp;
        }

        if (paneHost is not null)
        {
            paneHost.Child = null;
        }

        base.OnApplyTemplate();

        paneHost = GetTemplateChild(PartPaneHost) as Decorator;
        headerBorder = GetTemplateChild(PartHeaderBorder) as Border;
        floatButton = GetTemplateChild(PartFloatButton) as ButtonBase;
        closeButton = GetTemplateChild(PartCloseButton) as ButtonBase;

        if (headerBorder is not null)
        {
            headerBorder.MouseLeftButtonDown += OnHeaderMouseLeftButtonDown;
            headerBorder.MouseMove += OnHeaderMouseMove;
            headerBorder.MouseLeftButtonUp += OnHeaderMouseLeftButtonUp;
            headerBorder.ContextMenu = DockPaneTabItem.CreatePaneContextMenu(() => ActivePane, headerBorder);
        }

        if (paneHost is not null)
        {
            paneHost.Child = EnsureTabControl();
        }

        ApplyTexts();
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new DockRegionAutomationPeer(this);

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (!e.Handled &&
            e.Key == Key.Tab &&
            (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
            SelectNextPane(reverse: (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    /// <summary>
    /// Takes a pane out of the logical tree of the region so a floating window can host it.
    /// </summary>
    internal void ReleasePaneToWindow(DockPane pane)
    {
        if (pane.IsHostedInWindow)
        {
            return;
        }

        if (attachedPanes.Contains(pane))
        {
            RemoveLogicalChild(pane);
        }

        pane.IsHostedInWindow = true;
    }

    /// <summary>
    /// Takes a pane back into the logical tree of the region after its floating window let go of it.
    /// </summary>
    internal void AcceptPaneFromWindow(DockPane pane)
    {
        if (!pane.IsHostedInWindow)
        {
            return;
        }

        pane.IsHostedInWindow = false;
        if (attachedPanes.Contains(pane))
        {
            AddLogicalChild(pane);
        }
    }

    /// <summary>
    /// Rebuilds the docked (tab) list from <see cref="Panes"/> and the pane states, and keeps the active pane on a docked
    /// pane: the same pane when it is still docked, otherwise its neighbour at the same position.
    /// </summary>
    internal void RefreshDockedPanes(DockPane? activate = null)
    {
        var previousActive = ActivePane;
        var previousIndex = previousActive is null ? -1 : dockedPanes.IndexOf(previousActive);
        var desired = Panes.Where(x => x.State == DockPaneState.Docked).ToList();

        isSyncingPanes = true;
        try
        {
            SyncDockedPanes(desired);

            var next = activate is not null && desired.Contains(activate)
                ? activate
                : previousActive is not null && desired.Contains(previousActive)
                    ? previousActive
                    : desired.Count == 0
                        ? null
                        : desired[System.Math.Clamp(previousIndex, 0, desired.Count - 1)];
            SetCurrentValue(ActivePaneProperty, next);
        }
        finally
        {
            isSyncingPanes = false;
        }

        if (!ReferenceEquals(previousActive, ActivePane))
        {
            RaiseActivePaneChanged(previousActive, ActivePane);
        }

        SetValue(HasPanesPropertyKey, BooleanBoxes.Box(Panes.Count > 0));
        CoerceValue(VisibilityProperty);
        ApplyTexts();
        OwnerPanel?.InvalidateMeasure();
        CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>
    /// Sets the localized automation names and tool tips of the header buttons and the tabs.
    /// </summary>
    internal void ApplyTexts()
    {
        var header = ActivePane?.HeaderText ?? string.Empty;
        if (floatButton is not null)
        {
            var name = string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.Float1, header);
            AutomationProperties.SetName(floatButton, name);
            floatButton.ToolTip = name;
        }

        if (closeButton is not null)
        {
            var name = string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.Close1, header);
            AutomationProperties.SetName(closeButton, name);
            closeButton.ToolTip = name;
        }

        if (tabControl is null)
        {
            return;
        }

        foreach (var pane in dockedPanes)
        {
            if (tabControl.ItemContainerGenerator.ContainerFromItem(pane) is DockPaneTabItem tab)
            {
                tab.ApplyTexts();
            }
        }
    }

    private static void OnActivePaneChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var region = (DockRegion)d;
        if (region.isSyncingPanes)
        {
            return;
        }

        region.RaiseActivePaneChanged(e.OldValue as DockPane, e.NewValue as DockPane);
        region.ApplyTexts();
        CommandManager.InvalidateRequerySuggested();
    }

    private static object CoerceVisibility(
        DependencyObject d,
        object baseValue)
        => d is DockRegion { IsEmptyPaneGroup: true } region && DockPanelPro.GetDock(region) != DockPosition.Center
            ? Visibility.Collapsed
            : baseValue;

    private void RaiseActivePaneChanged(
        DockPane? oldValue,
        DockPane? newValue)
        => RaiseEvent(new RoutedPropertyChangedEventArgs<DockPane?>(oldValue, newValue, ActivePaneChangedEvent));

    private void SyncDockedPanes(List<DockPane> desired)
    {
        for (var i = dockedPanes.Count - 1; i >= 0; i--)
        {
            if (!desired.Contains(dockedPanes[i]))
            {
                dockedPanes.RemoveAt(i);
            }
        }

        for (var i = 0; i < desired.Count; i++)
        {
            if (i < dockedPanes.Count && ReferenceEquals(dockedPanes[i], desired[i]))
            {
                continue;
            }

            var existing = dockedPanes.IndexOf(desired[i]);
            if (existing >= 0)
            {
                dockedPanes.Move(existing, i);
            }
            else
            {
                dockedPanes.Insert(i, desired[i]);
            }
        }
    }

    private void OnPanesCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        foreach (var removed in attachedPanes.Where(x => !Panes.Contains(x)).ToList())
        {
            attachedPanes.Remove(removed);
            if (!removed.IsHostedInWindow)
            {
                RemoveLogicalChild(removed);
            }

            if (ReferenceEquals(removed.Region, this))
            {
                removed.Region = null;
            }
        }

        foreach (var added in Panes.Where(x => !attachedPanes.Contains(x)).ToList())
        {
            if (added.Region is not null && !ReferenceEquals(added.Region, this))
            {
                throw new InvalidOperationException("The pane already belongs to another region; use DockPanelPro.MovePane to move it.");
            }

            attachedPanes.Add(added);
            added.Region = this;
            if (!added.IsHostedInWindow)
            {
                AddLogicalChild(added);
            }
        }

        RefreshDockedPanes();
    }

    private DockPaneTabControl EnsureTabControl()
    {
        if (tabControl is not null)
        {
            return tabControl;
        }

        tabControl = new DockPaneTabControl
        {
            ItemsSource = dockedPanes,
        };
        tabControl.SetResourceReference(StyleProperty, "AtcApps.Styles.DockRegion.TabControl");
        tabControl.SetBinding(
            Selector.SelectedItemProperty,
            new Binding(nameof(ActivePane)) { Source = this, Mode = BindingMode.TwoWay });
        tabControl.SetBinding(
            TabControl.TabStripPlacementProperty,
            new Binding(nameof(TabStripPlacement)) { Source = this });
        return tabControl;
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        CultureManager.UiCultureChanged -= OnUiCultureChanged;
        CultureManager.UiCultureChanged += OnUiCultureChanged;
        ApplyTexts();
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
        => CultureManager.UiCultureChanged -= OnUiCultureChanged;

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
        => ApplyTexts();

    private void OnHeaderMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (ActivePane is null)
        {
            return;
        }

        if (e.ClickCount == 2)
        {
            headerDragStart = null;
            if (DockPanelProCommands.FloatPane.CanExecute(ActivePane, this))
            {
                DockPanelProCommands.FloatPane.Execute(ActivePane, this);
                e.Handled = true;
            }

            return;
        }

        headerDragStart = e.GetPosition(this);
    }

    private void OnHeaderMouseLeftButtonUp(
        object sender,
        MouseButtonEventArgs e)
        => headerDragStart = null;

    private void OnHeaderMouseMove(
        object sender,
        MouseEventArgs e)
    {
        if (headerDragStart is null ||
            e.LeftButton != MouseButtonState.Pressed ||
            ActivePane is null ||
            headerBorder is null)
        {
            headerDragStart = null;
            return;
        }

        var position = e.GetPosition(this);
        if (!DockPaneTabItem.IsDraggedOut(headerDragStart.Value, position, headerBorder.ActualHeight))
        {
            return;
        }

        headerDragStart = null;
        OwnerPanel?.FloatPaneByDrag(ActivePane);
    }
}