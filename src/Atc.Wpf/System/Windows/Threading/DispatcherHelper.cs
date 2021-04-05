// ReSharper disable once CheckNamespace
namespace System.Windows.Threading
{
    /// <summary>
    /// Processes all UI messages currently in the message queue.
    /// </summary>
    public static class DispatcherHelper
    {
        /// <summary>
        /// Runs the on main thread.
        /// </summary>
        /// <param name="action">The action.</param>
        public static void RunOnMainThread(Action action)
        {
            Application.Current.RunOnUiThread(action);
        }

        /// <summary>
        /// Does the events.
        /// </summary>
        public static void DoEvents()
        {
            // Create new nested message pump.
            var nestedFrame = new DispatcherFrame();

            // Dispatch a callback to the current message queue, when getting called,
            // this callback will end the nested message loop.
            // note that the priority of this callback should be lower than that of UI event messages.
            var exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrames), nestedFrame);
            try
            {
                // Pump the nested message loop, the nested message loop will immediately
                // process the messages left inside the message queue.
                Dispatcher.PushFrame(nestedFrame);
            }
            catch (InvalidOperationException)
            {
                // Dummy
            }

            // If the "exitFrame" callback is not finished, abort it.
            if (exitOperation.Status != DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }

        /// <summary>
        /// Exits the frames.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <returns>
        /// The <see cref="object" />.
        /// </returns>
        private static object? ExitFrames(object state)
        {
            if (state is DispatcherFrame frame)
            {
                frame.Continue = false;
            }

            return null;
        }
    }
}