// ReSharper disable once CheckNamespace
namespace System.Windows.Threading
{
    /// <summary>
    /// Extension methods for Dispatcher.
    /// </summary>
    public static class DispatcherExtensions
    {
        /// <summary>
        /// Invokes if required.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <param name="priority">The priority.</param>
        public static void InvokeIfRequired(
            this Dispatcher dispatcher,
            Action action,
            DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (dispatcher is null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

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
        /// Begins the invoke if required.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <param name="priority">The priority.</param>
        public static void BeginInvokeIfRequired(
            this Dispatcher dispatcher,
            Action action,
            DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (dispatcher is null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.BeginInvoke(action, priority);
            }
        }
    }
}