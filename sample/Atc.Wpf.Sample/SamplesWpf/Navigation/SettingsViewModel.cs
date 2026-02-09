namespace Atc.Wpf.Sample.SamplesWpf.Navigation;

public sealed class SettingsViewModel : ViewModelBase, INavigationAware, INavigationGuard
{
    private bool initialNotificationsEnabled;
    private string initialTheme = string.Empty;
    private bool notificationsEnabled = true;
    private string theme = "Dark";
    private bool hasUnsavedChanges;

    public bool NotificationsEnabled
    {
        get => notificationsEnabled;
        set
        {
            if (notificationsEnabled == value)
            {
                return;
            }

            notificationsEnabled = value;
            RaisePropertyChanged();
            UpdateUnsavedChanges();
        }
    }

    public string Theme
    {
        get => theme;
        set
        {
            if (string.Equals(theme, value, StringComparison.Ordinal))
            {
                return;
            }

            theme = value;
            RaisePropertyChanged();
            UpdateUnsavedChanges();
        }
    }

    public bool HasUnsavedChanges
    {
        get => hasUnsavedChanges;
        private set
        {
            if (hasUnsavedChanges == value)
            {
                return;
            }

            hasUnsavedChanges = value;
            RaisePropertyChanged();
        }
    }

    public void OnNavigatedTo(NavigationParameters? parameters)
    {
        initialNotificationsEnabled = NotificationsEnabled;
        initialTheme = Theme;
        HasUnsavedChanges = false;
    }

    public void OnNavigatedFrom()
    {
        HasUnsavedChanges = false;
    }

    public bool CanNavigateAway()
        => !HasUnsavedChanges;

    public Task<bool> CanNavigateAwayAsync(
        CancellationToken cancellationToken = default)
        => Task.FromResult(CanNavigateAway());

    private void UpdateUnsavedChanges()
    {
        HasUnsavedChanges =
            NotificationsEnabled != initialNotificationsEnabled ||
            !string.Equals(Theme, initialTheme, StringComparison.Ordinal);
    }
}