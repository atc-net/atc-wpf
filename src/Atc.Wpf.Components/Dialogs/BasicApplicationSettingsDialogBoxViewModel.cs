using SharedMisc = Atc.Wpf.Resources.Miscellaneous;

namespace Atc.Wpf.Components.Dialogs;

/// <summary>
/// View model for the <see cref="BasicApplicationSettingsDialogBox"/>, editing a copy of the
/// application settings and saving or reverting them when the dialog is closed.
/// </summary>
public class BasicApplicationSettingsDialogBoxViewModel : ViewModelBase
{
    private readonly DirectoryInfo? dataDirectory;
    private readonly BasicApplicationSettingsViewModel applicationSettingsBackup;

    /// <summary>
    /// Gets the command that accepts the changes, saves them when a data directory is set, and closes the dialog.
    /// </summary>
    public IRelayCommand<NiceDialogBox> OkCommand
        => new RelayCommand<NiceDialogBox>(OkCommandHandler);

    /// <summary>
    /// Gets the command that reverts any theme and language changes and closes the dialog.
    /// </summary>
    public IRelayCommand<NiceDialogBox> CancelCommand
        => new RelayCommand<NiceDialogBox>(CancelCommandHandler);

    /// <summary>
    /// Initializes a new instance of the <see cref="BasicApplicationSettingsDialogBoxViewModel"/> class
    /// that edits a copy of the given settings.
    /// </summary>
    public BasicApplicationSettingsDialogBoxViewModel(
        BasicApplicationSettingsViewModel basicApplicationSettingsViewModel)
    {
        ArgumentNullException.ThrowIfNull(basicApplicationSettingsViewModel);

        ApplicationSettings = basicApplicationSettingsViewModel.Clone();
        applicationSettingsBackup = basicApplicationSettingsViewModel.Clone();

        TitleBarText = SharedMisc.ApplicationSettings;

        ThemeManager.Current.ThemeChanged += OnThemeChanged;
        CultureManager.UiCultureChanged += OnUiCultureChanged;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BasicApplicationSettingsDialogBoxViewModel"/> class
    /// that edits a copy of the given settings and also copies saved settings to the application data directory.
    /// </summary>
    public BasicApplicationSettingsDialogBoxViewModel(
        DirectoryInfo applicationDataDirectory,
        BasicApplicationSettingsViewModel basicApplicationSettingsViewModel)
        : this(basicApplicationSettingsViewModel)
    {
        ArgumentNullException.ThrowIfNull(applicationDataDirectory);
        dataDirectory = applicationDataDirectory;
    }

    /// <summary>
    /// Gets or sets the text shown in the dialog title bar.
    /// </summary>
    public string TitleBarText { get; set; }

    /// <summary>
    /// Gets or sets the optional header control shown at the top of the dialog.
    /// </summary>
    public ContentControl? HeaderControl { get; set; }

    /// <summary>
    /// Gets or sets the application settings being edited.
    /// </summary>
    public BasicApplicationSettingsViewModel ApplicationSettings { get; set; }

    /// <summary>
    /// Clears the title bar text and shows the "Application settings" caption as a large header control instead.
    /// </summary>
    public void SetHeaderControlInsteadOfTitleBarText()
    {
        TitleBarText = string.Empty;
        HeaderControl = new ContentControl
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Content = new TextBlock
            {
                Text = SharedMisc.ApplicationSettings,
                FontSize = 24,
            },
        };
    }

    /// <summary>
    /// Serializes the visible application settings into the custom app-settings JSON, merging with
    /// the existing custom app-settings file when it exists.
    /// </summary>
    public string ToJson()
    {
        var file = new FileInfo(
            System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                AtcFileNameConstants.AppSettingsCustom));

        var dynamicJson = file.Exists
            ? new DynamicJson(file)
            : new DynamicJson();

        if (ApplicationSettings.ShowThemeAndAccent)
        {
            dynamicJson.SetValue(
                $"{BasicApplicationOptions.SectionName}.{nameof(ApplicationSettings.Theme)}",
                ApplicationSettings.Theme);
        }

        if (ApplicationSettings.ShowLanguage)
        {
            dynamicJson.SetValue(
                $"{BasicApplicationOptions.SectionName}.{nameof(ApplicationSettings.Language)}",
                ApplicationSettings.Language);
        }

        if (ApplicationSettings.ShowOpenRecentFileOnStartup)
        {
            dynamicJson.SetValue(
                $"{BasicApplicationOptions.SectionName}.{nameof(ApplicationSettings.OpenRecentFileOnStartup)}",
                ApplicationSettings.OpenRecentFileOnStartup);
        }

        return dynamicJson.ToJson();
    }

    private void SaveUpdatesToCustomFile()
    {
        var file = new FileInfo(
            System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                AtcFileNameConstants.AppSettingsCustom));

        File.WriteAllText(file.FullName, ToJson());

        if (dataDirectory is null)
        {
            return;
        }

        File.Copy(
            file.FullName,
            System.IO.Path.Combine(
                dataDirectory.FullName,
                AtcFileNameConstants.AppSettingsCustom),
            overwrite: true);
    }

    private void OnThemeChanged(
        object? sender,
        ThemeChangedEventArgs e)
    {
        ApplicationSettings.Theme = e.NewTheme.Name;
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        ApplicationSettings.Language = e.NewCulture.Name;
    }

    private void OkCommandHandler(NiceDialogBox dialogBox)
    {
        ThemeManager.Current.ThemeChanged -= OnThemeChanged;

        if (ApplicationSettings.IsDirty)
        {
            if (dataDirectory is not null)
            {
                SaveUpdatesToCustomFile();
            }

            dialogBox.DialogResult = true;
            ApplicationSettings.IsDirty = false;
        }

        dialogBox.Close();
    }

    private void CancelCommandHandler(NiceDialogBox dialogBox)
    {
        ThemeManager.Current.ThemeChanged -= OnThemeChanged;

        if (ApplicationSettings.IsDirty)
        {
            if (!ApplicationSettings.Theme.Equals(applicationSettingsBackup.Theme, StringComparison.Ordinal))
            {
                var sa = applicationSettingsBackup.Theme.Split('.');
                ThemeManager.Current.ChangeTheme(Application.Current, sa[0], sa[1]);
            }

            if (!ApplicationSettings.Language.Equals(applicationSettingsBackup.Language, StringComparison.Ordinal))
            {
                CultureManager.UiCulture = new CultureInfo(applicationSettingsBackup.Language);
            }

            ApplicationSettings = applicationSettingsBackup.Clone();
        }

        dialogBox.Close();
    }
}