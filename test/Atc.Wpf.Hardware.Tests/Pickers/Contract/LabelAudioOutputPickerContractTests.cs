namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class LabelAudioOutputPickerContractTests : LabelPickerContractTests<LabelAudioOutputPicker, AudioOutputPicker, AudioDeviceInfo>
{
    protected override IReadOnlyList<string> ExtraForwardedFlags
        => ["ShowLivePreview"];

    protected override AudioDeviceInfo CreateItem(int id)
        => new AudioDeviceInfo($"output-{id}", $"Speaker {id}", AudioDeviceKind.Output, isEnabled: true, isDefault: false);

    protected override void SetState(
        AudioDeviceInfo item,
        DeviceState state)
        => item.State = state;
}