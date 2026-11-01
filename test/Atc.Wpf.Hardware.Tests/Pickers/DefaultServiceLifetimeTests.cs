namespace Atc.Wpf.Hardware.Tests.Pickers;

/// <summary>
/// Pickers built with their parameterless constructor create their own service. These tests go through the
/// real services (the WinRT device watcher or the polling timer), because a leak would come from a reference
/// cycle across the COM boundary or a rooted timer, which a fake cannot reproduce.
/// </summary>
[SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "Forcing a collection is how the leak is detected.")]
public sealed class DefaultServiceLifetimeTests
{
    [WpfFact]
    public async Task WatcherPicker_NeverLoaded_ItsServiceIsCollectedOnceDropped()
    {
        var reference = CreateAndDropCameraPicker(load: false);

        Assert.False(await IsAliveAfterCollectingAsync(reference));
    }

    [WpfFact]
    public async Task WatcherPicker_LoadedAndUnloaded_ItsServiceIsCollectedOnceDropped()
    {
        var reference = CreateAndDropCameraPicker(load: true);

        Assert.False(await IsAliveAfterCollectingAsync(reference));
    }

    [WpfFact]
    public async Task AudioPicker_LoadedAndUnloaded_ItsServiceIsCollectedOnceDropped()
    {
        var reference = CreateAndDropAudioOutputPicker();

        Assert.False(await IsAliveAfterCollectingAsync(reference));
    }

    [WpfFact]
    public async Task PollingPicker_LoadedAndUnloaded_ItsServiceIsCollectedOnceDropped()
    {
        var reference = CreateAndDropDrivePicker();

        Assert.False(await IsAliveAfterCollectingAsync(reference));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateAndDropCameraPicker(bool load)
    {
        var picker = new UsbCameraPicker();
        if (load)
        {
            picker.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
            picker.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));
        }

        return new WeakReference(picker.Service);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateAndDropAudioOutputPicker()
    {
        var picker = new AudioOutputPicker();
        picker.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
        picker.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));
        return new WeakReference(picker.Service);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateAndDropDrivePicker()
    {
        var picker = new DrivePicker();
        picker.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
        picker.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));
        return new WeakReference(picker.Service);
    }

    private static async Task<bool> IsAliveAfterCollectingAsync(
        WeakReference reference)
    {
        // The initial refresh started by Loaded may still be running; a real leak survives every attempt.
        for (var attempt = 0; attempt < 20 && reference.IsAlive; attempt++)
        {
            await Task.Delay(50);
            await Dispatcher.Yield(DispatcherPriority.ApplicationIdle);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        return reference.IsAlive;
    }
}