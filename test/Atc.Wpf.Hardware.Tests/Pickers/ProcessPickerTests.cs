namespace Atc.Wpf.Hardware.Tests.Pickers;

public sealed class ProcessPickerTests
{
    private IReadOnlyList<ProcessSnapshot> snapshots = [];

    [WpfFact]
    public async Task SelectedProcessExits_AndItsPidIsReused_DoesNotRebindToTheNewProcess()
    {
        using var service = new ProcessService(() => snapshots, _ => null);
        var picker = new ProcessPicker(service)
        {
            ClearValueOnDisconnect = true,
            AutoRebindOnReconnect = true,
        };

        snapshots = [new ProcessSnapshot(100, "notepad", "a.txt", HasMainWindow: true)];
        await service.RefreshAsync();
        picker.Value = service.Processes[0];

        snapshots = [new ProcessSnapshot(100, "chrome", "Google", HasMainWindow: true)];
        await service.RefreshAsync();

        Assert.Null(picker.Value);
    }

    [WpfFact]
    [SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "Forcing a collection is how the leak is detected.")]
    public async Task PickerOnASharedService_IsCollectedOnceDropped()
    {
        using var sharedService = new ProcessService(() => snapshots, _ => null);
        snapshots = [new ProcessSnapshot(100, "notepad", "a.txt", HasMainWindow: true)];
        await sharedService.RefreshAsync();

        var reference = CreateSelectLoadAndUnloadPicker(sharedService);

        // Let the poll started by Loaded finish, so only a real subscription leak can keep the picker alive.
        await sharedService.RefreshAsync();
        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.False(reference.IsAlive);
        GC.KeepAlive(sharedService);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateSelectLoadAndUnloadPicker(
        ProcessService sharedService)
    {
        var picker = new ProcessPicker(sharedService);
        picker.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
        picker.Value = sharedService.Processes[0];
        picker.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));
        return new WeakReference(picker);
    }
}