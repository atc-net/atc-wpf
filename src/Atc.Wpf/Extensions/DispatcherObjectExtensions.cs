// ReSharper disable once CheckNamespace
namespace System.Windows.Threading;

/// <summary>
/// Extension methods for DispatcherObject.
/// </summary>
public static class DispatcherObjectExtensions
{
    public static T Invoke<T>(
        this DispatcherObject dispatcherObject,
        Func<T> func)
    {
        ArgumentNullException.ThrowIfNull(dispatcherObject);
        ArgumentNullException.ThrowIfNull(func);

        if (dispatcherObject.Dispatcher.CheckAccess())
        {
            return func();
        }

        return dispatcherObject.Dispatcher.Invoke(func);
    }

    public static void Invoke(
        this DispatcherObject dispatcherObject,
        Action invokeAction)
    {
        ArgumentNullException.ThrowIfNull(dispatcherObject);
        ArgumentNullException.ThrowIfNull(invokeAction);

        if (dispatcherObject.Dispatcher.CheckAccess())
        {
            invokeAction();
        }
        else
        {
            dispatcherObject.Dispatcher.Invoke(invokeAction);
        }
    }

    /// <summary>
    /// Runs the on UI thread.
    /// </summary>
    /// <param name="dispatcherObject">The dispatcher object.</param>
    /// <param name="invokeAction">The action to invoke.</param>
    /// <param name="priority">The priority.</param>
    public static void RunOnUiThread(
        this DispatcherObject dispatcherObject,
        Action invokeAction,
        DispatcherPriority priority = DispatcherPriority.Normal)
    {
        ArgumentNullException.ThrowIfNull(dispatcherObject);
        ArgumentNullException.ThrowIfNull(invokeAction);

        BeginInvoke(dispatcherObject, invokeAction, priority);
    }

    public static void BeginInvoke(
        this DispatcherObject dispatcherObject,
        Action invokeAction,
        DispatcherPriority priority = DispatcherPriority.Normal)
    {
        ArgumentNullException.ThrowIfNull(dispatcherObject);
        ArgumentNullException.ThrowIfNull(invokeAction);

        dispatcherObject.Dispatcher?.BeginInvokeIfRequired(invokeAction, priority);
    }

    public static void BeginInvoke<T>(
        this T dispatcherObject,
        Action<T> invokeAction,
        DispatcherPriority priority = DispatcherPriority.Normal)
        where T : DispatcherObject
    {
        ArgumentNullException.ThrowIfNull(dispatcherObject);
        ArgumentNullException.ThrowIfNull(invokeAction);

        dispatcherObject.Dispatcher?.BeginInvoke(priority, new Action(() => invokeAction(dispatcherObject)));
    }
}