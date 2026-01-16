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
        foreach (var item in SamplesTabControl.Items)
        {
            if (item is not TabItem tabItem || tabItem.Content is not TreeView treeView)
            {
                continue;
            }

            treeViewToTabItem[treeView] = tabItem;

            // Find Badge - it may be directly the Header or inside a Grid/StackPanel
            if (tabItem.Header is Badge badge)
            {
                treeViewToBadge[treeView] = badge;
            }
            else if (tabItem.Header is Panel panel)
            {
                var badgeInPanel = panel.Children
                    .OfType<Badge>()
                    .FirstOrDefault();
                if (badgeInPanel is not null)
                {
                    treeViewToBadge[treeView] = badgeInPanel;
                }
            }
        }
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        GetViewModel().OnLoaded(this, e);
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

    private void TreeViewOnSelectionChanged(
        object sender,
        RoutedPropertyChangedEventArgs<object> e)
    {
        GetViewModel().UpdateSelectedView(e.NewValue as SampleTreeViewItem);

        // Track selection: switch to the tab containing the selected item
        if (sender is TreeView treeView && e.NewValue is SampleTreeViewItem)
        {
            SelectTabItem(treeView);
        }
    }

    private void SampleFilterOnTextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        var filter = textBox.Text.Trim();
        var isFilterEmpty = string.IsNullOrWhiteSpace(filter);

        var treeViewMatchCounts = new Dictionary<TreeView, int>();

        foreach (var sampleTreeView in sampleTreeViews)
        {
            _ = SetVisibilityByFilterTreeViewItems(sampleTreeView.Items, filter);
            var count = isFilterEmpty ? 0 : CountByFilterTreeViewItems(sampleTreeView.Items, filter);
            treeViewMatchCounts[sampleTreeView] = count;
        }

        UpdateTabHeaders(treeViewMatchCounts, isFilterEmpty);
        TrySelectMatchingTab(treeViewMatchCounts, filter);
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

    private void TrySelectMatchingTab(
        Dictionary<TreeView, int> treeViewMatchCounts,
        string filter)
    {
        if (string.IsNullOrWhiteSpace(filter) || filter.Length < 2)
        {
            return;
        }

        var matchingTreeViews = treeViewMatchCounts
            .Where(kvp => kvp.Value > 0)
            .ToList();

        switch (matchingTreeViews.Count)
        {
            case 0:
                return;

            // If only one tab has matches, switch to it
            case 1:
                SelectTabItem(matchingTreeViews[0].Key);
                return;
        }

        // If current tab has no matches, switch to the first tab that does
        var currentTreeView = GetCurrentTreeView();
        if (currentTreeView is not null &&
            treeViewMatchCounts.TryGetValue(currentTreeView, out var currentCount) &&
            currentCount == 0)
        {
            SelectTabItem(matchingTreeViews[0].Key);
        }
    }

    private TreeView? GetCurrentTreeView()
    {
        if (SamplesTabControl.SelectedItem is not TabItem tabItem)
        {
            return null;
        }

        return tabItem.Content as TreeView;
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

    private void SelectTabItem(TreeView treeView)
    {
        foreach (var item in SamplesTabControl.Items)
        {
            if (item is not TabItem tabItem)
            {
                continue;
            }

            if (tabItem.Content is null || tabItem.Content.GetType() != treeView.GetType())
            {
                continue;
            }

            tabItem.IsSelected = true;
            break;
        }
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