namespace Atc.Wpf.Theming.Controls.Windows.Internal;

/// <summary>
/// This settings class is the default way to save the placement of the window
/// </summary>
internal class WindowApplicationSettings : ApplicationSettingsBase, IWindowPlacementSettings
{
    public WindowApplicationSettings(Window window)
        : base(window.GetType().FullName)
    {
    }

    [UserScopedSetting]
    public WindowPlacementSetting? Placement
    {
        get
        {
            try
            {
                return this[nameof(Placement)] as WindowPlacementSetting;
            }
            catch (ConfigurationErrorsException? ex)
            {
                string? filename = null;
                while (ex is not null && (filename = ex.Filename) is null)
                {
                    ex = ex.InnerException as ConfigurationErrorsException;
                }

                throw new AtcAppsException($"The settings file '{filename ?? "<unknown>"}' seems to be corrupted", ex);
            }
        }
        set => this[nameof(Placement)] = value;
    }

    /// <summary>
    /// Upgrades the application settings on loading.
    /// </summary>
    [UserScopedSetting]
    public bool UpgradeSettings
    {
        get
        {
            try
            {
                return (this[nameof(UpgradeSettings)] as bool?).GetValueOrDefault(true);
            }
            catch (ConfigurationErrorsException? ex)
            {
                string? filename = null;
                while (ex is not null && (filename = ex.Filename) is null)
                {
                    ex = ex.InnerException as ConfigurationErrorsException;
                }

                throw new AtcAppsException($"The settings file '{filename ?? "<unknown>"}' seems to be corrupted", ex);
            }
        }
        set => this[nameof(UpgradeSettings)] = value;
    }
}