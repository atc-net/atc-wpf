namespace Atc.Wpf.Sample.SamplesWpf.Navigation;

public sealed class NavigationServiceViewModel : ViewModelBase
{
    private readonly INavigationService navigationService;
    private object? currentView;
    private string navigationStatus = "Ready";

    public NavigationServiceViewModel()
    {
        navigationService = new NavigationService(CreateViewModel);
        navigationService.Navigated += OnNavigated;

        // Navigate to home on startup
        navigationService.NavigateTo<HomeViewModel>();
    }

    public object? CurrentView
    {
        get => currentView;
        set
        {
            currentView = value;
            RaisePropertyChanged();
        }
    }

    public string NavigationStatus
    {
        get => navigationStatus;
        set
        {
            navigationStatus = value;
            RaisePropertyChanged();
        }
    }

    public bool CanGoBack => navigationService.CanGoBack;

    public bool CanGoForward => navigationService.CanGoForward;

    public IRelayCommand GoBackCommand
        => new RelayCommand(
            () => navigationService.GoBack(),
            () => CanGoBack);

    public IRelayCommand GoForwardCommand
        => new RelayCommand(
            () => navigationService.GoForward(),
            () => CanGoForward);

    public IRelayCommand NavigateToHomeCommand
        => new RelayCommand(() => navigationService.NavigateTo<HomeViewModel>());

    public IRelayCommand NavigateToSettingsCommand
        => new RelayCommand(() => navigationService.NavigateTo<SettingsViewModel>());

    public IRelayCommand NavigateToDetailsCommand
        => new RelayCommand(
            () => navigationService.NavigateTo<DetailsViewModel>(
                new NavigationParameters()
                    .WithParameter("ItemId", 42)
                    .WithParameter("ItemName", "Sample Item")));

    private void OnNavigated(
        object? sender,
        NavigatedEventArgs e)
    {
        CurrentView = CreateViewForViewModel(e.CurrentViewModel);
        NavigationStatus = $"Navigated to: {e.CurrentViewModel.GetType().Name}";
        RaisePropertyChanged(nameof(CanGoBack));
        RaisePropertyChanged(nameof(CanGoForward));
    }

    private static object CreateViewModel(Type viewModelType)
        => Activator.CreateInstance(viewModelType)!;

    private static object CreateViewForViewModel(object viewModel)
        => viewModel switch
        {
            HomeViewModel => new HomeView { DataContext = viewModel },
            SettingsViewModel => new SettingsView { DataContext = viewModel },
            DetailsViewModel => new DetailsView { DataContext = viewModel },
            _ => new TextBlock { Text = $"No view for {viewModel.GetType().Name}" },
        };
}
