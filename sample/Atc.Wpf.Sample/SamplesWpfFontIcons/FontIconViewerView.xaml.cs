namespace Atc.Wpf.Sample.SamplesWpfFontIcons;

public partial class FontIconViewerView
{
    private readonly FontIconViewerDemoViewModel vm;

    public FontIconViewerView()
    {
        InitializeComponent();

        vm = new FontIconViewerDemoViewModel();
        DataContext = vm;

        vm.PropertyChanged += OnViewModelPropertyChanged;

        Loaded += OnLoaded;
    }

    [SuppressMessage("Usage", "MA0134:Observe result of async calls", Justification = "OK - fire-and-forget on Loaded.")]
    private async void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        await vm.InitializeAsync().ConfigureAwait(true);
    }

    private void OnViewModelPropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(FontIconViewerDemoViewModel.IconColor):
                _ = vm.RefreshFilterAsync();
                break;
            case nameof(FontIconViewerDemoViewModel.FilterText):
            case nameof(FontIconViewerDemoViewModel.FilterFontAwesomeBrand):
            case nameof(FontIconViewerDemoViewModel.FilterFontAwesomeRegular):
            case nameof(FontIconViewerDemoViewModel.FilterFontAwesomeSolid):
            case nameof(FontIconViewerDemoViewModel.FilterFontAwesomeBrand7):
            case nameof(FontIconViewerDemoViewModel.FilterFontAwesomeRegular7):
            case nameof(FontIconViewerDemoViewModel.FilterFontAwesomeSolid7):
            case nameof(FontIconViewerDemoViewModel.FilterBootstrap):
            case nameof(FontIconViewerDemoViewModel.FilterIcoFont):
            case nameof(FontIconViewerDemoViewModel.FilterMaterialDesign):
            case nameof(FontIconViewerDemoViewModel.FilterWeather):
                vm.DebouncedRefreshFilter();
                break;
        }
    }
}
