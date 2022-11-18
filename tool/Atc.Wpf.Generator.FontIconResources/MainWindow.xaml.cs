namespace Atc.Wpf.Generator.FontIconResources;

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
}