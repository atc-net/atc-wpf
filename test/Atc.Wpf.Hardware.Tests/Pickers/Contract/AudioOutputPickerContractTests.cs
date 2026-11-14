namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class AudioOutputPickerContractTests : DevicePickerContractTests<AudioOutputPicker, AudioDeviceInfo, IAudioDeviceService>
{
    protected override IAudioDeviceService CreateService(
        ObservableCollection<AudioDeviceInfo> devices,
        Func<Task> refreshAsync)
    {
        var service = Substitute.For<IAudioDeviceService>();
        service.Devices.Returns(devices);
        service.Kind.Returns(AudioDeviceKind.Output);
        service.RefreshAsync().Returns(_ => refreshAsync());
        return service;
    }

    protected override AudioOutputPicker CreatePicker(
        IAudioDeviceService service)
        => new(service);

    protected override AudioDeviceInfo CreateItem(int id)
        => new AudioDeviceInfo($"output-{id}", $"Speaker {id}", AudioDeviceKind.Output, isEnabled: true, isDefault: false);

    protected override void SetState(
        AudioDeviceInfo item,
        DeviceState state)
        => item.State = state;
}