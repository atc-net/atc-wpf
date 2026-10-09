namespace Atc.Wpf.Theming.Controls.Windows;

/// <summary>
/// Defines persistent settings that store the placement of a window.
/// </summary>
public interface IWindowPlacementSettings
{
    /// <summary>
    /// Gets or sets the stored window placement.
    /// </summary>
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