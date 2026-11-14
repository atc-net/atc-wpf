namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class LabelAudioInputPickerContractTests : LabelPickerContractTests<LabelAudioInputPicker, AudioInputPicker, AudioDeviceInfo>
{
    protected override IReadOnlyList<string> ExtraForwardedFlags
        => ["ShowLivePreview"];

    protected override AudioDeviceInfo CreateItem(int id)
        => new AudioDeviceInfo($"input-{id}", $"Microphone {id}", AudioDeviceKind.Input, isEnabled: true, isDefault: false);

    protected override void SetState(
        AudioDeviceInfo item,
        DeviceState state)
        => item.State = state;
}