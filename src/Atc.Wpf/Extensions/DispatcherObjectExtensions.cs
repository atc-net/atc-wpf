// ReSharper disable once CheckNamespace
namespace System.Windows.Threading;

/// <summary>
/// Extension methods for DispatcherObject.
/// </summary>
public static class DispatcherObjectExtensions
{
    /// <summary>
    /// Runs the on UI thread.
    /// </summary>
    /// <param name="dispatcherObject">The dispatcher object.</param>
    /// <param name="action">The action.</param>
    /// <param name="priority">The priority.</param>
    public static void RunOnUiThread(
        this DispatcherObject dispatcherObject,
        Action action,
        DispatcherPriority priority = DispatcherPriority.Normal)
    {
        if (dispatcherObject is null)
        {
            throw new ArgumentNullException(nameof(dispatcherObject));
        }

        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var dispatcher = dispatcherObject.Dispatcher;
        dispatcher.BeginInvokeIfRequired(action, priority);
    }
}