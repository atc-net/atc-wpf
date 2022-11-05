namespace Atc.Wpf.Helpers;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
public static class VisualTreeHelperEx
{
    public static T? GetParent<T>(DependencyObject child)
        where T : DependencyObject
    {
        var parent = VisualTreeHelper.GetParent(child);

        return parent switch
        {
            null => null,
            T tParent => tParent,
            _ => GetParent<T>(parent),
        };
    }
}