namespace Atc.Wpf.Sample.SamplesWpf.Navigation;

public sealed partial class NavigationServiceViewModel : ViewModelBase
{
    private readonly INavigationService navigationService;

    [ObservableProperty]
    private object? currentView;

    [ObservableProperty]
    private string navigationStatus = "Ready";

    [ObservableProperty]
    private bool canGoBack;

    [ObservableProperty]
    private bool canGoForward;

    public NavigationServiceViewModel()
    {
        navigationService = new NavigationService(CreateViewModel);
        navigationService.Navigated += OnNavigated;

        // Navigate to home on startup
        navigationService.NavigateTo<HomeViewModel>();
    }

    [RelayCommand]
    private void GoBack()
        => navigationService.GoBack();

    [RelayCommand]
    private void GoForward()
        => navigationService.GoForward();

    [RelayCommand]
    private void NavigateToHome()
        => navigationService.NavigateTo<HomeViewModel>();

    [RelayCommand]
    private void NavigateToSettings()
        => navigationService.NavigateTo<SettingsViewModel>();

    [RelayCommand]
    private void NavigateToDetails()
        => navigationService.NavigateTo<DetailsViewModel>(
            new NavigationParameters()
                .WithParameter("ItemId", 42)
                .WithParameter("ItemName", "Sample Item"));

    private void OnNavigated(
        object? sender,
        NavigatedEventArgs e)
    {
        CurrentView = CreateViewForViewModel(e.CurrentViewModel);
        NavigationStatus = $"Navigated to: {e.CurrentViewModel.GetType().Name}";
        CanGoBack = navigationService.CanGoBack;
        CanGoForward = navigationService.CanGoForward;
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