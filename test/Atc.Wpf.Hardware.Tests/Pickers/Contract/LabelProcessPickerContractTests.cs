namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class LabelProcessPickerContractTests : LabelPickerContractTests<LabelProcessPicker, ProcessPicker, RunningProcessInfo>
{
    protected override RunningProcessInfo CreateItem(int id)
        => new RunningProcessInfo(id, $"process{id}", $"Window {id}", mainModulePath: null);

    protected override void SetState(
        RunningProcessInfo item,
        DeviceState state)
        => item.State = state;
}