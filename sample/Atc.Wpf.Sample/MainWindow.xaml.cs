// ReSharper disable InvertIf
namespace Atc.Wpf.Sample;

[SuppressMessage("", "S1871:Conditional structure should not have exactly the same implementation", Justification = "OK.")]
public partial class MainWindow
{
    private IMainWindowViewModel GetViewModel() => (IMainWindowViewModel)DataContext!;

    private readonly TreeView[] sampleTreeViews;

    public MainWindow(
        IMainWindowViewModel viewModel)
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
            StvSamplesWpfSourceGenerators,
            StvSampleWpfFontIcons,
            StvSampleWpfTheming,
        ];
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
        => GetViewModel().UpdateSelectedView(e.NewValue as SampleTreeViewItem);

    private void SampleFilterOnTextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        foreach (var sampleTreeView in sampleTreeViews)
        {
            _ = SetVisibilityByFilterTreeViewItems(sampleTreeView.Items, textBox.Text);
        }

        TrySelectUniqueMatchingTab(textBox.Text);
    }

    private static bool SetVisibilityByFilterTreeViewItems(
        IEnumerable items,
        string filter)
    {
        var showRoot = false;
        foreach (var item in items)
        {
            if (item is not TreeViewItem treeViewItem)
            {
                continue;
            }

            var showBySubItems = SetVisibilityByFilterTreeViewItems(
                treeViewItem.Items,
                filter);

            var header = treeViewItem.Header?.ToString();
            var tag = treeViewItem.Tag?.ToString();
            var showByItem = false;
            if (header is not null &&
                header.Length > 0 &&
                header.Contains(filter, StringComparison.OrdinalIgnoreCase))
            {
                showByItem = true;
            }
            else if (tag is not null &&
                     tag.Length > 0 &&
                     tag.Contains(filter, StringComparison.OrdinalIgnoreCase))
            {
                showByItem = true;
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

    private void TrySelectUniqueMatchingTab(
        string filter)
    {
        if (filter is not null &&
            filter.Length > 2)
        {
            var treeViewMatchCounts = new Dictionary<TreeView, int>();
            foreach (var sampleTreeView in sampleTreeViews)
            {
                var count = CountByFilterTreeViewItems(sampleTreeView.Items, filter);
                treeViewMatchCounts.Add(sampleTreeView, count);
            }

            var matchingTreeViews = treeViewMatchCounts
                .Where(kvp => kvp.Value > 0)
                .ToList();

            if (matchingTreeViews.Count == 1)
            {
                SelectTabItem(matchingTreeViews[0].Key);
            }
        }
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

            if (header is not null &&
                header.Length > 0 &&
                header.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            {
                count++;
            }
            else if (tag is not null &&
                     tag.Length > 0 &&
                     tag.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            {
                count++;
            }
        }

        return count;
    }

    private void SelectTabItem(
        TreeView treeView)
    {
        foreach (var item in SamplesTabControl.Items)
        {
            if (item is not TabItem tabItem)
            {
                continue;
            }

            if (tabItem.Content.GetType() != treeView.GetType())
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

    private void ProcessTreeViewItems(
        bool expand)
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