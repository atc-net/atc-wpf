// ReSharper disable InvertIf
namespace Atc.Wpf.Sample;

[SuppressMessage("", "S1871:Conditional structure should not have exactly the same implementation", Justification = "OK.")]
public partial class MainWindow
{
    private IMainWindowViewModel GetViewModel()
        => (IMainWindowViewModel)DataContext!;

    private readonly TreeView[] sampleTreeViews;
    private readonly Dictionary<TreeView, TabItem> treeViewToTabItem = [];
    private readonly Dictionary<TreeView, Badge> treeViewToBadge = [];

    private bool suppressSelectionChanged;

    public MainWindow(IMainWindowViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;

        Loaded += OnLoaded;
        Closing += OnClosing;
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;

        sampleTreeViews =
        [
            StvSampleWpf,
            StvSampleWpfControls,
            StvSampleWpfNetworkControls,
            StvSamplesWpfSourceGenerators,
            StvSampleWpfFontIcons,
            StvSampleWpfTheming,
        ];

        InitializeTabMappings();
    }

    private void InitializeTabMappings()
    {
        // Map TreeViews to TabItems
        treeViewToTabItem[StvSampleWpf] = TabWpf;
        treeViewToTabItem[StvSampleWpfControls] = TabWpfControls;
        treeViewToTabItem[StvSampleWpfNetworkControls] = TabWpfNetworkControls;
        treeViewToTabItem[StvSampleWpfTheming] = TabWpfTheming;
        treeViewToTabItem[StvSamplesWpfSourceGenerators] = TabWpfSourceGenerators;
        treeViewToTabItem[StvSampleWpfFontIcons] = TabWpfFontIcons;

        // Map TreeViews to Badges
        treeViewToBadge[StvSampleWpf] = BadgeWpf;
        treeViewToBadge[StvSampleWpfControls] = BadgeWpfControls;
        treeViewToBadge[StvSampleWpfNetworkControls] = BadgeWpfNetworkControls;
        treeViewToBadge[StvSampleWpfTheming] = BadgeWpfTheming;
        treeViewToBadge[StvSamplesWpfSourceGenerators] = BadgeWpfSourceGenerators;
        treeViewToBadge[StvSampleWpfFontIcons] = BadgeWpfFontIcons;
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        GetViewModel().OnLoaded(this, e);

        // Rule 1: Application startup - no tab selected, all TreeViews visible
        suppressSelectionChanged = true;
        SamplesTabControl.SelectedIndex = -1;
        suppressSelectionChanged = false;

        UpdateTreeViewVisibility();
        Keyboard.Focus(TbSampleFilter);
    }

    private void OnClosing(
        object? sender,
        CancelEventArgs e)
        => GetViewModel().OnClosing(this, e);

    private void OnKeyDown(
        object sender,
        KeyEventArgs e)
        => GetViewModel().OnKeyDown(this, e);

    private void OnKeyUp(
        object sender,
        KeyEventArgs e)
        => GetViewModel().OnKeyUp(this, e);

    private void TabControlOnSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (suppressSelectionChanged)
        {
            return;
        }

