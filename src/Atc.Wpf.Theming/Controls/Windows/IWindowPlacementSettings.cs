namespace Atc.Wpf.Theming.Controls.Windows;

public interface IWindowPlacementSettings
{
    WindowPlacementSetting? Placement { get; set; }

    /// <summary>
    /// Refreshes the application settings property values from persistent storage.
    /// </summary>
    void Reload();

    /// <summary>
    /// Upgrades the application settings on loading.
    /// </summary>
    bool UpgradeSettings { get; set; }

    /// <summary>
    /// Updates application settings to reflect a more recent installation of the application.
    /// </summary>
    void Upgrade();

    /// <summary>
    /// Stores the current values of the settings properties.
    /// </summary>
    void Save();
}