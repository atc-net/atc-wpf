namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class PrinterPickerContractTests : DevicePickerContractTests<PrinterPicker, PrinterInfo, IPrinterService>
{
    protected override IPrinterService CreateService(
        ObservableCollection<PrinterInfo> devices,
        Func<Task> refreshAsync)
    {
        var service = Substitute.For<IPrinterService>();
        service.Printers.Returns(devices);
        service.RefreshAsync().Returns(_ => refreshAsync());
        return service;
    }

    protected override PrinterPicker CreatePicker(IPrinterService service)
        => new(service);

    protected override PrinterInfo CreateItem(int id)
        => new PrinterInfo($"Printer {id}", $"Printer {id}", isLocal: true, isShared: false, isDefault: false, queueStatus: "Ready");

    protected override void SetState(
        PrinterInfo item,
        DeviceState state)
        => item.State = state;
}