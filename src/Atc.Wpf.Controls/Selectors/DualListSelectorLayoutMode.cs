// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies the layout mode for a <see cref="Selectors.DualListSelector"/> control.
/// </summary>
public enum DualListSelectorLayoutMode
{
    /// <summary>
    /// Available list on the left, Selected list on the right.
    /// </summary>
    AvailableFirst,

    /// <summary>
    /// Selected list on the left, Available list on the right.
    /// </summary>
    SelectedFirst,
}