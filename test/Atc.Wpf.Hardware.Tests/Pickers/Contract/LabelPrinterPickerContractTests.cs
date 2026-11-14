namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class LabelPrinterPickerContractTests : LabelPickerContractTests<LabelPrinterPicker, PrinterPicker, PrinterInfo>
{
    protected override PrinterInfo CreateItem(int id)
        => new PrinterInfo($"Printer {id}", $"Printer {id}", isLocal: true, isShared: false, isDefault: false, queueStatus: "Ready");

    protected override void SetState(
        PrinterInfo item,
        DeviceState state)
        => item.State = state;
}