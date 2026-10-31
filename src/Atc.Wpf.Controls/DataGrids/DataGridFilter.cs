namespace Atc.Wpf.Controls.DataGrids;

/// <summary>
/// Opt-in, Excel-style filtering for the stock WPF <see cref="DataGrid"/>: a filter button in each column header with
/// a pop-up of the column's distinct values (filter box, Select all, "(Blanks)"), a text filter and a number/date
/// range filter; a debounced search across the visible columns; a column chooser on the header context menu; and
/// saving and restoring the column layout and filters.
/// </summary>
/// <remarks>
/// <para>
/// Filters are applied to the grid's default collection view (<see cref="CollectionViewSource.GetDefaultView"/>
/// of <see cref="ItemsControl.ItemsSource"/>). A filter the consumer already set on that view is kept and ANDed;
/// if the consumer replaces the view filter later, it is adopted (and ANDed) on the next filter change.
/// Grids that share one <c>ItemsSource</c> share its default view: their filters combine (AND), so each grid shows
/// the rows that pass the filters of all of them.
/// </para>
/// <para>
/// Columns are identified by <see cref="ResolveColumnKey"/>: the attached <c>ColumnKey</c>, else the
/// <see cref="DataGridColumn.SortMemberPath"/>, else the binding path, else the header text.
/// </para>
/// </remarks>
public static partial class DataGridFilter
{
    private static readonly TimeSpan DefaultSearchDelay = TimeSpan.FromMilliseconds(300);

