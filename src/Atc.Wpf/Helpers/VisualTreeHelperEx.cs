namespace Atc.Wpf.Helpers;

/// <summary>
/// Provides extended methods for working with the visual tree in WPF applications.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
public static class VisualTreeHelperEx
{
    /// <summary>
    /// Finds and returns the first parent object of the specified type T in the visual tree.
    /// </summary>
    /// <typeparam name="T">The type of the parent to find.</typeparam>
    /// <param name="control">The starting dependency object to search up from.</param>
    /// <returns>The first parent object of type T found in the visual tree, or null if no such parent exists.</returns>
    public static T? GetParent<T>(
        DependencyObject control)
        where T : DependencyObject
        => FindParent<T>(control);

    /// <summary>
    /// Searches for and returns the first child object of the specified type T within the visual tree.
    /// </summary>
    /// <typeparam name="T">The type of the child to find.</typeparam>
    /// <param name="control">The parent dependency object to search down from.</param>
    /// <returns>The first child of type T found within the visual tree, or null if no such child exists.</returns>
    public static T? GetChild<T>(
        DependencyObject control)
        where T : DependencyObject
        => FindChild<T>(control);

    /// <summary>
    /// Recursively finds the first parent of the specified type T in the visual tree.
    /// </summary>
    /// <typeparam name="T">The type of the parent to find.</typeparam>
    /// <param name="control">The starting point dependency object to search up from.</param>
    /// <returns>The first parent object of type T in the visual tree, or null if no such parent is found.</returns>
    public static T? FindParent<T>(
        DependencyObject control)
        where T : DependencyObject
    {
        var parent = VisualTreeHelper.GetParent(control);
        while (parent is not null)
        {
            if (parent is T tParent)
            {
                return tParent;
            }

            parent = VisualTreeHelper.GetParent(parent);
        }

        return null;
    }

    /// <summary>
    /// Recursively searches for and returns the first child of the specified type T within the visual tree.
    /// </summary>
    /// <typeparam name="T">The type of the child to find.</typeparam>
    /// <param name="control">The parent dependency object to search down from.</param>
    /// <returns>The first child of type T, or null if no such child is found.</returns>
    public static T? FindChild<T>(
        DependencyObject control)
        where T : DependencyObject
    {
        var childNumber = VisualTreeHelper.GetChildrenCount(control);
        for (var i = 0; i < childNumber; i++)
        {
            var child = VisualTreeHelper.GetChild(control, i);
            if (child is T tChild)
            {
                return tChild;
            }

            var result = FindChild<T>(child);
            if (result is not null)
            {
                return result;
            }
        }

        return null;
    }
}