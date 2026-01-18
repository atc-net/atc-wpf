namespace Atc.Wpf.Sample.SamplesWpf.Navigation;

public sealed class SettingsViewModel : ViewModelBase, INavigationAware
{
    private bool notificationsEnabled = true;
    private string theme = "Dark";

    public bool NotificationsEnabled
    {
        get => notificationsEnabled;
        set
        {
            notificationsEnabled = value;
            RaisePropertyChanged();
        }
    }

    public string Theme
    {
        get => theme;
        set
        {
            theme = value;
            RaisePropertyChanged();
        }
    }

    public void OnNavigatedTo(NavigationParameters? parameters)
    {
        // Load settings
    }

    public void OnNavigatedFrom()
    {
        // Save settings if needed
    }
}