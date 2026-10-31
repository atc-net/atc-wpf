namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// An enhanced dock panel with resizable splitters, collapsible regions, tabbed panes, floating tool windows
/// and layout persistence. Perfect for building IDE-style interfaces with tool windows.
/// </summary>
public sealed partial class DockPanelPro : Panel
{
    private const double SplitterKeyboardStep = 10;

    private readonly List<Thumb> splitters = [];

    private DockRegion? topRegion;
    private DockRegion? bottomRegion;
    private DockRegion? leftRegion;
    private DockRegion? rightRegion;
    private DockRegion? centerRegion;

    private Thumb? topSplitter;
    private Thumb? bottomSplitter;
    private Thumb? leftSplitter;
    private Thumb? rightSplitter;

    private double topHeight;
    private double bottomHeight;
    private double leftWidth;
    private double rightWidth;

    /// <summary>
    /// Identifies the Dock attached property.
    /// </summary>
    public static readonly DependencyProperty DockProperty = DependencyProperty.RegisterAttached(
        "Dock",
        typeof(DockPosition),
        typeof(DockPanelPro),
        new FrameworkPropertyMetadata(
            DockPosition.Center,
            FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

    /// <summary>
    /// Gets or sets the layout identifier for persistence.
    /// </summary>
    [DependencyProperty]
    private string? layoutId;

    /// <summary>
    /// Gets or sets a value indicating whether the layout is saved automatically and restored when the panel is first loaded.
    /// Needs a <see cref="LayoutId"/>: every <see cref="LayoutChanged"/> saves the layout to the <see cref="LayoutStore"/>
    /// under that identifier, a pending change is saved when the owner window closes, and the first <c>Loaded</c>
    /// loads the stored layout.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool autoSaveLayout;

    /// <summary>
    /// Gets or sets where <see cref="AutoSaveLayout"/> saves and loads the layout;
    /// <see langword="null"/> uses <see cref="FileDockLayoutStore.Default"/>.
    /// </summary>
    [DependencyProperty]
    private IDockLayoutStore? layoutStore;

    /// <summary>
    /// Gets or sets the thickness of splitters between regions.
    /// </summary>
    [DependencyProperty(DefaultValue = 5.0)]
    private double splitterThickness;

    /// <summary>
    /// Gets or sets the brush used for splitter backgrounds.
    /// </summary>
    [DependencyProperty]
    private Brush? splitterBackground;

    /// <summary>
    /// Gets or sets a value indicating whether panes (<see cref="DockPane"/>) can be floated into their own tool windows.
    /// A pane can also opt out with <see cref="DockPane.CanFloat"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = false, PropertyChangedCallback = nameof(OnAllowFloatingChanged))]
    private bool allowFloating;

    static DockPanelPro()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(DockPanelPro),
            new FrameworkPropertyMetadata(typeof(DockPanelPro)));
        RegisterCommandBindings();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockPanelPro"/> class.
    /// </summary>
    public DockPanelPro()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        AddHandler(DockRegion.ExpandedEvent, new RoutedEventHandler(OnRegionLayoutEvent));
        AddHandler(DockRegion.CollapsedEvent, new RoutedEventHandler(OnRegionLayoutEvent));
        AddHandler(DockRegion.ActivePaneChangedEvent, new RoutedPropertyChangedEventHandler<DockPane?>(OnRegionActivePaneChanged));
    }

    /// <summary>
    /// Gets the dock position for the specified element.
    /// </summary>
    /// <param name="element">The element to query.</param>
    /// <returns>The dock position.</returns>
    public static DockPosition GetDock(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (DockPosition)element.GetValue(DockProperty);
    }

    /// <summary>
    /// Sets the dock position for the specified element.
    /// </summary>
    /// <param name="element">The element to modify.</param>
    /// <param name="value">The dock position to set.</param>
    public static void SetDock(
        UIElement element,
        DockPosition value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(DockProperty, value);
    }

