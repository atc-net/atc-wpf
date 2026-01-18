namespace Atc.Wpf.Components.Dialogs;

public class BasicApplicationSettingsDialogBoxViewModel : ViewModelBase
{
    private readonly DirectoryInfo? dataDirectory;
    private readonly BasicApplicationSettingsViewModel applicationSettingsBackup;

    public IRelayCommand<NiceDialogBox> OkCommand
        => new RelayCommand<NiceDialogBox>(OkCommandHandler);

    public IRelayCommand<NiceDialogBox> CancelCommand
        => new RelayCommand<NiceDialogBox>(CancelCommandHandler);

    public BasicApplicationSettingsDialogBoxViewModel(
        BasicApplicationSettingsViewModel basicApplicationSettingsViewModel)
    {
        ArgumentNullException.ThrowIfNull(basicApplicationSettingsViewModel);

        ApplicationSettings = basicApplicationSettingsViewModel.Clone();
        applicationSettingsBackup = basicApplicationSettingsViewModel.Clone();

        TitleBarText = Miscellaneous.ApplicationSettings;

        ThemeManager.Current.ThemeChanged += OnThemeChanged;
        CultureManager.UiCultureChanged += OnUiCultureChanged;
    }

    public BasicApplicationSettingsDialogBoxViewModel(
        DirectoryInfo applicationDataDirectory,
        BasicApplicationSettingsViewModel basicApplicationSettingsViewModel)
        : this(basicApplicationSettingsViewModel)
    {
        ArgumentNullException.ThrowIfNull(applicationDataDirectory);
        dataDirectory = applicationDataDirectory;
    }

    public string TitleBarText { get; set; }

    public ContentControl? HeaderControl { get; set; }

    public BasicApplicationSettingsViewModel ApplicationSettings { get; set; }

    public void SetHeaderControlInsteadOfTitleBarText()
    {
        TitleBarText = string.Empty;
        HeaderControl = new ContentControl
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Content = new TextBlock
            {
                Text = Miscellaneous.ApplicationSettings,
                FontSize = 24,
            },
        };
    }

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