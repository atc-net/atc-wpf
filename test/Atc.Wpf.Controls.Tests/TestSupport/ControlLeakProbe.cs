namespace Atc.Wpf.Controls.Tests.TestSupport;

/// <summary>
/// Creates a control in a given lifecycle state and hands back only a <see cref="WeakReference"/> to it,
/// so a test can assert whether anything (e.g. a static event subscription) keeps the control alive.
/// Loaded/Unloaded are raised directly, so no window, template or theme resources are required.
/// </summary>
internal static class ControlLeakProbe
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static WeakReference CreateUnloaded(Type controlType)
        => new(Activator.CreateInstance(controlType));

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static WeakReference CreateLoaded(Type controlType)
    {
        var control = (FrameworkElement)Activator.CreateInstance(controlType)!;
        control.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
        return new WeakReference(control);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static WeakReference CreateLoadedThenUnloaded(Type controlType)
    {
        var control = (FrameworkElement)Activator.CreateInstance(controlType)!;
        control.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
        control.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));
        return new WeakReference(control);
    }

    [SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "Forcing a collection is the point of a leak probe.")]
    public static bool IsCollected(WeakReference reference)
    {
        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        return !reference.IsAlive;
    }
}