namespace Atc.Wpf.Sample;

/// <summary>
/// Interaction logic for MainWindow.
/// </summary>
public partial class MainWindow
{
    public MainWindow(IMainWindowViewModel viewModel)
    {
        this.InitializeComponent();
        this.DataContext = viewModel;

        this.Loaded += this.OnLoaded;
        this.Closing += this.OnClosing;
        this.KeyDown += this.OnKeyDown;
        this.KeyUp += this.OnKeyUp;
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        var vm = this.DataContext as IMainWindowViewModel;
        vm!.OnLoaded(this, args);
    }

    private void OnClosing(object sender, CancelEventArgs args)
    {
        var vm = this.DataContext as IMainWindowViewModel;
        vm!.OnClosing(this, args);
    }

    private void OnKeyDown(object sender, KeyEventArgs args)
    {
        var vm = this.DataContext as IMainWindowViewModel;
        vm!.OnKeyDown(this, args);
    }

    private void OnKeyUp(object sender, KeyEventArgs args)
    {
        var vm = this.DataContext as IMainWindowViewModel;
        vm!.OnKeyUp(this, args);
    }

    private void TreeViewOnSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        var vm = this.DataContext as IMainWindowViewModel;
        vm!.UpdateSelectedView(e.NewValue as SampleTreeViewItem);
    }
}