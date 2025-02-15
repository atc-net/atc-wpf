namespace Atc.Wpf.Sample;

/// <summary>
/// Interaction logic for MainWindow.
/// </summary>
public partial class MainWindow
{
    private IMainWindowViewModel GetViewModel() => (IMainWindowViewModel)DataContext!;

    private readonly TreeView[] sampleTreeViews;

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
            StvSamplesWpfSourceGeneratorsTreeView,
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
            var showByItem = string.IsNullOrEmpty(filter) ||
                             string.IsNullOrEmpty(header) ||
                             header.Contains(filter, StringComparison.OrdinalIgnoreCase);

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