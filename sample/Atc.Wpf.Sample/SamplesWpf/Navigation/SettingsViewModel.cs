namespace Atc.Wpf.Sample.SamplesWpf.Navigation;

public sealed partial class SettingsViewModel : ViewModelBase, INavigationAware, INavigationGuard
{
    private bool initialNotificationsEnabled;
    private string initialTheme = string.Empty;

    [ObservableProperty(AfterChangedCallback = nameof(UpdateUnsavedChanges))]
    private bool notificationsEnabled = true;

    [ObservableProperty(AfterChangedCallback = nameof(UpdateUnsavedChanges))]
    private string theme = "Dark";

    [ObservableProperty(SetterAccessModifier = Atc.XamlToolkit.PropertyAccessModifier.Private)]
    private bool hasUnsavedChanges;

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