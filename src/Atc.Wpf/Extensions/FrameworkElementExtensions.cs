// ReSharper disable once CheckNamespace
// ReSharper disable ConvertIfStatementToSwitchStatement
namespace System.Windows;

public static class FrameworkElementExtensions
{
    /// <summary>
    ///   Executes the specified action if the element is loaded or at the loaded event if it's not loaded.
    /// </summary>
    /// <param name="element">The element where the action should be run.</param>
    /// <param name="invokeAction">An action that takes no parameters.</param>
    public static void ExecuteWhenLoaded(
        this FrameworkElement element,
        Action invokeAction)
    {
        ArgumentNullException.ThrowIfNull(element);

        if (element.IsLoaded)
        {
            element.Invoke(invokeAction);
        }
        else
        {
            void ElementLoaded(
                object o,
                RoutedEventArgs a)
            {
                element.Loaded -= ElementLoaded;
                element.Invoke(invokeAction);
            }

            element.Loaded += ElementLoaded;
        }
    }
}