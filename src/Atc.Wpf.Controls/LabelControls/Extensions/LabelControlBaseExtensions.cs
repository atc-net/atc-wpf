// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

public static class LabelControlBaseExtensions
{
    public static bool IsValid(this ILabelControlBase labelControl)
        => labelControl is not ILabelControl control || control.IsValid();

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