    /// <summary>
    /// Saves the current layout to a JSON string: region sizes and expanded states, and (since layout version 2)
    /// the panes of each region in tab order, the active tab, and which panes are closed or floating, with the
    /// floating window bounds.
    /// </summary>
    /// <returns>A JSON string representing the current layout.</returns>
    public string SaveLayout()
    {
        var layout = new Internal.DockPanelLayout
        {
            Version = Internal.DockPanelLayout.CurrentVersion,
            LayoutId = LayoutId,
            Regions = [],
        };

        foreach (var child in InternalChildren)
        {
            if (child is not DockRegion region)
            {
                continue;
            }

            var dock = GetDock(region);
            var width = dock is DockPosition.Left or DockPosition.Right
                ? (dock == DockPosition.Left ? leftWidth : rightWidth)
                : region.ActualWidth;
            var height = dock is DockPosition.Top or DockPosition.Bottom
                ? (dock == DockPosition.Top ? topHeight : bottomHeight)
                : region.ActualHeight;

            layout.Regions.Add(new Internal.DockRegionState
            {
                RegionId = region.RegionId,
                Dock = dock,
                Width = width,
                Height = height,
                IsExpanded = region.IsExpanded,
                PaneIds = region.HasPanes
                    ? region.Panes.Where(x => !string.IsNullOrEmpty(x.PaneId)).Select(x => x.PaneId!).ToList()
                    : null,
                ActivePaneId = region.ActivePane?.PaneId,
            });
        }

        SavePaneStates(layout);

        return JsonSerializer.Serialize(layout, LayoutWriteOptions);
    }

    /// <summary>
    /// Loads layout from a JSON string.
    /// </summary>
    /// <remarks>
    /// Only the docking dimension of each side region is restored: the width of left and right
    /// regions and the height of top and bottom regions. The other dimension and the center
    /// region keep following the panel size, so a saved layout never pins them.
    /// A layout of version 2 also restores the panes of each region, the active tabs, closed panes and floating
    /// panes, whose window bounds are moved onto the visible screens. A layout without a version (version 1)
    /// leaves the panes as they are. Regions and panes that the layout does not know are left alone.
    /// </remarks>
    /// <param name="json">The JSON string containing the layout.</param>
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    public void LoadLayout(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return;
        }

        Internal.DockPanelLayout? layout;
        try
        {
            layout = JsonSerializer.Deserialize<Internal.DockPanelLayout>(json, LayoutReadOptions);
        }
        catch (JsonException)
        {
            return;
        }

        if (layout?.Regions is null)
        {
            return;
        }

        foreach (var regionState in layout.Regions)
        {
            var region = FindRegionById(regionState.RegionId);
            if (region is null)
            {
                continue;
            }

            var dock = GetDock(region);
            var hasWidth = !double.IsNaN(regionState.Width) && regionState.Width > 0;
            var hasHeight = !double.IsNaN(regionState.Height) && regionState.Height > 0;

            switch (dock)
            {
                case DockPosition.Left when hasWidth:
                    region.Width = regionState.Width;
                    leftWidth = regionState.Width;
                    break;
                case DockPosition.Right when hasWidth:
                    region.Width = regionState.Width;
                    rightWidth = regionState.Width;
                    break;
                case DockPosition.Top when hasHeight:
                    region.Height = regionState.Height;
                    topHeight = regionState.Height;
                    break;
                case DockPosition.Bottom when hasHeight:
                    region.Height = regionState.Height;
                    bottomHeight = regionState.Height;
                    break;
            }

            region.IsExpanded = regionState.IsExpanded;
        }

        LoadPaneStates(layout);

