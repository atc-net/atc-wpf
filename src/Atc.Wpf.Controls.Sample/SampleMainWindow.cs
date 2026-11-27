namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// The main window of a sample app: a <see cref="NiceWindow"/> that shows a <see cref="SampleExplorer"/>.
/// Use it as the root of the app's <c>MainWindow.xaml</c> and list the <see cref="SampleCategory"/> items in it.
/// </summary>
[ContentProperty(nameof(Categories))]
public partial class SampleMainWindow : NiceWindow
{
    /// <summary>
    /// The text shown in the explorer's status bar.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnStatusTextChanged))]
    private string? statusText;

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleMainWindow"/> class: 1600 x 1000 (at least
    /// 1024 x 768), centred, with a <see cref="MainWindowViewModelBase"/> as DataContext. A replacement DataContext that
    /// implements <see cref="IMainWindowViewModelBase"/> gets the window's events and <c>WindowState</c>.
    /// </summary>
    public SampleMainWindow()
    {
        Width = 1600;
        Height = 1000;
        MinWidth = 1024;
        MinHeight = 768;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        Explorer = new SampleExplorer();
        Content = Explorer;

        DataContextChanged += (_, _) => BindWindowState();
        DataContext = new MainWindowViewModelBase();

        Loaded += (_, e) => ViewModel?.OnLoaded(this, e);
        Closing += (_, e) => ViewModel?.OnClosing(this, e);
        KeyDown += (_, e) => ViewModel?.OnKeyDown(this, e);
        KeyUp += (_, e) => ViewModel?.OnKeyUp(this, e);
    }

    /// <summary>
    /// Gets the explorer shown in the window.
    /// </summary>
    public SampleExplorer Explorer { get; }

    /// <summary>
    /// Gets the categories of the explorer: one tab and one sample tree each.
    /// </summary>
    public ObservableCollection<SampleCategory> Categories
        => Explorer.Categories;

    private IMainWindowViewModelBase? ViewModel
        => DataContext as IMainWindowViewModelBase;

    private void BindWindowState()
    {
        // Only a main-window view model has a WindowState; binding to anything else would be a binding error.
        if (ViewModel is null)
        {
            BindingOperations.ClearBinding(this, WindowStateProperty);
        }
        else
        {
            SetBinding(WindowStateProperty, new Binding(nameof(IMainWindowViewModelBase.WindowState)));
        }
    }

    private static void OnStatusTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((SampleMainWindow)d).Explorer.StatusText = e.NewValue as string;
}