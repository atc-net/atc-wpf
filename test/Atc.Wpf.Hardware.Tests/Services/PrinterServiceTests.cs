namespace Atc.Wpf.Hardware.Tests.Services;

public sealed class PrinterServiceTests
{
    private IReadOnlyList<PrinterSnapshot> snapshots = [];

    [WpfFact]
    public async Task RefreshAsync_PrinterRemoved_MarksEntryDisconnected()
    {
        using var service = CreateService();
        snapshots = [Office(isDefault: true, queueStatus: "None")];
        await service.RefreshAsync();

        snapshots = [];
        await service.RefreshAsync();

        Assert.Single(service.Printers);
        Assert.Equal(DeviceState.Disconnected, service.Printers[0].State);
    }

    [WpfFact]
    public async Task RefreshAsync_DefaultPrinterAndQueueStatusChanged_UpdatesTheEntryInPlace()
    {
        using var service = CreateService();
        snapshots = [Office(isDefault: true, queueStatus: "None")];
        await service.RefreshAsync();
        var entry = service.Printers[0];

        snapshots = [Office(isDefault: false, queueStatus: "Printing")];
        await service.RefreshAsync();

        Assert.Same(entry, Assert.Single(service.Printers));
        Assert.False(entry.IsDefault);
        Assert.Equal("Printing", entry.QueueStatus);
    }

    [WpfFact]
    public async Task RefreshAsync_QueriesTheSpoolerOffTheUiThread_AndAppliesOnIt()
    {
        var uiThreadId = Environment.CurrentManagedThreadId;
        int? enumerateThreadId = null;
        int? collectionChangedThreadId = null;
        using var service = new PrinterService(enumerate: () =>
        {
            enumerateThreadId = Environment.CurrentManagedThreadId;
            return [Office(isDefault: true, queueStatus: "None")];
        });
        service.Printers.CollectionChanged += (_, _) => collectionChangedThreadId = Environment.CurrentManagedThreadId;

        await service.RefreshAsync();

        Assert.NotEqual(uiThreadId, enumerateThreadId);
        Assert.Equal(uiThreadId, collectionChangedThreadId);
        Assert.Single(service.Printers);
    }

    private static PrinterSnapshot Office(
        bool isDefault,
        string queueStatus)
        => new("Office", @"\\print01\Office", IsLocal: false, IsShared: true, isDefault, queueStatus);

    private PrinterService CreateService()
        => new(enumerate: () => snapshots);
}