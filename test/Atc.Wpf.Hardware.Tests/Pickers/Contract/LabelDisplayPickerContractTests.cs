namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class LabelDisplayPickerContractTests : LabelPickerContractTests<LabelDisplayPicker, DisplayPicker, DisplayInfo>
{
    protected override DisplayInfo CreateItem(int id)
        => new DisplayInfo(id, $"DISPLAY{id}", new Rect(0, 0, 1920, 1080), new Rect(0, 0, 1920, 1040), isPrimary: id == 1);

    protected override void SetState(
        DisplayInfo item,
        DeviceState state)
        => item.State = state;
}