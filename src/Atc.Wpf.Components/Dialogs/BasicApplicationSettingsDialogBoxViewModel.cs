using ComponentsMisc = Atc.Wpf.Components.Resources.Miscellaneous;
using SharedMisc = Atc.Wpf.Resources.Miscellaneous;

namespace Atc.Wpf.Components.Dialogs;

/// <summary>
/// View model for the <see cref="BasicApplicationSettingsDialogBox"/>, editing a copy of the
/// application settings and saving or reverting them when the dialog is closed.
/// </summary>
public class BasicApplicationSettingsDialogBoxViewModel : ViewModelBase, IDisposable
{
    private readonly DirectoryInfo? dataDirectory;
    private readonly BasicApplicationSettingsViewModel applicationSettingsBackup;
    private bool isDisposed;

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

    // Returns the error instead of throwing: an app installed under Program Files cannot write next
    // to its exe, and that must not take the host app down from the OK button.
    private Exception? SaveUpdatesToCustomFile()
    {
        var file = new FileInfo(
            System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                AtcFileNameConstants.AppSettingsCustom));

        var error = TryWriteCustomSettings(file.FullName, ToJson());
        if (error is not null || dataDirectory is null)
        {
            return error;
        }

        try
        {
            File.Copy(
                file.FullName,
                System.IO.Path.Combine(
                    dataDirectory.FullName,
                    AtcFileNameConstants.AppSettingsCustom),
                overwrite: true);
            return null;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Security.SecurityException)
        {
            return ex;
        }
    }

    internal static Exception? TryWriteCustomSettings(
        string path,
        string json)
    {
        try
        {
            File.WriteAllText(path, json);
            return null;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Security.SecurityException)
        {
            return ex;
        }
    }

    private static void ShowSaveFailureDialog(
        Window owner,
        Exception ex)
    {
        var settings = new DialogBoxSettings(DialogBoxType.Ok, LogCategoryType.Error)
        {
            TitleBarText = ComponentsMisc.SaveSettingsFailed,
        };

        var message = string.Format(
            CultureInfo.CurrentCulture,
            ComponentsMisc.CouldNotSaveSettingsFormat1,
            ex.Message);

        new InfoDialogBox(owner, settings, message).ShowDialog();
    }

    private void OnThemeChanged(
        object? sender,
        ThemeChangedEventArgs e)
    {
        ApplicationSettings.Theme = e.NewTheme.Name;
    }

    /// <summary>
    /// Detaches the view model from the process-wide theme and culture change events. Safe to call more than once.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the event subscriptions held by this view model.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> when called from <see cref="Dispose()"/>.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (isDisposed || !disposing)
        {
            return;
        }

        isDisposed = true;
        ThemeManager.Current.ThemeChanged -= OnThemeChanged;
        CultureManager.UiCultureChanged -= OnUiCultureChanged;
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        ApplicationSettings.Language = e.NewCulture.Name;
    }

    private void OkCommandHandler(NiceDialogBox dialogBox)
    {
        if (ApplicationSettings.IsDirty)
        {
            if (dataDirectory is not null)
            {
                var error = SaveUpdatesToCustomFile();
                if (error is not null)
                {
                    // The dialog stays open with the edits, so the user can retry or cancel.
                    ShowSaveFailureDialog(dialogBox, error);
                    return;
                }
            }

            dialogBox.DialogResult = true;
            ApplicationSettings.IsDirty = false;
        }

        Dispose();
        dialogBox.Close();
    }

    private void CancelCommandHandler(NiceDialogBox dialogBox)
    {
        Dispose();

        if (ApplicationSettings.IsDirty)
        {
            if (!ApplicationSettings.Theme.Equals(applicationSettingsBackup.Theme, StringComparison.Ordinal))
            {
                var sa = applicationSettingsBackup.Theme.Split('.');
                if (sa.Length == 2)
                {
                    ThemeManager.Current.ChangeTheme(Application.Current, sa[0], sa[1]);
                }
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