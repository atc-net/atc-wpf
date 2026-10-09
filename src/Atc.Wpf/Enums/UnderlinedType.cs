// ReSharper disable once CheckNamespace
namespace Atc.Wpf;

/// <summary>
/// Specifies which parts of a tab control are underlined.
/// </summary>
public enum UnderlinedType
{
    /// <summary>Nothing is underlined.</summary>
    None,

    /// <summary>All tab items are underlined.</summary>
    TabItems,

    /// <summary>Only the selected tab item is underlined.</summary>
    SelectedTabItem,

    /// <summary>The tab panel is underlined.</summary>
    TabPanel,
}