namespace Atc.Wpf.Sample.SamplesWpfControls.Navigation;

public partial class NavigationViewView : INotifyPropertyChanged
{
    private string status = "Ready";

    public NavigationViewView()
    {
        NavigationService = new NavigationService(type => Activator.CreateInstance(type)!);
        NavigationService.Navigated += OnNavigated;

        InitializeComponent();
        DataContext = this;

        NavigationService.NavigateTo<SamplesWpf.Navigation.HomeViewModel>();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public INavigationService NavigationService { get; }

    public NavigationParameters DetailsParameters { get; } = new NavigationParameters()
        .WithParameter("ItemId", 42)
        .WithParameter("ItemName", "Sample Item");

    public string Status
    {
        get => status;
        private set
        {
            status = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
        }
    }

    private void OnNavigated(
        object? sender,
        NavigatedEventArgs e)
        => Status = $"Navigated to {e.CurrentViewModel.GetType().Name}";

    private void OnItemInvoked(
        object? sender,
        NavigationViewItemInvokedEventArgs e)
    {
        if (!e.Item.SelectsOnInvoked)
        {
            Status = $"{e.Item.Content} was invoked: it runs an action and keeps the current selection";
            return;
        }

        // ItemInvoked is raised before the selection changes; check once the navigation has run.
        _ = Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            () =>
            {
                if (!ReferenceEquals(e.Item, ShellNavigationView.SelectedItem))
                {
                    Status = $"Navigation to {e.Item.Content} was blocked by the current page";
                }
            });
    }
}