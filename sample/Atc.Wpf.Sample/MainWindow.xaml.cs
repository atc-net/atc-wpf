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

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        var vm = DataContext as IMainWindowViewModel;
        vm!.OnLoaded(this, args);
    }

    private void OnClosing(object? sender, CancelEventArgs args)
    {
        var vm = DataContext as IMainWindowViewModel;
        vm!.OnClosing(this, args);
    }

    private void OnKeyDown(object sender, KeyEventArgs args)
    {
        var vm = DataContext as IMainWindowViewModel;
        vm!.OnKeyDown(this, args);
    }

    private void OnKeyUp(object sender, KeyEventArgs args)
    {
        var vm = DataContext as IMainWindowViewModel;
        vm!.OnKeyUp(this, args);
    }

    private void TreeViewOnSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        var vm = DataContext as IMainWindowViewModel;
        vm!.UpdateSelectedView(e.NewValue as SampleTreeViewItem);
    }
}