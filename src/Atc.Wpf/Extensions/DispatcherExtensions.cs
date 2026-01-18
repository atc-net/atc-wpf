namespace Atc.Wpf.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Dispatcher"/> to invoke actions based on thread access requirements.
/// </summary>
public static class DispatcherExtensions
{
    /// <summary>
    /// Asynchronously begins invoking the specified action on the dispatcher thread if required, otherwise executes it directly.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to use for invoking the action.</param>
    /// <param name="invokeAction">The action to be executed.</param>
    /// <param name="priority">The priority at which the action is invoked, if required. The default is <see cref="DispatcherPriority.Normal"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dispatcher"/> or <paramref name="invokeAction"/> is null.</exception>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Required to propagate exceptions through TaskCompletionSource.")]
    public static Task BeginInvokeIfRequired(
        this Dispatcher dispatcher,
        Action invokeAction,
        DispatcherPriority priority = DispatcherPriority.Normal)
    {
        ArgumentNullException.ThrowIfNull(dispatcher);
        ArgumentNullException.ThrowIfNull(invokeAction);

        if (dispatcher.CheckAccess())
        {
            invokeAction();
            return Task.CompletedTask;
        }

        var tcs = new TaskCompletionSource<object?>();
        _ = dispatcher.BeginInvoke(
            priority,
            new Action(() =>
            {
                try
                {
                    invokeAction();
                    tcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
        return tcs.Task;
    }
}