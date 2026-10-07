namespace Atc.Wpf.Hardware.Tests.Pickers;

public sealed class ProcessPickerTests
{
    private IReadOnlyList<ProcessSnapshot> snapshots = [];

    [StaFact]
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
}