#pragma warning disable CS0162 // Unreachable code detected
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

    [SuppressMessage("Major Bug", "S1751:Loops with at most one iteration should be refactored", Justification = "OK.")]
    public static T? GetChild<T>(DependencyObject control)
        where T : DependencyObject
    {
        var childNumber = VisualTreeHelper.GetChildrenCount(control);
        for (var i = 0; i < childNumber; i++)
        {
            var child = VisualTreeHelper.GetChild(control, i);

            return child switch
            {
                null => null,
                T tChild => tChild,
                _ => GetChild<T>(child),
            };
        }

        return null;
    }
}