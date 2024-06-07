// ReSharper disable once CheckNamespace
namespace System.Windows.Threading;

/// <summary>
/// Provides extension methods for the <see cref="DispatcherObject"/> class to invoke actions based on thread access requirements.
/// </summary>
public static class DispatcherObjectExtensions
{
    /// <summary>
    /// Invokes the specified function on the dispatcher thread if required, otherwise executes it directly.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="dispatcherObject">The dispatcher object to use for invoking the function.</param>
    /// <param name="func">The function to be executed.</param>
    /// <returns>The result of the function execution.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dispatcherObject"/> or <paramref name="func"/> is null.</exception>
    public static T Invoke<T>(
        this DispatcherObject dispatcherObject,
        Func<T> func)
    {
        ArgumentNullException.ThrowIfNull(dispatcherObject);
        ArgumentNullException.ThrowIfNull(func);

        return dispatcherObject.Dispatcher.CheckAccess()
            ? func()
            : dispatcherObject.Dispatcher.Invoke(func);
    }

    /// <summary>
    /// Invokes the specified action on the dispatcher thread if required, otherwise executes it directly.
    /// </summary>
    /// <param name="dispatcherObject">The dispatcher object to use for invoking the action.</param>
    /// <param name="invokeAction">The action to be executed.</param>
    /// <param name="priority">The priority at which the action is invoked, if required. The default is <see cref="DispatcherPriority.Normal"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dispatcherObject"/> or <paramref name="invokeAction"/> is null.</exception>
    public static void Invoke(
        this DispatcherObject dispatcherObject,
        Action invokeAction,
        DispatcherPriority priority = DispatcherPriority.Normal)
    {
        ArgumentNullException.ThrowIfNull(dispatcherObject);
        ArgumentNullException.ThrowIfNull(invokeAction);

        if (dispatcherObject.Dispatcher.CheckAccess())
        {
            invokeAction();
        }
        else
        {
            dispatcherObject.Dispatcher.Invoke(invokeAction, priority);
        }
    }

    /// <summary>
    /// Runs the specified action on the UI thread.
    /// </summary>
    /// <param name="dispatcherObject">The dispatcher object to use for invoking the action.</param>
    /// <param name="invokeAction">The action to be executed.</param>
    /// <param name="priority">The priority at which the action is invoked. The default is <see cref="DispatcherPriority.Normal"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dispatcherObject"/> or <paramref name="invokeAction"/> is null.</exception>
    public static void RunOnUiThread(
        this DispatcherObject dispatcherObject,
        Action invokeAction,
        DispatcherPriority priority = DispatcherPriority.Normal)
    {
        ArgumentNullException.ThrowIfNull(dispatcherObject);
        ArgumentNullException.ThrowIfNull(invokeAction);

        TaskHelper.FireAndForget(() => _ = dispatcherObject.BeginInvoke(invokeAction, priority));
    }

    /// <summary>
    /// Asynchronously begins invoking the specified action on the dispatcher thread if required, otherwise executes it directly.
    /// </summary>
    /// <param name="dispatcherObject">The dispatcher object to use for invoking the action.</param>
    /// <param name="invokeAction">The action to be executed.</param>
    /// <param name="priority">The priority at which the action is invoked, if required. The default is <see cref="DispatcherPriority.Normal"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dispatcherObject"/> or <paramref name="invokeAction"/> is null.</exception>
    public static Task BeginInvoke(
        this DispatcherObject dispatcherObject,
        Action invokeAction,
        DispatcherPriority priority = DispatcherPriority.Normal)
    {
        ArgumentNullException.ThrowIfNull(dispatcherObject);
        ArgumentNullException.ThrowIfNull(invokeAction);

        return dispatcherObject.Dispatcher.BeginInvokeIfRequired(invokeAction, priority);
    }

    /// <summary>
    /// Asynchronously begins invoking the specified action on the dispatcher thread if required, otherwise executes it directly, passing the dispatcher object as a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the dispatcher object.</typeparam>
    /// <param name="dispatcherObject">The dispatcher object to use for invoking the action.</param>
    /// <param name="invokeAction">The action to be executed, which takes the dispatcher object as a parameter.</param>
    /// <param name="priority">The priority at which the action is invoked, if required. The default is <see cref="DispatcherPriority.Normal"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dispatcherObject"/> or <paramref name="invokeAction"/> is null.</exception>
    public static void BeginInvoke<T>(
        this T dispatcherObject,
        Action<T> invokeAction,
        DispatcherPriority priority = DispatcherPriority.Normal)
        where T : DispatcherObject
    {
        ArgumentNullException.ThrowIfNull(dispatcherObject);
        ArgumentNullException.ThrowIfNull(invokeAction);

        _ = dispatcherObject.Dispatcher.BeginInvoke(priority, new Action(() => invokeAction(dispatcherObject)));
    }
}