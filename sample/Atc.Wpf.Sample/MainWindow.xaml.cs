namespace Atc.Wpf.Sample;

/// <summary>
/// Interaction logic for MainWindow.
/// </summary>
public partial class MainWindow
{
    public MainWindow(IMainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        Loaded += OnLoaded;
        Closing += OnClosing;
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        var vm = DataContext as IMainWindowViewModel;
        vm!.OnLoaded(this, e);

        Keyboard.Focus(TbSampleFilter);
    }

    private void OnClosing(
        object? sender,
        CancelEventArgs e)
    {
        var vm = DataContext as IMainWindowViewModel;
        vm!.OnClosing(this, e);
    }

    private void OnKeyDown(
        object sender,
        KeyEventArgs e)
    {
        var vm = DataContext as IMainWindowViewModel;
        vm!.OnKeyDown(this, e);
    }

    private void OnKeyUp(
        object sender,
        KeyEventArgs e)
    {
        var vm = DataContext as IMainWindowViewModel;
        vm!.OnKeyUp(this, e);
    }

    private void TreeViewOnSelectionChanged(
        object sender,
        RoutedPropertyChangedEventArgs<object> e)
    {
        var vm = DataContext as IMainWindowViewModel;
        vm!.UpdateSelectedView(e.NewValue as SampleTreeViewItem);
    }

    private void SampleFilterOnTextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        _ = SetVisibilityByFilterTreeViewItems(StvSampleWpf.Items, textBox.Text);
        _ = SetVisibilityByFilterTreeViewItems(StvSampleWpfControls.Items, textBox.Text);
        _ = SetVisibilityByFilterTreeViewItems(StvSamplesWpfSourceGeneratorsTreeView.Items, textBox.Text);
        _ = SetVisibilityByFilterTreeViewItems(StvSampleWpfFontIcons.Items, textBox.Text);
        _ = SetVisibilityByFilterTreeViewItems(StvSampleWpfTheming.Items, textBox.Text);
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
}