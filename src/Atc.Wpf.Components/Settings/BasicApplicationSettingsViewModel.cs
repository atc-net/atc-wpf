namespace Atc.Wpf.Components.Settings;

/// <summary>
/// View model for the basic application settings (theme, language and opening the recent file on startup),
/// tracking changes through <c>IsDirty</c>.
/// </summary>
public partial class BasicApplicationSettingsViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets a value indicating whether the theme and accent setting is shown.
    /// </summary>
    [ObservableProperty]
    private bool showThemeAndAccent = true;

    /// <summary>
    /// Gets or sets a value indicating whether the language setting is shown.
    /// </summary>
    [ObservableProperty]
    private bool showLanguage = true;

    /// <summary>
    /// Gets or sets a value indicating whether the open-recent-file-on-startup setting is shown.
    /// </summary>
    [ObservableProperty]
    private bool showOpenRecentFileOnStartup = true;

    private string theme = string.Empty;
    private string language = string.Empty;
    private bool openRecentFileOnStartup;

    /// <summary>
    /// Initializes a new instance of the <see cref="BasicApplicationSettingsViewModel"/> class.
    /// </summary>
    public BasicApplicationSettingsViewModel()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BasicApplicationSettingsViewModel"/> class with values from the given options.
    /// </summary>
    public BasicApplicationSettingsViewModel(
        BasicApplicationOptions applicationOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationOptions);

        theme = applicationOptions.Theme;
        language = applicationOptions.Language;
        openRecentFileOnStartup = applicationOptions.OpenRecentFileOnStartup;
    }

    /// <summary>
    /// Gets or sets the theme name (for example <c>Dark.Blue</c>).
    /// </summary>
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

    /// <summary>
    /// Gets or sets the UI culture name (for example <c>en-US</c>).
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the most recent file is opened on startup.
    /// </summary>
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

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Theme)}: {Theme}, {nameof(Language)}: {Language}, {nameof(OpenRecentFileOnStartup)}: {OpenRecentFileOnStartup}, {nameof(IsDirty)}: {IsDirty}";
}