        InvalidateMeasure();
        InvalidateArrange();
    }

    /// <summary>
    /// Resets the layout to default values: region sizes, expanded states, and all panes docked
    /// (floating panes are docked back, closed panes restored).
    /// </summary>
    public void ResetLayout()
    {
        foreach (var pane in GetPanes().Where(x => x.State != DockPaneState.Docked))
        {
            CloseFloatingWindow(pane);
            ChangePaneState(pane, DockPaneState.Docked);
        }

        topHeight = 0;
        bottomHeight = 0;
        leftWidth = 0;
        rightWidth = 0;

        foreach (var child in InternalChildren)
        {
            if (child is DockRegion region)
            {
                region.Width = double.NaN;
                region.Height = double.NaN;
                region.IsExpanded = true;
            }
        }

        InvalidateMeasure();
        InvalidateArrange();
        NotifyLayoutChanged();
    }

    /// <inheritdoc/>
    protected override int VisualChildrenCount
        => InternalChildren.Count + splitters.Count;

    /// <inheritdoc/>
    protected override Visual GetVisualChild(int index)
    {
        if (index < InternalChildren.Count)
        {
            return InternalChildren[index];
        }

        var splitterIndex = index - InternalChildren.Count;
        if (splitterIndex < splitters.Count)
        {
            return splitters[splitterIndex];
        }

        throw new ArgumentOutOfRangeException(nameof(index));
    }

    /// <inheritdoc/>
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    protected override Size MeasureOverride(Size availableSize)
    {
        CategorizeChildren();
        EnsureSplitters();

        var splitterSize = SplitterThickness;

        // Handle infinite available size (e.g., inside ScrollViewer)
        var measureWidth = double.IsPositiveInfinity(availableSize.Width) ? 800 : availableSize.Width;
        var measureHeight = double.IsPositiveInfinity(availableSize.Height) ? 600 : availableSize.Height;

        // Calculate available space after splitters
        var usedWidth = 0.0;
        var usedHeight = 0.0;
        var desiredWidth = 0.0;
        var desiredHeight = 0.0;

        // Measure top region
        if (topRegion is not null && IsShown(topRegion))
        {
            var height = GetRegionHeight(topRegion, ref topHeight, 100);
            height = ClampHeight(height, topRegion);
            topRegion.Measure(new Size(measureWidth, height));
            usedHeight += height + splitterSize;
            desiredHeight += height + splitterSize;
        }

        // Measure bottom region
        if (IsShown(bottomRegion))
        {
            var height = GetRegionHeight(bottomRegion, ref bottomHeight, 100);
            height = ClampHeight(height, bottomRegion);
            bottomRegion.Measure(new Size(measureWidth, height));
            usedHeight += height + splitterSize;
            desiredHeight += height + splitterSize;
        }

        var centerHeight = System.Math.Max(0, measureHeight - usedHeight);

        // Measure left region
        if (IsShown(leftRegion))
        {
            var width = GetRegionWidth(leftRegion, ref leftWidth, 200);
            width = ClampWidth(width, leftRegion);
            leftRegion.Measure(new Size(width, centerHeight));
            usedWidth += width + splitterSize;
            desiredWidth += width + splitterSize;
        }

        // Measure right region
        if (IsShown(rightRegion))
        {
            var width = GetRegionWidth(rightRegion, ref rightWidth, 200);
            width = ClampWidth(width, rightRegion);
            rightRegion.Measure(new Size(width, centerHeight));
            usedWidth += width + splitterSize;
            desiredWidth += width + splitterSize;
        }

        // Measure center region
        var centerWidth = System.Math.Max(0, measureWidth - usedWidth);
        centerRegion?.Measure(new Size(centerWidth, centerHeight));

        // Add center region's desired size
        if (centerRegion is not null)
        {
            desiredWidth += centerRegion.DesiredSize.Width;
            desiredHeight += centerRegion.DesiredSize.Height;
        }
        else
        {
            // Default minimum center size
            desiredWidth += 100;
            desiredHeight += 100;
        }

        // Measure splitters
        foreach (var splitter in splitters)
        {
            splitter.Measure(new Size(measureWidth, measureHeight));
        }

        // Return calculated desired size, constrained by available size
        var resultWidth = double.IsPositiveInfinity(availableSize.Width)
            ? desiredWidth
            : availableSize.Width;
        var resultHeight = double.IsPositiveInfinity(availableSize.Height)
            ? desiredHeight
            : availableSize.Height;

        return new Size(resultWidth, resultHeight);
    }

    /// <inheritdoc/>
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    protected override Size ArrangeOverride(Size finalSize)
    {
        var splitterSize = SplitterThickness;
        var top = 0.0;
        var bottom = finalSize.Height;
        var left = 0.0;
        var right = finalSize.Width;

        // Arrange top region and splitter
        if (topRegion is not null && IsShown(topRegion))
        {
            var height = ClampHeight(topHeight, topRegion);
            topRegion.Arrange(new Rect(0, top, finalSize.Width, height));
            top += height;

            if (topSplitter is not null)
            {
                topSplitter.Arrange(new Rect(0, top, finalSize.Width, splitterSize));
                top += splitterSize;
            }
        }

        // Arrange bottom region and splitter
        if (IsShown(bottomRegion))
        {
            var height = ClampHeight(bottomHeight, bottomRegion);
            bottom -= height;
            bottomRegion.Arrange(new Rect(0, bottom, finalSize.Width, height));

            if (bottomSplitter is not null)
            {
                bottom -= splitterSize;
                bottomSplitter.Arrange(new Rect(0, bottom, finalSize.Width, splitterSize));
            }
        }

        var centerHeight = System.Math.Max(0, bottom - top);

        // Arrange left region and splitter
        if (IsShown(leftRegion))
        {
            var width = ClampWidth(leftWidth, leftRegion);
            leftRegion.Arrange(new Rect(left, top, width, centerHeight));
            left += width;

            if (leftSplitter is not null)
            {
                leftSplitter.Arrange(new Rect(left, top, splitterSize, centerHeight));
                left += splitterSize;
            }
        }

        // Arrange right region and splitter
        if (IsShown(rightRegion))
        {
            var width = ClampWidth(rightWidth, rightRegion);
            right -= width;
            rightRegion.Arrange(new Rect(right, top, width, centerHeight));

            if (rightSplitter is not null)
            {
                right -= splitterSize;
                rightSplitter.Arrange(new Rect(right, top, splitterSize, centerHeight));
            }
        }

        // Arrange center region
        var centerWidth = System.Math.Max(0, right - left);
        centerRegion?.Arrange(new Rect(left, top, centerWidth, centerHeight));

        // Hide unused splitters
        HideUnusedSplitters();

        return finalSize;
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new DockPanelProAutomationPeer(this);

    private static bool IsShown([NotNullWhen(true)] DockRegion? region)
        => region is not null && region.IsExpanded && !region.IsEmptyPaneGroup;

    private static void OnAllowFloatingChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => CommandManager.InvalidateRequerySuggested();

    private DockRegion? FindRegionById(string? regionId)
    {
        if (string.IsNullOrEmpty(regionId))
        {
            return null;
        }

        foreach (var child in InternalChildren)
        {
            if (child is DockRegion region && region.RegionId == regionId)
            {
                return region;
            }
        }

        return null;
    }

    [SuppressMessage("Design", "CA1508: 'x' is always 'null'. Remove or refactor the condition(s) to avoid dead code.", Justification = "OK.")]
    [SuppressMessage("Design", "S3458: Remove this empty 'case' clause", Justification = "OK.")]
    private void CategorizeChildren()
    {
        topRegion = null;
        bottomRegion = null;
        leftRegion = null;
        rightRegion = null;
        centerRegion = null;

        foreach (var child in InternalChildren)
        {
            if (child is not DockRegion region)
            {
                continue;
            }

            var dock = GetDock(region);
            switch (dock)
            {
                case DockPosition.Top:
                    topRegion ??= region;
                    break;
                case DockPosition.Bottom:
                    bottomRegion ??= region;
                    break;
                case DockPosition.Left:
                    leftRegion ??= region;
                    break;
                case DockPosition.Right:
                    rightRegion ??= region;
                    break;
                case DockPosition.Center:
                default:
                    centerRegion ??= region;
                    break;
            }
        }
    }

    private void EnsureSplitters()
    {
        EnsureSplitter(ref topSplitter, topRegion, isHorizontal: true);
        EnsureSplitter(ref bottomSplitter, bottomRegion, isHorizontal: true);
        EnsureSplitter(ref leftSplitter, leftRegion, isHorizontal: false);
        EnsureSplitter(ref rightSplitter, rightRegion, isHorizontal: false);
    }

    private void EnsureSplitter(
        ref Thumb? splitter,
        DockRegion? region,
        bool isHorizontal)
    {
        if (!IsShown(region) || !region.IsResizable)
        {
            if (splitter is not null)
            {
                RemoveSplitter(splitter);
                splitter = null;
            }

            return;
        }

        if (splitter is not null)
        {
            return;
        }

        splitter = CreateSplitter(isHorizontal, region);
        splitters.Add(splitter);
        AddVisualChild(splitter);
    }

    private Thumb CreateSplitter(
        bool isHorizontal,
        DockRegion region)
    {
        var splitter = new Thumb
        {
            Cursor = isHorizontal ? Cursors.SizeNS : Cursors.SizeWE,
            Background = SplitterBackground ?? Brushes.Transparent,
            Opacity = 0.01, // Nearly invisible but still hit-testable
            Tag = region,
            Focusable = true,
            IsTabStop = true,
        };

        AutomationProperties.SetAutomationId(splitter, "Splitter_" + (region.RegionId ?? string.Empty));
        ApplySplitterText(splitter);

        splitter.DragDelta += OnSplitterDragDelta;
        splitter.DragCompleted += OnSplitterDragCompleted;
        splitter.KeyDown += OnSplitterKeyDown;
        splitter.IsKeyboardFocusedChanged += OnSplitterKeyboardFocusChanged;

        return splitter;
    }

    /// <summary>
    /// Names a splitter "Resize <i>region</i>" for UI Automation and as its tool tip.
    /// </summary>
    private static void ApplySplitterText(Thumb splitter)
    {
        if (splitter.Tag is not DockRegion region)
        {
            return;
        }

        var regionName = region.Header as string
                         ?? region.ActivePane?.HeaderText
                         ?? region.RegionId
                         ?? string.Empty;
        var text = string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.Resize1, regionName).Trim();
        AutomationProperties.SetName(splitter, text);
    }

    private void ApplySplitterTexts()
    {
        foreach (var splitter in splitters)
        {
            ApplySplitterText(splitter);
        }
    }

    /// <summary>
    /// Arrow keys on a focused splitter move it by <see cref="SplitterKeyboardStep"/> (mirrored in right-to-left).
    /// </summary>
    private void OnSplitterKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (sender is not Thumb splitter ||
            Keyboard.Modifiers != ModifierKeys.None)
        {
            return;
        }

        var isHorizontal = ReferenceEquals(splitter, topSplitter) || ReferenceEquals(splitter, bottomSplitter);
        var (horizontal, vertical) = FlowDirectionKeyHelper.ToLayoutKey(e.Key, FlowDirection) switch
        {
            Key.Up when isHorizontal => (0d, -SplitterKeyboardStep),
            Key.Down when isHorizontal => (0d, SplitterKeyboardStep),
            Key.Left when !isHorizontal => (-SplitterKeyboardStep, 0d),
            Key.Right when !isHorizontal => (SplitterKeyboardStep, 0d),
            _ => (0d, 0d),
        };

        if (horizontal == 0 && vertical == 0)
        {
            return;
        }

        e.Handled = true;
        MoveSplitter(splitter, horizontal, vertical);
        OnSplitterDragCompleted(splitter, new DragCompletedEventArgs(horizontal, vertical, canceled: false));
    }

    /// <summary>
    /// Shows a keyboard-focused splitter (it is nearly transparent otherwise).
    /// </summary>
    private void OnSplitterKeyboardFocusChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
    {
        if (sender is not Thumb splitter)
        {
            return;
        }

        if (splitter.IsKeyboardFocused)
        {
            splitter.Opacity = 1;
            if (SplitterBackground is null)
            {
                splitter.SetResourceReference(Control.BackgroundProperty, "AtcApps.Brushes.Accent");
            }
        }
        else
        {
            splitter.Opacity = 0.01;
            splitter.Background = SplitterBackground ?? Brushes.Transparent;
        }
    }

    private void RemoveSplitter(Thumb splitter)
    {
        splitter.DragDelta -= OnSplitterDragDelta;
        splitter.DragCompleted -= OnSplitterDragCompleted;
        splitter.KeyDown -= OnSplitterKeyDown;
        splitter.IsKeyboardFocusedChanged -= OnSplitterKeyboardFocusChanged;
        splitters.Remove(splitter);
        RemoveVisualChild(splitter);
    }

    private void HideUnusedSplitters()
    {
        if (topSplitter is not null && !IsShown(topRegion))
        {
            topSplitter.Arrange(new Rect(0, 0, 0, 0));
        }

        if (bottomSplitter is not null && !IsShown(bottomRegion))
        {
            bottomSplitter.Arrange(new Rect(0, 0, 0, 0));
        }

        if (leftSplitter is not null && !IsShown(leftRegion))
        {
            leftSplitter.Arrange(new Rect(0, 0, 0, 0));
        }

        if (rightSplitter is not null && !IsShown(rightRegion))
        {
            rightSplitter.Arrange(new Rect(0, 0, 0, 0));
        }
    }

    private void OnSplitterDragDelta(
        object sender,
        DragDeltaEventArgs e)
    {
        if (sender is Thumb splitter)
        {
            MoveSplitter(splitter, e.HorizontalChange, e.VerticalChange);
        }
    }

    private void MoveSplitter(
        Thumb splitter,
        double horizontalChange,
        double verticalChange)
    {
        if (splitter == topSplitter && topRegion is not null)
        {
            topHeight = ClampHeight(topHeight + verticalChange, topRegion);
            KeepExplicitSize(topRegion, HeightProperty, topHeight);
        }
        else if (splitter == bottomSplitter && bottomRegion is not null)
        {
            bottomHeight = ClampHeight(bottomHeight - verticalChange, bottomRegion);
            KeepExplicitSize(bottomRegion, HeightProperty, bottomHeight);
        }
        else if (splitter == leftSplitter && leftRegion is not null)
        {
            leftWidth = ClampWidth(leftWidth + horizontalChange, leftRegion);
            KeepExplicitSize(leftRegion, WidthProperty, leftWidth);
        }
        else if (splitter == rightSplitter && rightRegion is not null)
        {
            rightWidth = ClampWidth(rightWidth - horizontalChange, rightRegion);
            KeepExplicitSize(rightRegion, WidthProperty, rightWidth);
        }

        InvalidateMeasure();
    }

    /// <summary>
    /// A region with an explicit <c>Width</c>/<c>Height</c> (its initial size) would keep that size in a
    /// larger or smaller slot: move the explicit size along with the splitter.
    /// </summary>
    private static void KeepExplicitSize(
        DockRegion region,
        DependencyProperty property,
        double size)
    {
        if (!double.IsNaN((double)region.GetValue(property)))
        {
            region.SetCurrentValue(property, size);
        }
    }

    private void OnSplitterDragCompleted(
        object sender,
        DragCompletedEventArgs e)
    {
        // Notify regions that their size may have changed
        foreach (var child in InternalChildren)
        {
            if (child is DockRegion region)
            {
                region.RaiseRegionSizeChanged();
            }
        }

        NotifyLayoutChanged();
    }

    private static double GetRegionWidth(
        DockRegion region,
        ref double storedWidth,
        double defaultWidth)
    {
        if (storedWidth > 0)
        {
            return storedWidth;
        }

        storedWidth = !double.IsNaN(region.Width) && region.Width > 0
            ? region.Width
            : defaultWidth;

        return storedWidth;
    }

    private static double GetRegionHeight(
        DockRegion region,
        ref double storedHeight,
        double defaultHeight)
    {
        if (storedHeight > 0)
        {
            return storedHeight;
        }

        storedHeight = !double.IsNaN(region.Height) && region.Height > 0
            ? region.Height
            : defaultHeight;

        return storedHeight;
    }

    private static double ClampWidth(
        double width,
        DockRegion region)
    {
        var min = region.MinWidth;
        var max = double.IsPositiveInfinity(region.MaxWidth) ? double.MaxValue : region.MaxWidth;
        return System.Math.Max(min, System.Math.Min(max, width));
    }

    private static double ClampHeight(
        double height,
        DockRegion region)
    {
        var min = region.MinHeight;
        var max = double.IsPositiveInfinity(region.MaxHeight) ? double.MaxValue : region.MaxHeight;
        return System.Math.Max(min, System.Math.Min(max, height));
    }
}