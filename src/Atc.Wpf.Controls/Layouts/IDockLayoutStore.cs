namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// Stores the layouts that a <see cref="DockPanelPro"/> with <see cref="DockPanelPro.AutoSaveLayout"/> saves and
/// loads, keyed by its <see cref="DockPanelPro.LayoutId"/>.
/// </summary>
public interface IDockLayoutStore
{
    /// <summary>
    /// Loads a saved layout.
    /// </summary>
    /// <param name="layoutId">The layout identifier.</param>
    /// <returns>The layout JSON, or <see langword="null"/> when none is saved.</returns>
    string? Load(string layoutId);

    /// <summary>
    /// Saves a layout.
    /// </summary>
    /// <param name="layoutId">The layout identifier.</param>
    /// <param name="layout">The layout JSON (see <see cref="DockPanelPro.SaveLayout"/>).</param>
    void Save(
        string layoutId,
        string layout);
}