namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class LabelWindowPickerContractTests : LabelPickerContractTests<LabelWindowPicker, WindowPicker, TopLevelWindowInfo>
{
    protected override TopLevelWindowInfo CreateItem(int id)
        => new TopLevelWindowInfo(id, $"Window {id}", "Class", processId: id, processName: $"process{id}");

    protected override void SetState(
        TopLevelWindowInfo item,
        DeviceState state)
        => item.State = state;
}