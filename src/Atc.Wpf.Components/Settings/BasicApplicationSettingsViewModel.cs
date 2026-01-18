namespace Atc.Wpf.Components.Settings;

public class BasicApplicationSettingsViewModel : ViewModelBase
{
    private bool showThemeAndAccent = true;
    private bool showLanguage = true;
    private bool showOpenRecentFileOnStartup = true;
    private string theme = string.Empty;
    private string language = string.Empty;
    private bool openRecentFileOnStartup;

    public BasicApplicationSettingsViewModel()
    {
    }

    public BasicApplicationSettingsViewModel(
        BasicApplicationOptions applicationOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationOptions);

        theme = applicationOptions.Theme;
        language = applicationOptions.Language;
        openRecentFileOnStartup = applicationOptions.OpenRecentFileOnStartup;
    }

    public bool ShowThemeAndAccent
    {
        get => showThemeAndAccent;
        set
        {
            if (value == showThemeAndAccent)
            {
                return;
            }

            showThemeAndAccent = value;
            OnPropertyChanged();
        }
    }

    public bool ShowLanguage
    {
        get => showLanguage;
        set
        {
            if (value == showLanguage)
            {
                return;
            }

            showLanguage = value;
            OnPropertyChanged();
        }
    }

    public bool ShowOpenRecentFileOnStartup
    {
        get => showOpenRecentFileOnStartup;
        set
        {
            if (value == showOpenRecentFileOnStartup)
            {
                return;
            }

            showOpenRecentFileOnStartup = value;
            RaisePropertyChanged(() => ShowOpenRecentFileOnStartup);
        }
    }

    public string Theme
    {
        get => theme;
        set
        {
            if (value == theme)
            {
                return;
            }

            theme = value;
            IsDirty = true;
            RaisePropertyChanged();
        }
    }

    public string Language
    {
        get => language;
        set
        {
            if (value == language)
            {
                return;
            }

            language = value;
            IsDirty = true;
            RaisePropertyChanged();
        }
    }

    public bool OpenRecentFileOnStartup
    {
        get => openRecentFileOnStartup;
        set
        {
            if (value == openRecentFileOnStartup)
            {
                return;
            }

            openRecentFileOnStartup = value;
            IsDirty = true;
            RaisePropertyChanged();
        }
    }

    public override string ToString()
        => $"{nameof(Theme)}: {Theme}, {nameof(Language)}: {Language}, {nameof(OpenRecentFileOnStartup)}: {OpenRecentFileOnStartup}, {nameof(IsDirty)}: {IsDirty}";
}