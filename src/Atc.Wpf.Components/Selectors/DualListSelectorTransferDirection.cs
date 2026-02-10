namespace Atc.Wpf.Components.Selectors;

/// <summary>
/// Specifies the direction of an item transfer in a <see cref="DualListSelector"/> control.
/// </summary>
public enum DualListSelectorTransferDirection
{
    /// <summary>
    /// Items are transferred from the Available list to the Selected list.
    /// </summary>
    ToSelected,

    /// <summary>
    /// Items are transferred from the Selected list to the Available list.
    /// </summary>
    ToAvailable,
}