        // Rule 5: Tab clicked - show only that category's TreeView
        UpdateTreeViewVisibility();
        ApplyCurrentFilter();
    }

    private void TreeViewOnSelectionChanged(
        object sender,
        RoutedPropertyChangedEventArgs<object> e)
    {
        GetViewModel().UpdateSelectedView(e.NewValue as SampleTreeViewItem);

        // Rule 4: TreeView item clicked - select the tab containing this item
        if (sender is TreeView treeView && e.NewValue is SampleTreeViewItem)
        {
            SelectTabForTreeView(treeView);
        }
    }

    private void FilterOnTextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        var filter = textBox.Text.Trim();
        var isFilterEmpty = string.IsNullOrWhiteSpace(filter);

        // Rule 3: SearchBox cleared - deselect tab, show all TreeViews
        // Rule 6: SearchBox text changed while tab selected - deselect tab
        suppressSelectionChanged = true;
        SamplesTabControl.SelectedIndex = -1;
        suppressSelectionChanged = false;
        UpdateTreeViewVisibility();

        // Deselect all TreeView items when searching
        ClearTreeViewSelections();

        var treeViewMatchCounts = new Dictionary<TreeView, int>();

        foreach (var sampleTreeView in sampleTreeViews)
        {
            _ = SetVisibilityByFilterTreeViewItems(sampleTreeView.Items, filter);
            var count = isFilterEmpty ? 0 : CountByFilterTreeViewItems(sampleTreeView.Items, filter);
            treeViewMatchCounts[sampleTreeView] = count;
        }

        UpdateTabHeaders(treeViewMatchCounts, isFilterEmpty);
    }

    private void UpdateTreeViewVisibility()
    {
        var selectedTab = SamplesTabControl.SelectedItem as TabItem;

        foreach (var (treeView, tabItem) in treeViewToTabItem)
        {
            if (selectedTab is null)
            {
                // No tab selected - show all TreeViews
                treeView.Visibility = Visibility.Visible;
            }
            else
            {
                // Tab selected - show only matching TreeView
                treeView.Visibility = tabItem == selectedTab
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
    }

    private void SelectTabForTreeView(TreeView treeView)
    {
        if (treeViewToTabItem.TryGetValue(treeView, out var tabItem))
        {
            tabItem.IsSelected = true;
        }
    }

    private void ClearTreeViewSelections()
    {
        foreach (var treeView in sampleTreeViews)
        {
            ClearTreeViewItemSelection(treeView.Items);
        }
    }

    private static void ClearTreeViewItemSelection(ItemCollection items)
    {
        foreach (var item in items)
        {
            if (item is TreeViewItem treeViewItem)
            {
                treeViewItem.IsSelected = false;
                ClearTreeViewItemSelection(treeViewItem.Items);
            }
        }
    }

    private void ApplyCurrentFilter()
    {
        var filter = TbSampleFilter.Text.Trim();
        var isFilterEmpty = string.IsNullOrWhiteSpace(filter);

        var treeViewMatchCounts = new Dictionary<TreeView, int>();

        foreach (var sampleTreeView in sampleTreeViews)
        {
            _ = SetVisibilityByFilterTreeViewItems(sampleTreeView.Items, filter);
            var count = isFilterEmpty ? 0 : CountByFilterTreeViewItems(sampleTreeView.Items, filter);
            treeViewMatchCounts[sampleTreeView] = count;
        }

        UpdateTabHeaders(treeViewMatchCounts, isFilterEmpty);
    }

    private void UpdateTabHeaders(
        Dictionary<TreeView, int> matchCounts,
        bool resetToOriginal)
    {
        foreach (var (treeView, badge) in treeViewToBadge)
        {
            if (resetToOriginal)
            {
                badge.BadgeContent = null;
            }
            else if (matchCounts.TryGetValue(treeView, out var count) && count > 0)
            {
                badge.BadgeContent = count;
            }
            else
            {
                badge.BadgeContent = null;
            }
        }
    }

    private static bool SetVisibilityByFilterTreeViewItems(
        IEnumerable items,
        string filter)
    {
        var showRoot = false;
        var isFilterEmpty = string.IsNullOrWhiteSpace(filter);

        foreach (var item in items)
        {
            if (item is not TreeViewItem treeViewItem)
            {
                continue;
            }

            var showBySubItems = SetVisibilityByFilterTreeViewItems(
                treeViewItem.Items,
                filter);

            bool showByItem;
            if (isFilterEmpty)
            {
                // Show all items when filter is empty
                showByItem = true;
            }
            else
            {
                var header = treeViewItem.Header?.ToString();
                var tag = treeViewItem.Tag?.ToString();
                showByItem = MatchesFilter(header, filter) || MatchesFilter(tag, filter);
            }

            treeViewItem.Visibility = showByItem || showBySubItems
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (!showRoot &&
                treeViewItem.Visibility == Visibility.Visible)
            {
                showRoot = true;
            }
        }

        return showRoot;
    }

    private static int CountByFilterTreeViewItems(
        IEnumerable items,
        string filter)
    {
        var count = 0;
        if (string.IsNullOrEmpty(filter))
        {
            return count;
        }

        foreach (var item in items)
        {
            if (item is not TreeViewItem treeViewItem)
            {
                continue;
            }

            count += CountByFilterTreeViewItems(
                treeViewItem.Items,
                filter);

            var header = treeViewItem.Header?.ToString();
            var tag = treeViewItem.Tag?.ToString();

            if (MatchesFilter(header, filter) || MatchesFilter(tag, filter))
            {
                count++;
            }
        }

        return count;
    }

    private static bool MatchesFilter(
        string? text,
        string filter)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(filter))
        {
            return false;
        }

        // PascalCase abbreviation match: "NW" matches "NiceWindow", "RC" matches "RelayCommand"
        // When filter is all uppercase (2+ chars), ONLY match against PascalCase initials
        if (filter.Length >= 2 && IsAllUpperCase(filter))
        {
            var upperCaseLetters = GetUpperCaseLetters(text);
            return upperCaseLetters.Contains(filter, StringComparison.Ordinal);
        }

        // Contains match (case-insensitive) for normal text filters
        return text.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAllUpperCase(string text)
        => text.All(char.IsUpper);

    private static string GetUpperCaseLetters(string text)
    {
        var sb = new StringBuilder();
        foreach (var c in text.Where(char.IsUpper))
        {
            sb.Append(c);
        }

        return sb.ToString();
    }

    private void SampleExpandAll(
        object sender,
        RoutedEventArgs e)
        => ProcessTreeViewItems(expand: true);

    private void SampleCollapseAll(
        object sender,
        RoutedEventArgs e)
        => ProcessTreeViewItems(expand: false);

    private void ProcessTreeViewItems(bool expand)
    {
        foreach (var treeView in sampleTreeViews)
        {
            foreach (TreeViewItem item in treeView.Items)
            {
                SetTreeViewItemExpansion(item, expand);
            }
        }
    }

    private static void SetTreeViewItemExpansion(
        TreeViewItem treeViewItem,
        bool expand)
    {
        treeViewItem.IsExpanded = expand;

        foreach (TreeViewItem item in treeViewItem.Items)
        {
            SetTreeViewItemExpansion(item, expand);
        }
    }
}
