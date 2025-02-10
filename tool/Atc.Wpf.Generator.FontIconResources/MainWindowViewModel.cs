namespace Atc.Wpf.Generator.FontIconResources;

public sealed class MainWindowViewModel : MainWindowViewModelBase, IMainWindowViewModel
{
    private string? resourcesFolder;
    private string? outputEnumFolder;

    public MainWindowViewModel()
    {
        ResourcesFolder = @"D:\Code\atc-net\atc-wpf\src\Atc.Wpf.FontIcons\Resources";
        OutputEnumFolder = @"D:\Code\atc-net\atc-wpf\src\Atc.Wpf.FontIcons\Enums";
    }

    public ICommand GenerateFontAwesomeBrandCommand => new RelayCommand(GenerateFontAwesomeBrandCommandHandler, CanGenerateCommandHandler);

    public ICommand GenerateFontAwesomeRegularCommand => new RelayCommand(GenerateFontAwesomeRegularCommandHandler, CanGenerateCommandHandler);

    public ICommand GenerateFontAwesomeSolidCommand => new RelayCommand(GenerateFontAwesomeSolidCommandHandler, CanGenerateCommandHandler);

    public ICommand GenerateBootstrapCommand => new RelayCommand(GenerateBootstrapCommandHandler, CanGenerateCommandHandler);

    public ICommand GenerateIcoCommand => new RelayCommand(GenerateIcoCommandHandler, CanGenerateCommandHandler);

    public ICommand GenerateMaterialDesignCommand => new RelayCommand(GenerateMaterialDesignCommandHandler, CanGenerateCommandHandler);

    public ICommand GenerateWeatherCommand => new RelayCommand(GenerateWeatherCommandHandler, CanGenerateCommandHandler);

    public string? ResourcesFolder
    {
        get => resourcesFolder;
        set
        {
            resourcesFolder = value;
            RaisePropertyChanged();
        }
    }

    public string? OutputEnumFolder
    {
        get => outputEnumFolder;
        set
        {
            outputEnumFolder = value;
            RaisePropertyChanged();
        }
    }

    private bool CanGenerateCommandHandler()
    {
        if (string.IsNullOrEmpty(ResourcesFolder) ||
            string.IsNullOrEmpty(OutputEnumFolder))
        {
            return false;
        }

        return true;
    }

    private void GenerateFontAwesomeBrandCommandHandler()
    {
        GenerateHelper.GenerateFontAwesomeBrand(
            new DirectoryInfo(ResourcesFolder!),
            new DirectoryInfo(OutputEnumFolder!));
    }

    private void GenerateFontAwesomeRegularCommandHandler()
    {
        GenerateHelper.GenerateFontAwesomeRegular(
            new DirectoryInfo(ResourcesFolder!),
            new DirectoryInfo(OutputEnumFolder!));
    }

    private void GenerateFontAwesomeSolidCommandHandler()
    {
        GenerateHelper.GenerateFontAwesomeSolid(
            new DirectoryInfo(ResourcesFolder!),
            new DirectoryInfo(OutputEnumFolder!));
    }

    private void GenerateBootstrapCommandHandler()
    {
        GenerateHelper.GenerateBootstrap(
            new DirectoryInfo(ResourcesFolder!),
            new DirectoryInfo(OutputEnumFolder!));
    }

    private void GenerateIcoCommandHandler()
    {
        GenerateHelper.GenerateIco(
            new DirectoryInfo(ResourcesFolder!),
            new DirectoryInfo(OutputEnumFolder!));
    }

    private void GenerateMaterialDesignCommandHandler()
    {
        GenerateHelper.GenerateMaterialDesign(
            new DirectoryInfo(ResourcesFolder!),
            new DirectoryInfo(OutputEnumFolder!));
    }

    private void GenerateWeatherCommandHandler()
    {
        GenerateHelper.GenerateWeather(
            new DirectoryInfo(ResourcesFolder!),
            new DirectoryInfo(OutputEnumFolder!));
    }
}