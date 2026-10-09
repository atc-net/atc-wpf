// ReSharper disable CheckNamespace
namespace Atc.Wpf.Forms;

/// <summary>
/// Extension methods for <see cref="ILabelControlBase"/>.
/// </summary>
public static class LabelControlBaseExtensions
{
    /// <summary>
    /// Determines whether the label control is valid; controls that do not implement <see cref="ILabelControl"/> are always valid.
    /// </summary>
    /// <param name="labelControl">The label control.</param>
    /// <returns><see langword="true"/> if the control is valid; otherwise, <see langword="false"/>.</returns>
    public static bool IsValid(this ILabelControlBase labelControl)
        => labelControl is not ILabelControl control || control.IsValid();

    /// <summary>
    /// Finds a label control by its identifier, full identifier or <c>Tag</c>.
    /// </summary>
    /// <typeparam name="T">The label control type to return.</typeparam>
    /// <param name="labelControls">The label controls to search.</param>
    /// <param name="identifier">The identifier to look for.</param>
    /// <returns>The matching label control, or <see langword="null"/> if none is found.</returns>
    public static T? FindByIdentifier<T>(
        this List<ILabelControlBase> labelControls,
        string identifier)
        where T : class, ILabelControlBase
    {
        ArgumentNullException.ThrowIfNull(labelControls);
        ArgumentException.ThrowIfNullOrEmpty(identifier);

        if (labelControls.Find(x => x.Identifier == identifier || x.GetFullIdentifier() == identifier) is T labelControl)
        {
            return labelControl;
        }

        foreach (var lc in labelControls)
        {
            if (lc is FrameworkElement { Tag: not null } frameworkElement &&
                frameworkElement.Tag.ToString() == identifier)
            {
                return lc as T;
            }
        }

        return null;
    }
}