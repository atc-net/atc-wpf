// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies how a <see cref="DataDisplay.Breadcrumb"/> handles overflow when there are too many items.
/// </summary>
public enum BreadcrumbOverflowMode
{
    /// <summary>
    /// All items are displayed without any overflow handling.
    /// </summary>
    None,

    /// <summary>
    /// Excess items are collapsed into a dropdown menu, showing only the first item,
    /// an overflow button, and the last few visible items.
    /// </summary>
    Collapsed,
}