// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

public static class LabelControlBaseExtensions
{
    public static bool IsValid(
        this ILabelControlBase labelControl)
        => labelControl is not ILabelControl control || control.IsValid();

    public static T? FindByIdentifier<T>(
        this List<ILabelControlBase> labelControls,
        string identifier)
        where T : class, ILabelControlBase
    {
        ArgumentNullException.ThrowIfNull(labelControls);
        ArgumentException.ThrowIfNullOrEmpty(identifier);

        return labelControls.Find(x => x.Identifier == identifier) as T;
    }
}