    /// <summary>
    /// Identifies the <c>IsEnabled</c> attached property: <see langword="true"/> on a <see cref="DataGrid"/> adds a
    /// filter button to each column header (Alt+Down on the button or on a cell opens the column's filter).
    /// </summary>
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled",
        typeof(bool),
        typeof(DataGridFilter),
        new PropertyMetadata(BooleanBoxes.FalseBox, OnIsEnabledChanged));

    /// <summary>
    /// Identifies the <c>ShowColumnChooser</c> attached property: <see langword="true"/> on a <see cref="DataGrid"/>
    /// shows a menu of the columns with check boxes when a column header is right-clicked.
    /// </summary>
    public static readonly DependencyProperty ShowColumnChooserProperty = DependencyProperty.RegisterAttached(
        "ShowColumnChooser",
        typeof(bool),
        typeof(DataGridFilter),
        new PropertyMetadata(BooleanBoxes.FalseBox, OnShowColumnChooserChanged));

    /// <summary>
    /// Identifies the <c>SearchText</c> attached property: the text a <see cref="DataGrid"/> searches for in all
    /// visible columns (contains, case-insensitive), applied after <c>SearchDelay</c> and ANDed with the column filters.
    /// Bind it to a search box's <c>Text</c>.
    /// </summary>
    public static readonly DependencyProperty SearchTextProperty = DependencyProperty.RegisterAttached(
        "SearchText",
        typeof(string),
        typeof(DataGridFilter),
        new PropertyMetadata(string.Empty, OnSearchTextChanged));

    /// <summary>
    /// Identifies the <c>SearchDelay</c> attached property: how long a <see cref="DataGrid"/> waits after the last
    /// change of <c>SearchText</c> before it searches (default 300 ms; zero searches at once).
    /// </summary>
    public static readonly DependencyProperty SearchDelayProperty = DependencyProperty.RegisterAttached(
        "SearchDelay",
        typeof(TimeSpan),
        typeof(DataGridFilter),
        new PropertyMetadata(DefaultSearchDelay));

    /// <summary>
    /// Identifies the <c>IsColumnFilterEnabled</c> attached property: <see langword="false"/> on a column removes its
    /// filter button.
    /// </summary>
    public static readonly DependencyProperty IsColumnFilterEnabledProperty = DependencyProperty.RegisterAttached(
        "IsColumnFilterEnabled",
        typeof(bool),
        typeof(DataGridFilter),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Identifies the <c>ColumnKey</c> attached property: a stable key of a column for filters and saved layouts
    /// (see <see cref="ResolveColumnKey"/>).
    /// </summary>
    public static readonly DependencyProperty ColumnKeyProperty = DependencyProperty.RegisterAttached(
        "ColumnKey",
        typeof(string),
        typeof(DataGridFilter),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <c>CanUserHideColumn</c> attached property: <see langword="false"/> on a column keeps it from
    /// being hidden in the column chooser (e.g. the key column).
    /// </summary>
    public static readonly DependencyProperty CanUserHideColumnProperty = DependencyProperty.RegisterAttached(
        "CanUserHideColumn",
        typeof(bool),
        typeof(DataGridFilter),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Identifies the read-only <c>IsColumnFilterActive</c> attached property key.
    /// </summary>
    internal static readonly DependencyPropertyKey IsColumnFilterActivePropertyKey = DependencyProperty.RegisterAttachedReadOnly(
        "IsColumnFilterActive",
        typeof(bool),
        typeof(DataGridFilter),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Identifies the read-only <c>IsColumnFilterActive</c> attached property: <see langword="true"/> on a column
    /// while it has an active filter (for header styles).
    /// </summary>
    public static readonly DependencyProperty IsColumnFilterActiveProperty = IsColumnFilterActivePropertyKey.DependencyProperty;

    /// <summary>
    /// Identifies the <c>FilterChanged</c> attached routed event, raised on the <see cref="DataGrid"/> (bubbling)
    /// after the column filters or the search were applied, once per change.
    /// </summary>
    public static readonly RoutedEvent FilterChangedEvent = EventManager.RegisterRoutedEvent(
        "FilterChanged",
        RoutingStrategy.Bubble,
        typeof(DataGridFilterChangedEventHandler),
        typeof(DataGridFilter));

    /// <summary>
    /// Identifies the internal <c>IsColumnFilterSupported</c> attached property: <see langword="false"/> on a column
    /// whose grid source cannot apply its filter (the filter button is disabled).
    /// </summary>
    internal static readonly DependencyProperty IsColumnFilterSupportedProperty = DependencyProperty.RegisterAttached(
        "IsColumnFilterSupported",
        typeof(bool),
        typeof(DataGridFilter),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    private static readonly DependencyProperty StateProperty = DependencyProperty.RegisterAttached(
        "State",
        typeof(DataGridFilterState),
        typeof(DataGridFilter),
        new PropertyMetadata(defaultValue: null));

    private static readonly DependencyProperty ObservedItemsSourceProperty = DependencyProperty.RegisterAttached(
        "ObservedItemsSource",
        typeof(object),
        typeof(DataGridFilter),
        new PropertyMetadata(defaultValue: null, OnObservedItemsSourceChanged));

    private static readonly DependencyProperty OriginalHeaderTemplateProperty = DependencyProperty.RegisterAttached(
        "OriginalHeaderTemplate",
        typeof(DataTemplate),
        typeof(DataGridFilter),
        new PropertyMetadata(defaultValue: null));

    private static readonly DependencyProperty WrapperHeaderTemplateProperty = DependencyProperty.RegisterAttached(
        "WrapperHeaderTemplate",
        typeof(DataTemplate),
        typeof(DataGridFilter),
        new PropertyMetadata(defaultValue: null));

    /// <summary>Gets whether the column filter buttons are shown on the grid.</summary>
    /// <param name="element">The grid.</param>
    /// <returns>The value.</returns>
    public static bool GetIsEnabled(DependencyObject element)
        => (bool)(element ?? throw new ArgumentNullException(nameof(element))).GetValue(IsEnabledProperty);

    /// <summary>Sets whether the column filter buttons are shown on the grid.</summary>
    /// <param name="element">The grid.</param>
    /// <param name="value">The value.</param>
    public static void SetIsEnabled(
        DependencyObject element,
        bool value)
        => (element ?? throw new ArgumentNullException(nameof(element))).SetValue(IsEnabledProperty, BooleanBoxes.Box(value));

    /// <summary>Gets whether right-clicking a column header shows the column chooser.</summary>
    /// <param name="element">The grid.</param>
    /// <returns>The value.</returns>
    public static bool GetShowColumnChooser(DependencyObject element)
        => (bool)(element ?? throw new ArgumentNullException(nameof(element))).GetValue(ShowColumnChooserProperty);

    /// <summary>Sets whether right-clicking a column header shows the column chooser.</summary>
    /// <param name="element">The grid.</param>
    /// <param name="value">The value.</param>
    public static void SetShowColumnChooser(
        DependencyObject element,
        bool value)
        => (element ?? throw new ArgumentNullException(nameof(element))).SetValue(ShowColumnChooserProperty, BooleanBoxes.Box(value));

    /// <summary>Gets the search text of the grid.</summary>
    /// <param name="element">The grid.</param>
    /// <returns>The value.</returns>
    public static string GetSearchText(DependencyObject element)
        => (string)(element ?? throw new ArgumentNullException(nameof(element))).GetValue(SearchTextProperty);

    /// <summary>Sets the search text of the grid.</summary>
    /// <param name="element">The grid.</param>
    /// <param name="value">The value.</param>
    public static void SetSearchText(
        DependencyObject element,
        string value)
        => (element ?? throw new ArgumentNullException(nameof(element))).SetValue(SearchTextProperty, value);

    /// <summary>Gets how long the grid waits before it searches.</summary>
    /// <param name="element">The grid.</param>
    /// <returns>The value.</returns>
    public static TimeSpan GetSearchDelay(DependencyObject element)
        => (TimeSpan)(element ?? throw new ArgumentNullException(nameof(element))).GetValue(SearchDelayProperty);

    /// <summary>Sets how long the grid waits before it searches.</summary>
    /// <param name="element">The grid.</param>
    /// <param name="value">The value.</param>
    public static void SetSearchDelay(
        DependencyObject element,
        TimeSpan value)
        => (element ?? throw new ArgumentNullException(nameof(element))).SetValue(SearchDelayProperty, value);

    /// <summary>Gets whether the column has a filter button.</summary>
    /// <param name="element">The column.</param>
    /// <returns>The value.</returns>
    public static bool GetIsColumnFilterEnabled(DependencyObject element)
        => (bool)(element ?? throw new ArgumentNullException(nameof(element))).GetValue(IsColumnFilterEnabledProperty);

    /// <summary>Sets whether the column has a filter button.</summary>
    /// <param name="element">The column.</param>
    /// <param name="value">The value.</param>
    public static void SetIsColumnFilterEnabled(
        DependencyObject element,
        bool value)
        => (element ?? throw new ArgumentNullException(nameof(element))).SetValue(IsColumnFilterEnabledProperty, BooleanBoxes.Box(value));

    /// <summary>Gets the stable key of the column (the attached value only; see <see cref="ResolveColumnKey"/>).</summary>
    /// <param name="element">The column.</param>
    /// <returns>The value.</returns>
    public static string? GetColumnKey(DependencyObject element)
        => (string?)(element ?? throw new ArgumentNullException(nameof(element))).GetValue(ColumnKeyProperty);

    /// <summary>Sets the stable key of the column.</summary>
    /// <param name="element">The column.</param>
    /// <param name="value">The value.</param>
    public static void SetColumnKey(
        DependencyObject element,
        string? value)
        => (element ?? throw new ArgumentNullException(nameof(element))).SetValue(ColumnKeyProperty, value);

    /// <summary>Gets whether the column chooser may hide the column.</summary>
    /// <param name="element">The column.</param>
    /// <returns>The value.</returns>
    public static bool GetCanUserHideColumn(DependencyObject element)
        => (bool)(element ?? throw new ArgumentNullException(nameof(element))).GetValue(CanUserHideColumnProperty);

    /// <summary>Sets whether the column chooser may hide the column.</summary>
    /// <param name="element">The column.</param>
    /// <param name="value">The value.</param>
    public static void SetCanUserHideColumn(
        DependencyObject element,
        bool value)
        => (element ?? throw new ArgumentNullException(nameof(element))).SetValue(CanUserHideColumnProperty, BooleanBoxes.Box(value));

    /// <summary>Gets whether the column has an active filter.</summary>
    /// <param name="element">The column.</param>
    /// <returns>The value.</returns>
    public static bool GetIsColumnFilterActive(DependencyObject element)
        => (bool)(element ?? throw new ArgumentNullException(nameof(element))).GetValue(IsColumnFilterActiveProperty);

    /// <summary>Adds a handler for the <c>FilterChanged</c> attached event.</summary>
    /// <param name="element">The grid (or an ancestor).</param>
    /// <param name="handler">The handler.</param>
    public static void AddFilterChangedHandler(
        DependencyObject element,
        DataGridFilterChangedEventHandler handler)
    {
        if (element is UIElement uiElement)
        {
            uiElement.AddHandler(FilterChangedEvent, handler);
        }
    }

    /// <summary>Removes a handler for the <c>FilterChanged</c> attached event.</summary>
    /// <param name="element">The grid (or an ancestor).</param>
    /// <param name="handler">The handler.</param>
    public static void RemoveFilterChangedHandler(
        DependencyObject element,
        DataGridFilterChangedEventHandler handler)
    {
        if (element is UIElement uiElement)
        {
            uiElement.RemoveHandler(FilterChangedEvent, handler);
        }
    }

    /// <summary>
    /// Returns the stable key of a column: the attached <c>ColumnKey</c>, else the
    /// <see cref="DataGridColumn.SortMemberPath"/>, else the binding path, else the header text.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <returns>The key (empty when the column has none of them).</returns>
    public static string ResolveColumnKey(DataGridColumn column)
    {
        ArgumentNullException.ThrowIfNull(column);

        if (GetColumnKey(column) is { Length: > 0 } key)
        {
            return key;
        }

        if (!string.IsNullOrEmpty(column.SortMemberPath))
        {
            return column.SortMemberPath;
        }

        if (column is DataGridBoundColumn { Binding: Binding { Path.Path: { Length: > 0 } path } })
        {
            return path;
        }

        return column.Header switch
        {
            string text => text,
            null => string.Empty,
            var header => header.ToString() ?? string.Empty,
        };
    }

    /// <summary>
    /// Returns the active column filters of the grid.
    /// </summary>
    /// <param name="grid">The grid.</param>
    /// <returns>The filters (empty when none).</returns>
    public static IReadOnlyList<DataGridColumnFilter> GetColumnFilters(
        DataGrid grid)
    {
        ArgumentNullException.ThrowIfNull(grid);
        return GetState(grid)?.GetFilters() ?? [];
    }

    /// <summary>
    /// Sets the filter of the column <see cref="DataGridColumnFilter.ColumnKey"/> names and applies it
    /// (one refresh, one <c>FilterChanged</c>). A filter that restricts nothing removes the column's filter.
    /// </summary>
    /// <param name="grid">The grid.</param>
    /// <param name="filter">The filter.</param>
    public static void SetColumnFilter(
        DataGrid grid,
        DataGridColumnFilter filter)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(filter);
        EnsureState(grid).SetFilter(filter.ColumnKey, filter);
    }

    /// <summary>
    /// Removes the filter of a column and applies (nothing happens when the column has no filter).
    /// </summary>
    /// <param name="grid">The grid.</param>
    /// <param name="column">The column.</param>
    public static void ClearColumnFilter(
        DataGrid grid,
        DataGridColumn column)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(column);
        GetState(grid)?.SetFilter(ResolveColumnKey(column), filter: null);
    }

    /// <summary>
    /// Removes every column filter and applies once. The search text and the consumer's own view filter stay.
    /// </summary>
    /// <param name="grid">The grid.</param>
    public static void ClearAllFilters(DataGrid grid)
    {
        ArgumentNullException.ThrowIfNull(grid);
        GetState(grid)?.ClearFilters();
    }

    /// <summary>
    /// Opens the filter pop-up of a column (below its header button when the header is shown).
    /// </summary>
    /// <param name="grid">The grid.</param>
    /// <param name="column">The column.</param>
    public static void OpenColumnFilter(
        DataGrid grid,
        DataGridColumn column)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(column);

        if (!CanFilterColumn(grid, column))
        {
            return;
        }

        EnsureState(grid).OpenPopup(column, FindFilterButton(grid, column));
    }

    /// <summary>
    /// Opens the column chooser (a menu of the columns with check boxes) at the grid's top-left corner, e.g. from a
    /// toolbar button. Columns with <c>CanUserHideColumn="False"</c> cannot be unticked, nor can the last shown one.
    /// </summary>
    /// <param name="grid">The grid.</param>
    public static void OpenColumnChooser(DataGrid grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        var menu = CreateColumnChooserMenu(grid);
        menu.PlacementTarget = grid;
        menu.Placement = PlacementMode.Relative;
        menu.IsOpen = true;
    }

    /// <summary>
    /// Returns whether a column can be filtered: the filter is enabled for it, its values can be read
    /// (see <see cref="CanReadColumn"/>) and the grid's source can apply its filter.
    /// </summary>
    internal static bool CanFilterColumn(
        DataGrid grid,
        DataGridColumn column)
        => CanReadColumn(grid, column) &&
           GetState(grid)?.IsColumnSupported(column) != false;

    /// <summary>
    /// Returns whether a column gets a filter button: the filter is enabled for it and its values can be read
    /// (a binding path, or a <see cref="DataGridColumn.SortMemberPath"/> for template columns).
    /// </summary>
    internal static bool CanReadColumn(
        DataGrid grid,
        DataGridColumn column)
        => GetIsColumnFilterEnabled(column) &&
           DataGridColumnValueAccessor.Create(column, GetGridCulture(grid)) is not null;

    /// <summary>
    /// Sets whether the grid's source can apply the filter of a column.
    /// </summary>
    internal static void SetIsColumnFilterSupported(
        DataGridColumn column,
        bool value)
    {
        if ((bool)column.GetValue(IsColumnFilterSupportedProperty) != value)
        {
            column.SetValue(IsColumnFilterSupportedProperty, BooleanBoxes.Box(value));
        }
    }

    /// <summary>
    /// Returns the culture the grid's cells format their values in (its <see cref="FrameworkElement.Language"/>).
    /// </summary>
    internal static CultureInfo GetGridCulture(DataGrid grid)
    {
        try
        {
            return grid.Language.GetSpecificCulture();
        }
        catch (InvalidOperationException)
        {
            return CultureInfo.CurrentCulture;
        }
    }

    /// <summary>
    /// Returns the text of a column header for names and menus.
    /// </summary>
    internal static string GetColumnTitle(DataGridColumn column)
        => column.Header switch
        {
            string text => text,
            TextBlock textBlock => textBlock.Text,
            ContentControl { Content: string content } => content,
            _ => ResolveColumnKey(column),
        };

    /// <summary>
    /// Returns the distinct values the filter pop-up of a column offers.
    /// </summary>
    internal static IReadOnlyList<DataGridFilterValue> GetDistinctValues(
        DataGrid grid,
        DataGridColumn column)
        => EnsureState(grid).GetDistinctValues(column, out _);

    /// <summary>
    /// Returns the content of the open filter pop-up of a column, or <see langword="null"/>.
    /// </summary>
    internal static DataGridFilterPopupContent? GetOpenPopupContent(
        DataGrid grid,
        DataGridColumn? column = null)
    {
        var state = GetState(grid);
        return state?.PopupContent is { } content && (column is null || ReferenceEquals(state.PopupColumn, column))
            ? content
            : null;
    }

    /// <summary>
    /// Returns the open filter pop-up, or <see langword="null"/>.
    /// </summary>
    internal static Popup? GetOpenPopup(DataGrid grid)
        => GetState(grid)?.Popup;

    /// <summary>
    /// Closes the open filter pop-up.
    /// </summary>
    internal static void CloseColumnFilter(DataGrid grid)
        => GetState(grid)?.ClosePopup();

    /// <summary>
    /// Handles a key pressed in the grid (Alt+Down opens a column filter); returns whether it was handled.
    /// </summary>
    internal static bool HandleKey(
        DataGrid grid,
        Key key,
        Key systemKey,
        object? originalSource)
        => GetState(grid)?.HandleKey(key, systemKey, originalSource) == true;

    /// <summary>
    /// Applies a pending (debounced) search now.
    /// </summary>
    internal static void ApplyPendingSearch(DataGrid grid)
        => GetState(grid)?.ApplySearchNow();

    /// <summary>
    /// Returns whether a search is waiting for its delay.
    /// </summary>
    internal static bool IsSearchPending(DataGrid grid)
        => GetState(grid)?.IsSearchPending == true;

    /// <summary>
    /// Returns whether filter work waits for an edit or add transaction of the view to end.
    /// </summary>
    internal static bool IsApplyPending(DataGrid grid)
        => GetState(grid)?.IsApplyPending == true;

    /// <summary>
    /// Builds the column chooser menu: one checkable item per column.
    /// </summary>
    internal static ContextMenu CreateColumnChooserMenu(DataGrid grid)
    {
        var menu = new ContextMenu();
        AutomationProperties.SetName(menu, Atc.Wpf.Resources.Miscellaneous.Columns);
        foreach (var column in grid.Columns.OrderBy(x => x.DisplayIndex))
        {
            var item = new MenuItem
            {
                Header = GetColumnTitle(column),
                IsCheckable = true,
                IsChecked = column.Visibility == Visibility.Visible,
                IsEnabled = GetCanUserHideColumn(column),
                StaysOpenOnClick = true,
                Tag = column,
            };
            item.Checked += OnColumnChooserItemChanged;
            item.Unchecked += OnColumnChooserItemChanged;
            menu.Items.Add(item);
        }

        return menu;

        void OnColumnChooserItemChanged(
            object sender,
            RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var column = (DataGridColumn)item.Tag;
            if (!item.IsChecked &&
                !grid.Columns.Any(x => x.Visibility == Visibility.Visible && !ReferenceEquals(x, column)))
            {
                // Keep at least one column shown.
                item.IsChecked = true;
                return;
            }

            column.Visibility = item.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            GetState(grid)?.ReapplySearch();
        }
    }

    /// <summary>
    /// Returns whether the column currently shows the filter header template.
    /// </summary>
    internal static bool IsHeaderWrapped(DataGridColumn column)
        => column.GetValue(WrapperHeaderTemplateProperty) is DataTemplate wrapper &&
           ReferenceEquals(column.HeaderTemplate, wrapper);

    /// <summary>
    /// Gives a column the filter header template around its own header template.
    /// </summary>
    internal static void WrapHeader(DataGridColumn column)
    {
        if (IsHeaderWrapped(column))
        {
            return;
        }

        var original = column.HeaderTemplate;
        var factory = new FrameworkElementFactory(typeof(DataGridFilterHeader));
        factory.SetBinding(DataGridFilterHeader.HeaderContentProperty, new Binding());
        factory.SetValue(DataGridFilterHeader.HeaderTemplateProperty, original);
        factory.SetValue(DataGridFilterHeader.HeaderTemplateSelectorProperty, column.HeaderTemplateSelector);
        var wrapper = new DataTemplate { VisualTree = factory };
        wrapper.Seal();

        column.SetValue(OriginalHeaderTemplateProperty, original);
        column.SetValue(WrapperHeaderTemplateProperty, wrapper);
        column.HeaderTemplate = wrapper;
    }

    private static void UnwrapHeader(DataGridColumn column)
    {
        if (!IsHeaderWrapped(column))
        {
            return;
        }

        column.HeaderTemplate = (DataTemplate?)column.GetValue(OriginalHeaderTemplateProperty);
        column.ClearValue(OriginalHeaderTemplateProperty);
        column.ClearValue(WrapperHeaderTemplateProperty);
    }

    private static DataGridFilterState? GetState(DataGrid grid)
        => (DataGridFilterState?)grid.GetValue(StateProperty);

    private static DataGridFilterState EnsureState(DataGrid grid)
    {
        if (GetState(grid) is { } state)
        {
            return state;
        }

        state = new DataGridFilterState(grid);
        grid.SetValue(StateProperty, state);
        BindingOperations.SetBinding(
            grid,
            ObservedItemsSourceProperty,
            new Binding(nameof(ItemsControl.ItemsSource)) { Source = grid });
        return state;
    }

    private static DataGridFilterButton? FindFilterButton(
        DataGrid grid,
        DataGridColumn column)
    {
        var stack = new Stack<DependencyObject>();
        stack.Push(grid);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current is DataGridFilterButton button && ReferenceEquals(button.Column, column))
            {
                return button.IsVisible ? button : null;
            }

            if (current is DataGridCellsPresenter or ScrollBar)
            {
                // The header buttons are in the column headers presenter; skip the rows.
                continue;
            }

            for (var i = VisualTreeHelper.GetChildrenCount(current) - 1; i >= 0; i--)
            {
                stack.Push(VisualTreeHelper.GetChild(current, i));
            }
        }

        return null;
    }

    private static void OnIsEnabledChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not DataGrid grid)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            EnsureState(grid);
            foreach (var column in grid.Columns)
            {
                WrapHeader(column);
            }

            return;
        }

        foreach (var column in grid.Columns)
        {
            UnwrapHeader(column);
        }

        GetState(grid)?.DisableColumnFilters();
    }

    private static void OnShowColumnChooserChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is DataGrid grid && (bool)e.NewValue)
        {
            EnsureState(grid);
        }
    }

    private static void OnSearchTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is DataGrid grid)
        {
            EnsureState(grid).OnSearchTextChanged();
        }
    }

    private static void OnObservedItemsSourceChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is DataGrid grid)
        {
            GetState(grid)?.OnItemsSourceChanged();
        }
    }
}