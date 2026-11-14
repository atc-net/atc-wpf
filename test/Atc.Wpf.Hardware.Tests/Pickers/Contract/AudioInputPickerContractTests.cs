namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class AudioInputPickerContractTests : DevicePickerContractTests<AudioInputPicker, AudioDeviceInfo, IAudioDeviceService>
{
    protected override IAudioDeviceService CreateService(
        ObservableCollection<AudioDeviceInfo> devices,
        Func<Task> refreshAsync)
    {
        var service = Substitute.For<IAudioDeviceService>();
        service.Devices.Returns(devices);
        service.Kind.Returns(AudioDeviceKind.Input);
        service.RefreshAsync().Returns(_ => refreshAsync());
        return service;
    }

    protected override AudioInputPicker CreatePicker(
        IAudioDeviceService service)
        => new(service);

    protected override AudioDeviceInfo CreateItem(int id)
        => new AudioDeviceInfo($"input-{id}", $"Microphone {id}", AudioDeviceKind.Input, isEnabled: true, isDefault: false);

    protected override void SetState(
        AudioDeviceInfo item,
        DeviceState state)
        => item.State = state;
}