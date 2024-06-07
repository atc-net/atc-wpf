// ReSharper disable once CheckNamespace
namespace System.Windows.Threading;

/// <summary>
/// Provides extension methods for the <see cref="Dispatcher"/> class to invoke actions based on thread access requirements.
/// </summary>
public static class DispatcherExtensions
{
    /// <summary>
    /// Invokes the specified action on the dispatcher thread if required, otherwise executes it directly.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to use for invoking the action.</param>
    /// <param name="action">The action to be executed.</param>
    /// <param name="priority">The priority at which the action is invoked, if required. The default is <see cref="DispatcherPriority.Normal"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dispatcher"/> or <paramref name="action"/> is null.</exception>
    public static void InvokeIfRequired(
        this Dispatcher dispatcher,
        Action action,
        DispatcherPriority priority = DispatcherPriority.Normal)
    {
        ArgumentNullException.ThrowIfNull(dispatcher);
        ArgumentNullException.ThrowIfNull(action);

        if (dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            dispatcher.Invoke(action, priority);
        }
    }

    /// <summary>
    /// Asynchronously invokes the specified action on the dispatcher thread if required, otherwise executes it directly.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to use for invoking the action.</param>
    /// <param name="action">The action to be executed.</param>
    /// <param name="priority">The priority at which the action is invoked, if required. The default is <see cref="DispatcherPriority.Normal"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dispatcher"/> or <paramref name="action"/> is null.</exception>
    public static async Task InvokeAsyncIfRequired(
        this Dispatcher dispatcher,
        Action action,
        DispatcherPriority priority = DispatcherPriority.Normal)
    {
        ArgumentNullException.ThrowIfNull(dispatcher);
        ArgumentNullException.ThrowIfNull(action);

        if (dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            await dispatcher.InvokeAsync(action, priority);
        }
    }

    /// <summary>
    /// Asynchronously begins invoking the specified action on the dispatcher thread if required, otherwise executes it directly.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to use for invoking the action.</param>
    /// <param name="action">The action to be executed.</param>
    /// <param name="priority">The priority at which the action is invoked, if required. The default is <see cref="DispatcherPriority.Normal"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dispatcher"/> or <paramref name="action"/> is null.</exception>
    public static async Task BeginInvokeIfRequired(
        this Dispatcher dispatcher,
        Action action,
        DispatcherPriority priority = DispatcherPriority.Normal)
    {
        ArgumentNullException.ThrowIfNull(dispatcher);
        ArgumentNullException.ThrowIfNull(action);

        if (dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            await dispatcher.BeginInvoke(action, priority);
        }
    }
}